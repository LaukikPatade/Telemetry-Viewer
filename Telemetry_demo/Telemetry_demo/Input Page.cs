﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml.Serialization;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.IO;

namespace Telemetry_demo
{
    public partial class UserControl1 : UserControl
    {
        #region Fields
        private readonly List<InputConfig> configs = new List<InputConfig>();
        private ListBox lstConfiguredInputs;
        private TextBox txtSyncByte;
        private TextBox txtColumnNames;
        private Label lblSyncByte;
        private Label lblColumnNames;
        private FlowLayoutPanel dynamicPanel;
        private TableLayoutPanel tableLayoutPanel;
        private readonly List<TextBox> columnTextBoxes = new List<TextBox>();
        private Button btnAddColumn;
        private readonly ConfigManager configManager = new ConfigManager();
        private Thread udpThread;
        private UdpDataReceiver udpReceiver;
        #endregion

        #region Constructor
        public UserControl1()
        {
            try
            {
                InitializeComponent();
                LoadInitialAssets();
                InitializeControls();
                SetupPanelEvents();
                InitializeSyncByteControls();
                //configManager.SetConfigPath();
            }
            catch (Exception ex)
            {
                HandleError("Failed to initialize Input Page", ex);
            }
        }
        #endregion

        #region Initialization Methods
        private void InitializeControls()
        {
            try
            {
                lstConfiguredInputs = new ListBox();
                groupBoxChannels.Enabled = false;
                btnSaveChannels.Click += (s, e) => SaveChannelsFromGrid(ConnName.Text);
                dgvChannels.CellClick += dgvChannels_CellClick;
            }
            catch (Exception ex)
            {
                HandleError("Failed to initialize controls", ex);
            }
        }

        private void LoadInitialAssets()
        {
            try
            {
                LoadSavedConfigs();
                LoadComPorts();
                LoadBaudRates();
                LoadModes();
                LoadConnectionTypes();
            }
            catch (Exception ex)
            {
                HandleError("Failed to load initial assets", ex);
            }
        }

        private void LoadSavedConfigs()
        {
            try
            {
                ConfigManager.SetConfigPath();
                List<InputConfig> configs = ConfigManager.LoadConfigs();
                configList.Items.Clear();
                foreach (InputConfig config in configs)
                {
                    configList.Items.Add($"{config.InputName}: {config.Port} - {config.BaudRate}");
                }
            }
            catch (Exception ex)
            {
                HandleError("Failed to load saved configurations", ex);
            }
        }

        private void LoadComPorts()
        {
            try
            {
                string[] ports = SerialPort.GetPortNames();
                ComportBox.Items.Clear();
                ComportBox.Items.AddRange(ports);
                
                if (ComportBox.Items.Count > 0)
                {
                    ComportBox.SelectedIndex = 0;
                }
                else
                {
                    ShowWarning("No COM ports found!");
                }
            }
            catch (Exception ex)
            {
                HandleError("Failed to load COM ports", ex);
            }
        }

        private void LoadBaudRates()
        {
            try
            {
                int[] baud = { 9600, 19200, 38400, 57600, 115200, 230400, 460800, 921600 };
                BaudComboBox.Items.Clear();
                BaudComboBox.Items.AddRange(baud.Select(rate => rate.ToString()).ToArray());
                BaudComboBox.SelectedIndex = 0;
                
                // Enable custom text input
                BaudComboBox.DropDownStyle = ComboBoxStyle.DropDown;
                
                // Add tooltip to guide users
                toolTip1.SetToolTip(BaudComboBox, "Select from common baud rates or enter a custom value (e.g., 150000)");
                
                // Add validation for numeric input only
                BaudComboBox.KeyPress += (s, e) =>
                {
                    if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                    {
                        e.Handled = true;
                    }
                };
            }
            catch (Exception ex)
            {
                HandleError("Failed to load baud rates", ex);
            }
        }

        private void LoadModes()
        {
            try
            {
                string[] modes = { "CSV Mode", "Binary Mode" };
                ModeComboBox.Items.Clear();
                ModeComboBox.Items.AddRange(modes);
                ModeComboBox.SelectedIndex = 0;
                ModeComboBox.SelectedIndexChanged += ModeComboBox_SelectedIndexChanged;
            }
            catch (Exception ex)
            {
                HandleError("Failed to load modes", ex);
            }
        }

