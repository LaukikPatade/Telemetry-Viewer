using System;
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

namespace Telemetry_demo
{
    public partial class UserControl1 : UserControl
    {
        private List<InputConfig> configs = new List<InputConfig>();
        private ListBox lstConfiguredInputs;
        private TextBox txtSyncByte;
        private TextBox txtColumnNames;
        private Label lblSyncByte;
        private Label lblColumnNames;
        private FlowLayoutPanel dynamicPanel;
        private TableLayoutPanel tableLayoutPanel;
        private List<TextBox> columnTextBoxes = new List<TextBox>(); // Store column input boxes
        private Button btnAddColumn; // Button to add columns
        private ConfigManager configManager;
        private Thread udpThread;
        public UserControl1()
        {
            //StartUDP();
            InitializeComponent();
            LoadInitialAssets();
            lstConfiguredInputs = new ListBox();
            //udpThread = new Thread(StartUDP);
            //udpThread.IsBackground = true; // So it closes when the app closes
            //udpThread.Start();
        }

        
        private void LoadInitialAssets()
        {

            try
            {
                // Loading saved configs
                List<InputConfig> configs = ConfigManager.LoadConfigs();
                    foreach (InputConfig config in configs)
                    {
                        configList.Items.Add($"{config.InputName}: {config.Port} - {config.BaudRate}");
                    }

                // Loading COM Ports
                string[] ports = SerialPort.GetPortNames();

                ComportBox.Items.Clear();
                ComportBox.Items.AddRange(ports);
                if (ComportBox.Items.Count > 0)
                {
                    ComportBox.SelectedIndex = 0;
                }
                else
                {
                    MessageBox.Show("No COM ports found!");
                }

                // Loading Baud Rates
                int[] baud = { 9600, 19200, 38400, 57600, 115200, 230400, 460800, 921600 };

                BaudComboBox.Items.Clear();

                foreach (int rate in baud)
                {
                    BaudComboBox.Items.Add(rate.ToString());
                }

                BaudComboBox.SelectedIndex = 0;


                // Loading Modes
                string[] modes = { "CSV Mode", "Binary Mode" };

                ModeComboBox.Items.Clear();

                foreach (string mode in modes)
                {
                    ModeComboBox.Items.Add(mode);
                }
                ModeComboBox.SelectedIndex = 0;

                // Loading Connection Types
                string[] connTypes = { "UART", "UDP" };
                ConnTypeComboBox.Items.Clear();
                foreach (string connType in connTypes)
                {
                    ConnTypeComboBox.Items.Add(connType);
                }
                ConnTypeComboBox.SelectedIndex = 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        
        
        private void ConnTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedType = ConnTypeComboBox.SelectedItem.ToString();
            Console.WriteLine(selectedType);
            if (selectedType == "UART")
            {
                ComportBox.Visible = true;
                ComportBoxLabel.Visible = true;
                UDPPortTextBox.Visible = false;
                UDPPortLbl.Visible = false;
            }
            else if (selectedType == "UDP")
            {
                ComportBox.Visible = false;
                ComportBoxLabel.Visible = false;
                UDPPortTextBox.Visible = true;
                UDPPortLbl.Visible = true;
            }
        }


        private void LoadDynamicPanel(InputConfig inputConfig)
        {
            // Clear previous controls
            dynamicPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom, // Ensures it aligns correctly
                AutoSize = true,      // Makes it resize dynamically
                FlowDirection = FlowDirection.TopDown,
                BorderStyle = BorderStyle.FixedSingle, // Optional: Makes it visible for debugging
                Padding = new Padding(5)
            };
            this.Controls.Add(dynamicPanel);
            dynamicPanel.Controls.Clear();
            InitializeTableLayoutPanel();
            columnTextBoxes.Clear();

            if (ModeComboBox.SelectedItem.ToString() == "CSV Mode")
            {
                lblColumnNames = new Label { Text = "Enter Column Names (comma-separated):" };
                dynamicPanel.Controls.Add(lblColumnNames);

                /*AddColumnTextBox();*/
                btnAddColumn = new Button
                {
                    Text = "Add Channel",
                    AutoSize = true
                };

                btnAddColumn.Click += BtnAddColumn_Click;
                dynamicPanel.Controls.Add(btnAddColumn);
            }
            else if (ModeComboBox.SelectedItem.ToString() == "Binary Mode")
            {
                lblSyncByte = new Label { Text = "Enter Sync Byte (Hex format e.g. 0xFF):" };
                txtSyncByte = new TextBox { Width = 100 };

                dynamicPanel.Controls.Add(lblSyncByte);
                dynamicPanel.Controls.Add(txtSyncByte);
            }
            Button btnSaveChannels = new Button
            {
                Text="Save Channels",
                AutoSize = true
            };
            btnSaveChannels.Click += (sender, e) => SaveChannelsToConfig(inputConfig.InputName);

            dynamicPanel.Controls.Add(btnSaveChannels);
            // Refresh the UI to ensure changes appear
            dynamicPanel.Refresh();
            this.Refresh();
        }



