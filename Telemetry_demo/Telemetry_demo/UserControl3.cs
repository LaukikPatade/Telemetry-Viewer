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
        /*private SerialPort serialPort;*/
        public UserControl3()
        {

            InitializeComponent();
        }

        // Event handler for "Add Grid" button
        private void BtnAddGrid_Click(object sender, EventArgs e)
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

        

        private void AddChartToPanel(Panel panel, string InputName)
        {
            // Extract COM port and Baud rate from shared configurations
            Dictionary<string, Series> channelSeries = new Dictionary<string, Series>();
            string ComPort = sharedInputs.Configurations[InputName].ComPort;
            int BaudRate = sharedInputs.Configurations[InputName].BaudRate;
            /*int BaudRate = 1500000;*/
            var channels = sharedInputs.Configurations[InputName].ChannelConfig.Channels;
            // Create a new Chart control
            Chart chart = new Chart
            {
                Dock = DockStyle.Fill
            };

            // Configure chart area
            ChartArea chartArea = new ChartArea("MainArea");
            chart.ChartAreas.Add(chartArea);

            // Create a data series
            channelSeries.Clear();
            foreach(string channel in channels)
            {
                Series series = new Series(channel)
                {
                    ChartType = SeriesChartType.Line,
                    BorderWidth = 2
                };
                channelSeries[channel] = series;
                chart.Series.Add(series);
            }
            /*Series series = new Series("Sensor Data")
            {
                ChartType = SeriesChartType.Line,
                BorderWidth = 2
            };*/
            /*chart.Series.Add(series);*/

            // Add chart to the panel
            panel.Controls.Clear(); // Clear any existing controls
            panel.Controls.Add(chart);

            // Initialize and configure the SerialPort
            SerialPort serialPort = new SerialPort(ComPort, BaudRate)
            {
                DtrEnable = true, // Ensure data is received properly
                RtsEnable = true
            };

            try
            {
                serialPort.Open();

                // Run data reading in a separate task
                Task.Run(() =>
                {
                    while (serialPort.IsOpen)
                    {
                        try
                        {
                            string data = serialPort.ReadLine(); // Read incoming data
                            ProcessCSVData(data, channels, channelSeries,panel,chart);

                            /*if (double.TryParse(data, out double sensorValue))
                            {
                                // Update chart on UI thread
                                *//*panel.Invoke(new Action(() =>
                                {
                                    if (series.Points.Count > 100) // Keep chart efficient
                                        series.Points.RemoveAt(0);

                                    series.Points.AddY(sensorValue);
                                }));*//*
                            }*/
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error reading from {ComPort}: {ex.Message}");
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open {ComPort}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }





        // Close the serial port connection when done
        public void DisconnectSerialPort(SerialPort serialPort)
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
                Console.WriteLine("Serial port disconnected.");
            }
        }

        /*public void ProcessCSVData(string data, List<string> channels, Dictionary<string,Series> channelSeries,Panel panel)
        {
            string[] values = data.Split(',');
            if (values.Length!=channels.Count) { return; }

            panel.Invoke(new Action(() =>
            {
                for(int i = 0; i < channels.Count; i++)
                {
                if (double.TryParse(values[i], out double parseValue))
                    {
                        string channel = channels[i];
                        if (channelSeries[channel].Points.Count > 100) channelSeries[channel].Points.RemoveAt(0);

                        channelSeries[channel].Points.AddY(parseValue);
                    }
                }
            }));
        }*/
        public void ProcessCSVData(string data, List<string> channels, Dictionary<string, Series> channelSeries, Panel panel,Chart chart)
        {
            string[] values = data.Split(','); // Split CSV values
            if (values.Length != channels.Count) return; // Ensure data matches channel count

            panel.Invoke(new Action(() =>
            {
                for (int i = 0; i < values.Length; i++)
                {
                    if (double.TryParse(values[i], out double parsedValue))
                    {
                        string channel = channels[i];

                        // Ensure only selected channels are plotted
                        /*if (!channelCheckBoxes.ContainsKey(channel) || !channelCheckBoxes[channel].Checked)
                            continue;*/

                        Series series = channelSeries[channel];

                        // Maintain a rolling window of 100 points
                        if (series.Points.Count > 100)
                        {
                            series.Points.RemoveAt(0); // Remove oldest data
                        }

                        // Add new data point with incremented X-value
                        double xValue = series.Points.Count > 0 ? series.Points.Last().XValue + 1 : 0;
                        series.Points.AddXY(xValue, parsedValue);

                        // Adjust X-axis to create a scrolling effect
                        chart.ChartAreas[0].AxisX.Minimum = series.Points.First().XValue;
                        chart.ChartAreas[0].AxisX.Maximum = series.Points.Last().XValue;
                    }
                }
            }));
        }










    }
}