        private void ModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string selectedMode = ModeComboBox.SelectedItem?.ToString() ?? "CSV Mode";
                bool isBinaryMode = selectedMode == "Binary Mode";
                
                // Show/hide sync byte controls based on mode
                SyncByteLabel.Visible = isBinaryMode;
                SyncByteTextBox.Visible = isBinaryMode;
                
                // Adjust group box height if needed
                if (isBinaryMode)
                {
                    groupBoxConnection.Height = 300; // Increase height for sync byte controls
                }
                else
                {
                    groupBoxConnection.Height = 260; // Default height
                }
            }
            catch (Exception ex)
            {
                HandleError("Failed to update mode UI", ex);
            }
        }

        private void LoadConnectionTypes()
        {
            try
            {
                string[] connTypes = { "UART", "UDP" };
                ConnTypeComboBox.Items.Clear();
                ConnTypeComboBox.Items.AddRange(connTypes);
                ConnTypeComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                HandleError("Failed to load connection types", ex);
            }
        }

        private void SetupPanelEvents()
        {
            try
            {
                // Find all panels in the control and wire up the Paint event
                foreach (Control control in this.Controls)
                {
                    if (control is Panel panel)
                    {
                        panel.Paint += Panel_Paint;
                    }
                }
            }
            catch (Exception ex)
            {
                HandleError("Failed to setup panel events", ex);
            }
        }

        private void Panel_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                Panel panel = sender as Panel;
                if (panel != null)
                {
                    // Draw a border around the panel
                    using (Pen pen = new Pen(Color.LightGray, 1))
                    {
                        e.Graphics.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1);
                    }
                }
            }
            catch (Exception ex)
            {
                HandleError("Failed to paint panel", ex);
            }
        }

        private void InitializeSyncByteControls()
        {
            try
            {
                // Initially hide sync byte controls
                SyncByteLabel.Visible = false;
                SyncByteTextBox.Visible = false;
                
                // Set default value and tooltip
                SyncByteTextBox.Text = "0x00";
                toolTip1.SetToolTip(SyncByteTextBox, "Enter sync byte in hexadecimal format (e.g., 0x00, 0xFF)");
                
                // Add validation for hex input
                SyncByteTextBox.KeyPress += (s, e) =>
                {
                    if (!char.IsControl(e.KeyChar) && 
                        !char.IsDigit(e.KeyChar) && 
                        e.KeyChar != 'x' && 
                        e.KeyChar != 'X' && 
                        e.KeyChar != 'a' && 
                        e.KeyChar != 'A' && 
                        e.KeyChar != 'b' && 
                        e.KeyChar != 'B' && 
                        e.KeyChar != 'c' && 
                        e.KeyChar != 'C' && 
                        e.KeyChar != 'd' && 
                        e.KeyChar != 'D' && 
                        e.KeyChar != 'e' && 
                        e.KeyChar != 'E' && 
                        e.KeyChar != 'f' && 
                        e.KeyChar != 'F')
                    {
                        e.Handled = true;
                    }
                };
            }
            catch (Exception ex)
            {
                HandleError("Failed to initialize sync byte controls", ex);
            }
        }
        #endregion

        #region Event Handlers
        private void ConnTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string selectedType = ConnTypeComboBox.SelectedItem?.ToString() ?? "UART";
                UpdateConnectionTypeUI(selectedType);
            }
            catch (Exception ex)
            {
                HandleError("Failed to update connection type UI", ex);
            }
        }

        private void UpdateConnectionTypeUI(string selectedType)
        {
            bool isUART = selectedType == "UART";
            ComportBox.Visible = isUART;
            ComportBoxLabel.Visible = isUART;
            UDPPortTextBox.Visible = !isUART;
            UDPPortLbl.Visible = !isUART;
        }

        private void BtnSaveConn_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateConnectionInputs())
                    return;

                var inputConfig = CreateInputConfig();
                if (inputConfig == null)
                    return;

                SaveConfiguration(inputConfig);
            }
            catch (Exception ex)
            {
                HandleError("Failed to save connection", ex);
            }
        }

        private bool ValidateConnectionInputs()
        {
            if (string.IsNullOrWhiteSpace(ConnName.Text))
            {
                ShowWarning("Please enter a connection name.");
                return false;
            }

            // Validate baud rate - can be from dropdown or custom input
            string baudRateText = BaudComboBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(baudRateText))
            {
                ShowWarning("Please enter or select a baud rate.");
                return false;
            }

            // Validate that baud rate is a valid positive integer
            if (!int.TryParse(baudRateText, out int baudRate) || baudRate <= 0)
            {
                ShowWarning("Please enter a valid positive baud rate number.");
                return false;
            }

            if (ModeComboBox.SelectedItem == null)
            {
                ShowWarning("Please select a connection mode.");
                return false;
            }

            if (ConnTypeComboBox.SelectedItem == null)
            {
                ShowWarning("Please select a connection type.");
                return false;
            }

            string connType = ConnTypeComboBox.SelectedItem.ToString();
            if (connType == "UART" && ComportBox.SelectedItem == null)
            {
                ShowWarning("Please select a COM port.");
                return false;
            }
            else if (connType == "UDP" && string.IsNullOrWhiteSpace(UDPPortTextBox.Text))
            {
                ShowWarning("Please enter a UDP port.");
                return false;
            }

            // Validate sync byte for binary mode
            string selectedMode = ModeComboBox.SelectedItem.ToString();
            if (selectedMode == "Binary Mode")
            {
                if (string.IsNullOrWhiteSpace(SyncByteTextBox.Text))
                {
                    ShowWarning("Please enter a sync byte for binary mode.");
                    return false;
                }

                // Validate hex format
                if (!IsValidHexString(SyncByteTextBox.Text))
                {
                    ShowWarning("Please enter a valid hexadecimal sync byte (e.g., 0x00, 0xFF).");
                    return false;
                }
            }

            return true;
        }

        private bool IsValidHexString(string hexString)
        {
            try
            {
                // Remove 0x prefix if present
                string cleanHex = hexString.Replace("0x", "").Replace("0X", "");
                
                // Check if it's a valid hex string
                if (cleanHex.Length == 0 || cleanHex.Length > 2)
                    return false;
                
                // Try to parse as hex
                Convert.ToByte(cleanHex, 16);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private InputConfig CreateInputConfig()
        {
            string connType = ConnTypeComboBox.SelectedItem.ToString();
            string port = connType == "UART" ? ComportBox.SelectedItem.ToString() : UDPPortTextBox.Text;
            int baudRate = int.Parse(BaudComboBox.Text.Trim());
            string inputMode = ModeComboBox.SelectedItem.ToString();
            string inputName = ConnName.Text;

            if (sharedInputs.Configurations.ContainsKey(inputName))
            {
                ShowWarning("Input with that name already exists. Please select another name.");
                return null;
            }

            var config = new InputConfig(port, baudRate, inputMode, inputName, connType);
            
            // Add sync byte information for binary mode
            if (inputMode == "Binary Mode" && !string.IsNullOrWhiteSpace(SyncByteTextBox.Text))
            {
                string syncByteHex = SyncByteTextBox.Text.Replace("0x", "").Replace("0X", "");
                byte syncByte = Convert.ToByte(syncByteHex, 16);
                config.SyncByte = syncByte;
            }

            return config;
        }

        private void SaveConfiguration(InputConfig inputConfig)
        {
            configs.Add(inputConfig);
            sharedInputs.AddConfiguration(inputConfig);
            groupBoxChannels.Enabled = true;
            ShowSuccess("Connection saved successfully!");
        }

        private void SaveChannelsFromGrid(string configName)
        {
            try
            {
                if (string.IsNullOrEmpty(configName))
                {
                    ShowWarning("Please enter a configuration name.");
                    return;
                }

                var currentConfig = ValidateAndGetConfig(configName);
                if (currentConfig == null)
                    return;

                var channelInfos = ExtractChannelInfos();
                if (channelInfos == null || channelInfos.Count == 0)
                {
                    ShowWarning("No valid channels found.");
                    return;
                }

                UpdateAndSaveConfig(currentConfig, channelInfos);
                LoadSavedConfigs();
            }
            catch (Exception ex)
            {
                HandleError("Failed to save channels", ex);
            }
        }

        private InputConfig ValidateAndGetConfig(string configName)
        {
            var config = configs.FirstOrDefault(c => c.InputName == configName);
            if (config == null)
            {
                ShowWarning($"Configuration '{configName}' not found. Please save the connection first.");
                return null;
            }
            return config;
        }

        private List<ChannelInfo> ExtractChannelInfos()
        {
            try
            {
                var channelInfos = new List<ChannelInfo>();
                foreach (DataGridViewRow row in dgvChannels.Rows)
                {
                    if (row.IsNewRow) continue;
                    var nameCell = row.Cells["ChannelName"]?.Value;
                    var typeCell = row.Cells["ChannelType"]?.Value;
                    if (nameCell != null && !string.IsNullOrWhiteSpace(nameCell.ToString()) &&
                        typeCell != null && !string.IsNullOrWhiteSpace(typeCell.ToString()))
                    {
                        channelInfos.Add(new ChannelInfo
                        {
                            Name = nameCell.ToString(),
                            Type = typeCell.ToString()
                        });
                    }
                }
                return channelInfos;
            }
            catch (Exception ex)
            {
                HandleError("Could not extract channel infos", ex);
            }
            return null;
        }

        private void UpdateAndSaveConfig(InputConfig config, List<ChannelInfo> channelInfos)
        {
            config.ChannelConfig = new inputStructure { Channels = channelInfos };
            ConfigManager.SaveConfig(config);
            ShowSuccess($"Saved {channelInfos.Count} channels for {config.InputName}.");
        }

        private void dgvChannels_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.ColumnIndex == dgvChannels.Columns["ChannelColor"].Index)
                {
                    ShowColorDialog(e.RowIndex);
                }
            }
            catch (Exception ex)
            {
                HandleError("Failed to handle cell click", ex);
            }
        }

        private void ShowColorDialog(int rowIndex)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    dgvChannels.Rows[rowIndex].Cells["ChannelColor"].Style.BackColor = colorDialog.Color;
                    dgvChannels.Rows[rowIndex].Cells["ChannelColor"].Value = colorDialog.Color.Name;
                }
            }
        }

        private void panelChannelCard_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                Panel panel = sender as Panel;
                if (panel != null)
                {
                    // Draw a border around the panel
                    using (Pen pen = new Pen(Color.LightGray, 1))
                    {
                        e.Graphics.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1);
                    }
                }
            }
            catch (Exception ex)
            {
                HandleError("Failed to paint channel card", ex);
            }
        }
        #endregion

        #region Error Handling
        private void HandleError(string message, Exception ex)
        {
            string errorMessage = $"{message}\nError: {ex.Message}";
            if (ex.InnerException != null)
            {
                errorMessage += $"\nInner Error: {ex.InnerException.Message}";
            }
            MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            LogError(errorMessage, ex);
        }

        private void ShowWarning(string message)
        {
            MessageBox.Show(message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ShowSuccess(string message)
        {
            MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void LogError(string message, Exception ex)
        {
            try
            {
                string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error.log");
                string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}\nStack Trace: {ex.StackTrace}\n\n";
                File.AppendAllText(logPath, logMessage);
            }
            catch
            {
                // If logging fails, we can't do much about it
            }
        }
        #endregion

        #region UDP Implementation
        public class UdpDataReceiver : IDisposable
        {
            private readonly UdpClient udpClient;
            private readonly int port;
            private Thread listenThread;
            private bool listening = true;
            private bool disposed = false;

            public delegate void DataReceivedHandler(string data);
            public event DataReceivedHandler OnDataReceived;

            public UdpDataReceiver(int port)
            {
                try
                {
                    this.port = port;
                    udpClient = new UdpClient(port);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to initialize UDP receiver on port {port}", ex);
                }
            }

            public void StartListening()
            {
                try
                {
                    listenThread = new Thread(ListenForData)
                    {
                        IsBackground = true
                    };
                    listenThread.Start();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Failed to start UDP listener", ex);
                }
            }

            private void ListenForData()
            {
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, port);
                try
                {
                    while (listening && !disposed)
                    {
                        byte[] receivedBytes = udpClient.Receive(ref remoteEndPoint);
                        string data = Encoding.ASCII.GetString(receivedBytes);
                        OnDataReceived?.Invoke(data);
                    }
                }
                catch (SocketException ex)
                {
                    if (!disposed)
                    {
                        throw new InvalidOperationException("UDP listener stopped unexpectedly", ex);
                    }
                }
            }

            public void StopListening()
            {
                listening = false;
                Dispose();
            }

            public void Dispose()
            {
                if (!disposed)
                {
                    disposed = true;
                    udpClient?.Close();
                    udpClient?.Dispose();
                }
            }
        }
        #endregion

      
    }
}