        /*******************************************************************************UTILITY BUTTONS*********************************************************************************/

        // Button to save connections
        private void BtnSaveConn_Click(object sender, EventArgs e)
        {
            string port = "";
            int baudRate = 0;
            string inputMode = "";
            string connType = "UART";
            

            // Use SelectedItem for baud rate as well
            if (BaudComboBox.SelectedItem != null)
            {
                baudRate = int.Parse(BaudComboBox.SelectedItem.ToString());
            }
            else
            {
                MessageBox.Show("Please select a baud rate.");
            }

            if (ModeComboBox.SelectedItem != null)
            {
                inputMode = ModeComboBox.SelectedItem.ToString();
            }
            else
            {
                MessageBox.Show("Please select a connection type.");
            }

            if (ConnTypeComboBox.SelectedItem != null)
            {
                connType = ConnTypeComboBox.SelectedItem.ToString();
            }
            else
            {
                MessageBox.Show("Please select a connection type.");
            }
            

            if (connType == "UART")
            {
                if (ComportBox.SelectedItem != null)
                {

                    port = ComportBox.SelectedItem.ToString();

                }
                else
                {

                    MessageBox.Show("Please select a COM port.");
                    return; // Exit the constructor if no COM port is selected
                }
            }
            else
            {
                port= UDPPortTextBox.Text;
            }

            string inputName = ConnName.Text;
            if (sharedInputs.Configurations.ContainsKey(inputName))
            {
                MessageBox.Show("Input with that name already exists please select another input");
                return;
            }
            
            var inputConfig = new InputConfig(port, baudRate, inputMode, inputName, connType);
            configs.Add(inputConfig);
            sharedInputs.AddConfiguration(inputConfig);
            LoadDynamicPanel(inputConfig);
        }

        // Button to add columns
        private void BtnAddColumn_Click(object sender, EventArgs eventArgs)
        {
            AddChannelRow();
            dynamicPanel.Refresh();
        }


        /*******************************************************************************************************************************************************/




        private void InitializeTableLayoutPanel()
        {
            tableLayoutPanel = new TableLayoutPanel
            {
                ColumnCount = 3,
                AutoSize = true,
                Dock = DockStyle.Fill,
                CellBorderStyle=TableLayoutPanelCellBorderStyle.Single
            };
            dynamicPanel.Controls.Add(tableLayoutPanel);
        }


