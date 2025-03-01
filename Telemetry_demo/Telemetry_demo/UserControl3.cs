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
    public partial class UserControl3 : UserControl
    {
        private SerialPort serialPort;
        private Chart chart;
        private Panel panel;
        public UserControl3()
        {

            InitializeComponent();
        }

        private void UserControl3_Load(object sender, EventArgs e)
        {

        }


        // Event handler for "Add Grid" button
        // Event handler for "Add Grid" button
        private void btnAddGrid_Click(object sender, EventArgs e)
        {
            int rows = Convert.ToInt32(txtRows.Text);
            int columns = Convert.ToInt32(txtColumns.Text);

            TableLayoutPanel grid = new TableLayoutPanel
            {
                RowCount = rows,
                ColumnCount = columns,
                Dock = DockStyle.Fill
            };

            for (int i = 0; i < rows; i++)
            {
                grid.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / rows));
            }
            for (int i = 0; i < columns; i++)
            {
                grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / columns));
            }

            // Add panels with buttons to simulate gridlines and add chart functionality
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    // Create a panel for each cell
                    Panel panel = new Panel();
                    panel.Dock = DockStyle.Fill;
                    panel.BorderStyle = BorderStyle.FixedSingle;

                    // Create an "Add Chart" button
                    ComboBox inputSelect = new ComboBox();
                    foreach (var config in sharedInputs.configs)
                    {
                        inputSelect.Items.Add(config.InputName);
                    }
                    inputSelect.Dock=DockStyle.Left;
                    Button addChartButton = new Button();
                    addChartButton.Text = "Add Chart";
                    addChartButton.Dock = DockStyle.Right;  // Place the button at the top of the panel

                    // Subscribe to the click event of the button
                    addChartButton.Click += (s, evt) => AddChartToPanel(panel,inputSelect.Text);

                    // Add the button to the panel
                    panel.Controls.Add(addChartButton);
                    panel.Controls.Add(inputSelect);
                    // Add the panel to the grid
                    grid.Controls.Add(panel, col, row);
                }
            }

            // Add the grid to the form
            this.Controls.Add(grid);
            grid.BringToFront();
        }

        // Method to handle adding a chart to a panel when the button is clicked
        private void AddChartToPanel(Panel panel,String InputName)
        {
            String ComPort = sharedInputs.Configurations[InputName].ComPort;
            int BaudRate = sharedInputs.Configurations[InputName].BaudRate;
            ConnectToSerialPort(ComPort,BaudRate);
            // Clear the panel (removes the button)
            panel.Controls.Clear();

            // Create a new chart
            Chart chart = new Chart
            {
                Dock = DockStyle.Fill
            };

            // Create a chart area and add it to the chart
            ChartArea chartArea = new ChartArea();
            chart.ChartAreas.Add(chartArea);

            // Optionally, add some series or data to the chart
            Series series = new Series
            {
                ChartType = SeriesChartType.Line
            };
            series.Points.AddXY(1, 10);
            series.Points.AddXY(2, 20);
            series.Points.AddXY(3, 15);
            chart.Series.Add(series);

            // Add the chart to the panel
            panel.Controls.Add(chart);

            // Force a layout update to ensure the chart is displayed
            //panel.ResumeLayout(true); // This forces the layout update
            //panel.PerformLayout();    // This ensures all the layout changes take effect
        }
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = serialPort.ReadLine();  // Read incoming data
                double sensorValue;

                // Parse the received data into a double value
                if (double.TryParse(data, out sensorValue))
                {
                    // Invoke the method to update the chart on the UI thread
                    panel.Invoke(new Action(() =>
                    {
                        AddDataPointToChart(sensorValue);
                    }));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading data: {ex.Message}");
            }
        }

        // Add data points to the chart continuously
        private void AddDataPointToChart(double sensorValue)
        {
            Series series = chart.Series["SensorData"];

            // Add a new data point to the series
            series.Points.AddY(sensorValue);

            // Optionally, keep the chart to show only the latest N points for a "scrolling" effect
            if (series.Points.Count > 100)
            {
                series.Points.RemoveAt(0);  // Remove oldest points to keep a consistent number of data points
            }

            chart.Invalidate(); // Redraw the chart
        }

        // Close the serial port connection when done
        public void DisconnectSerialPort()
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
                Console.WriteLine("Serial port disconnected.");
            }
        }


        //private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        //{
        //    string data = serialPort.ReadLine();
        //    if (int.TryParse(data, out int analogValue))
        //    {
        //        // Update the chart on the UI thread
        //        this.Invoke(new Action(() =>
        //        {
        //            sampleCount++;
        //            chartUART.Series["Analog Values"].Points.AddXY(sampleCount, analogValue);
        //            chartUART.ChartAreas[0].RecalculateAxesScale();
        //        }));
        //    }
        //}
        private void InitializeChart()
        {
            chart = new Chart();
            chart.Dock = DockStyle.Fill;

            ChartArea chartArea = new ChartArea();
            chartArea.AxisX.Title = "Time (s)";
            chartArea.AxisY.Title = "Sensor Data";
            chart.ChartAreas.Add(chartArea);

            Series series = new Series
            {
                Name = "SensorData",
                ChartType = SeriesChartType.Line,
                XValueType = ChartValueType.Int32,
                YValueType = ChartValueType.Double
            };
            chart.Series.Add(series);

            panel.Controls.Add(chart); // Add chart to panel
        }
        public void ConnectToSerialPort(string comPort, int baudRate)
        {
            serialPort = new SerialPort(comPort, baudRate);
            serialPort.DataReceived += SerialPort_DataReceived;

            try
            {
                serialPort.Open();
                MessageBox.Show("Serial port connected.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error connecting to serial port: {ex.Message}");
            }
        }

  
       
        

    }
}
