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


namespace Telemetry_demo
{
    public partial class UserControl1 : UserControl
    {
        SerialPort serialPort = new SerialPort();
        private int sampleCount = 0;
        private List<InputConfig> configs = new List<InputConfig>();
        private ListBox lstConfiguredInputs;
        private TextBox txtSyncByte;
        private TextBox txtColumnNames;
        private Label lblSyncByte;
        private Label lblColumnNames;
        private FlowLayoutPanel dynamicPanel;
        private List<TextBox> columnTextBoxes = new List<TextBox>(); // Store column input boxes
        private Button btnAddColumn; // Button to add columns
        public UserControl1()
        {
            InitializeComponent();
            LoadCOMPorts();
            LoadBaudRates();
            LoadModes();
            lstConfiguredInputs = new ListBox();

            // Initialize dynamic panel

            // Add the panel to an existing container (like the form or another panel)
            

            // Attach event handler for mode selection
            /*ModeComboBox.SelectedIndexChanged += ModeComboBox_SelectedIndexChanged;*/
        }

        private void BtnSaveConn_Click(object sender, EventArgs e)
        {
            string comPort = "";
            int baudRate = 0;
            string mode = "";
            if (ComportBox.SelectedItem != null)
            {

                comPort = ComportBox.SelectedItem.ToString();

            }
            else
            {

                MessageBox.Show("Please select a COM port.");
                return; // Exit the constructor if no COM port is selected
            }

            // Use SelectedItem for baud rate as well
            if (BaudComboBox.SelectedItem != null)
            {
                baudRate = int.Parse(BaudComboBox.SelectedItem.ToString());
            }
            else
            {
                MessageBox.Show("Please select a baud rate.");
            }
            string inputName = ConnName.Text;
            if (sharedInputs.Configurations.ContainsKey(inputName))
            {
                MessageBox.Show("Input with that name already exists please select another inptut");
                return;
            }
            var inputConfig = new InputConfig(comPort, baudRate, mode, inputName);
            configs.Add(inputConfig);
            sharedInputs.AddConfiguration(inputConfig);
            configList.Items.Add($"{inputName}: {sharedInputs.Configurations[inputName].ComPort} - {baudRate}");
            LoadDynamicPanel(inputConfig);
        }

        
        private void getInputStructure(InputConfig inputConfig)
        {
            
        }

        private void LoadCOMPorts()
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
                MessageBox.Show("No COM ports found!");
            }

        }
        private void LoadBaudRates()
        {

            int[] baud = { 9600, 19200, 38400, 57600, 115200, 230400, 460800, 921600 };

            BaudComboBox.Items.Clear();

            foreach (int rate in baud)
            {
                BaudComboBox.Items.Add(rate.ToString());
            }

            BaudComboBox.SelectedIndex = 0;
        }
        private void LoadModes()
        {
            string[] modes = { "CSV Mode", "Binary Mode" };

            ModeComboBox.Items.Clear();

            foreach (string mode in modes)
            {
                ModeComboBox.Items.Add(mode);
            }
            ModeComboBox.SelectedIndex = 0;

        }
        private void LoadDynamicPanel(InputConfig inputConfig)
        {
            // Clear previous controls
            Console.WriteLine("YIPEEE");
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
            columnTextBoxes.Clear();

            if (ModeComboBox.SelectedItem.ToString() == "CSV Mode")
            {
                lblColumnNames = new Label { Text = "Enter Column Names (comma-separated):" };
                dynamicPanel.Controls.Add(lblColumnNames);

                AddColumnTextBox();
                btnAddColumn = new Button
                {
                    Text = "Add",
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

            // Refresh the UI to ensure changes appear
            dynamicPanel.Refresh();
            this.Refresh();
        }
        /*private void ModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Clear previous controls
            dynamicPanel.Controls.Clear();
            columnTextBoxes.Clear();

            if (ModeComboBox.SelectedItem.ToString() == "CSV Mode")
            {
                lblColumnNames = new Label { Text = "Enter Column Names (comma-separated):" };
                dynamicPanel.Controls.Add(lblColumnNames);

                AddColumnTextBox();
                btnAddColumn=new Button 
                { 
                Text = "Add" ,
                AutoSize=true
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

            // Refresh the UI to ensure changes appear
            dynamicPanel.Refresh();
            this.Refresh();
        }*/

        private void AddColumnTextBox()
        {
            TextBox txtColumn = new TextBox { Width = 100 };
            columnTextBoxes.Add(txtColumn);
            dynamicPanel.Controls.Add(txtColumn);
        }

        private void BtnAddColumn_Click(object sender, EventArgs eventArgs)
        {
            AddColumnTextBox();
            dynamicPanel.Refresh();
        }
    }
}