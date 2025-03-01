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

namespace Telemetry_demo
{
    public partial class UserControl1 : UserControl
    {
        SerialPort serialPort= new SerialPort();
        private int sampleCount = 0;
        public UserControl1()
        {
            InitializeComponent();
            InitializeChart();
            LoadCOMPorts();
            LoadBaudRates();
            
            //if (comboBox1.SelectedItem != null)
            //{
            //    serialPort.PortName = comboBox1.SelectedItem.ToString();
            //}
            //else
            //{
            //    MessageBox.Show("Please select a COM port.");
            //    return; // Exit the constructor if no COM port is selected
            //}

            //// Use SelectedItem for baud rate as well
            //if (comboBox2.SelectedItem != null)
            //{
            //    serialPort.BaudRate = int.Parse(comboBox2.SelectedItem.ToString());
            //}
            //else
            //{
            //    MessageBox.Show("Please select a baud rate.");
            //}
               
            //serialPort.Open();
        }
        private void InitializeChart()
        {
            // Configure the chart
            chartUART.Series.Clear();
            var series = new Series
            {
                Name = "Analog Values",
                Color = System.Drawing.Color.Blue,
                ChartType = SeriesChartType.Line,
                BorderWidth = 2
            };
            chartUART.Series.Add(series);
            chartUART.ChartAreas.Add(new ChartArea());
            chartUART.ChartAreas[0].AxisX.Title = "Samples";
            chartUART.ChartAreas[0].AxisY.Title = "Analog Value";
        }
        private void btnUARTConnect_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                serialPort.PortName = comboBox1.SelectedItem.ToString();
            }
            else
            {
                MessageBox.Show("Please select a COM port.");
                return; // Exit the constructor if no COM port is selected
            }

            // Use SelectedItem for baud rate as well
            if (comboBox2.SelectedItem != null)
            {
                serialPort.BaudRate = int.Parse(comboBox2.SelectedItem.ToString());
            }
            else
            {
                MessageBox.Show("Please select a baud rate.");
            }
            serialPort.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived);
            serialPort.Open();
            btnUARTConnect.Enabled = false;
            
        }
        private void LoadCOMPorts()
        {
            string[] ports =SerialPort.GetPortNames();

            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(ports);
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("No COM ports found!");
            }

        }
        private void LoadBaudRates()
        {
            int[] baud = { 9600, 19200, 38400, 57600, 115200, 230400, 460800, 921600 };

            comboBox2.Items.Clear();
            //comboBox2.Items.AddRange(baud);
            foreach (int rate in baud)
            {
                comboBox2.Items.Add(rate.ToString());
            }
            comboBox2.SelectedIndex = 0;

        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = serialPort.ReadLine();
            if (int.TryParse(data, out int analogValue))
            {
                // Update the chart on the UI thread
                this.Invoke(new Action(() =>
                {
                    sampleCount++;
                    chartUART.Series["Analog Values"].Points.AddXY(sampleCount, analogValue);
                    chartUART.ChartAreas[0].RecalculateAxesScale();
                }));
            }
        }
    }
}