        /******************************************************************UTILITY FUNCTIONS***********************************************************************************/
        private void AddChannelRow()
        {
            int rowIndex = tableLayoutPanel.RowCount;

            Label lblChannel = new Label { Text = $"Channel {rowIndex + 1}", AutoSize = true };
            TextBox nameChannel = new TextBox { Text = "Enter Channel Name", AutoSize = true };
            Button btnRemove = new Button{Text = "X"};

            btnRemove.Click += (s, e) => RemoveChannelRow(rowIndex);

            // Add controls to the table panel
            tableLayoutPanel.Controls.Add(lblChannel, 0, rowIndex);
            tableLayoutPanel.Controls.Add(nameChannel, 1, rowIndex);
            tableLayoutPanel.Controls.Add(btnRemove, 2, rowIndex);

            // Increase the row count
            tableLayoutPanel.RowCount++;
        }
        private void RemoveChannelRow(int rowIndex)
        {
            for(int i = 0; i<tableLayoutPanel.ColumnCount; i++)
            {
                Control control = tableLayoutPanel.GetControlFromPosition(i, rowIndex);
                if(control!=null)
                {
                    tableLayoutPanel.Controls.Remove(control);
                    control.Dispose();
                }
            }
            for (int j = rowIndex + 1; j < tableLayoutPanel.RowCount; j++)
            {
                for(int i =0; i < tableLayoutPanel.ColumnCount; i++)
                {
                    Control control = tableLayoutPanel.GetControlFromPosition(j, i);
                    if(control!=null)
                    {
                        tableLayoutPanel.SetRow(control, i-1);
                    }
                }
            }

            tableLayoutPanel.RowCount--;
        }
        private void SaveChannelsToConfig(string inputName)
        {
            if(!sharedInputs.Configurations.ContainsKey(inputName))
            {
                MessageBox.Show("Error: No such input available");
                return;
            }

            var config = sharedInputs.Configurations[inputName];
            config.ChannelConfig.Channels.Clear();

            for (int i=0; i < tableLayoutPanel.RowCount; i++)
            {
                Control textBox = tableLayoutPanel.GetControlFromPosition(1, i);
                if (textBox is TextBox channelInput)
                {
                    string channelName = channelInput.Text;
                    if(!string.IsNullOrEmpty(channelName))
                    {
                        config.ChannelConfig.Channels.Add(channelName);
                    }
                }

            }
            MessageBox.Show($"Channels saved for {inputName}");
            Console.WriteLine(config.ChannelConfig.Channels.ToString());
            ConfigManager.SaveConfig(config);
        }


        public class UdpDataReceiver
        {
            private readonly UdpClient udpClient;
            private readonly int port;
            private Thread listenThread;
            private bool listening = true;

            public delegate void DataReceivedHandler(string data);
            public event DataReceivedHandler OnDataReceived;

            public UdpDataReceiver(int port)
            {
                this.port = port;
                udpClient = new UdpClient(port);
            }

            public void StartListening()
            {
                Console.WriteLine("STartintg to listen udp");
                listenThread = new Thread(() =>
                {
                    Console.WriteLine("yelloooo");
                    IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, port);
                    Console.WriteLine("haloooooooooo");
                    try
                    {
                        Console.WriteLine("jellooooooo");
                        Console.WriteLine(listening);
                        while (listening)
                        {
                            Console.WriteLine("listening");
                            byte[] receivedBytes = udpClient.Receive(ref remoteEndPoint);
                            Console.WriteLine("jellooooooo");
                            string data = Encoding.ASCII.GetString(receivedBytes);
                            Console.WriteLine(data);
                            // Fire event or handle data
                            OnDataReceived?.Invoke(data);
                        }
                    }
                    catch (SocketException ex)
                    {
                        Console.WriteLine("UDP listener stopped: " + ex.Message);
                    }
                });

                listenThread.IsBackground = true;
                listenThread.Start();
            }

            public void StopListening()
            {
                listening = false;
                udpClient.Close();
            }
        }

        UdpDataReceiver udpReceiver;
        private void StartUDP()
        {
            int port = 8080; // Use the same port Arduino is sending to

            UdpClient udpClient = new UdpClient();
            udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, port));

            Console.WriteLine($"Listening on UDP {port}...");

            while (true)
            {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = udpClient.Receive(ref remoteEP);
                string message = Encoding.ASCII.GetString(data);
                Console.WriteLine($"Received from {remoteEP}: {message}");
            }
        }
    }
}