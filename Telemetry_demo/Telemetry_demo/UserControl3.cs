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
using System.IO;


namespace Telemetry_demo
{
    public partial class UserControl3 : UserControl
    {
        /*private SerialPort serialPort;*/
        List<InputConfig> inputConfigs=ConfigManager.LoadConfigs();
        private StreamWriter csvWriter;
        string filePath = "test.csv";
        private int xCounter = 0;
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
                    Panel panel = new Panel
                    {
                        Dock = DockStyle.Fill,
                        BorderStyle = BorderStyle.FixedSingle
                };
                    

                    // Create an "Add Chart" button
                    ComboBox inputSelect = new ComboBox();
                    foreach (var config in inputConfigs)
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
        private void ResetAxis(Chart chart)
        {
            var chartArea = chart.ChartAreas[0];
            chartArea.AxisX.Minimum = double.NaN; // Reset to auto-scaling
            chartArea.AxisX.Maximum = double.NaN;
        }
         
        private void AddChartToPanel(Panel panel, string InputName)
        {
            //***************************************Declarations**********************************************//
            Dictionary<string, CheckBox> channelCheckBoxes = new Dictionary<string, CheckBox>();
            Dictionary<string, List<DataPoint>> allChannelData = new Dictionary<string, List<DataPoint>>();
            Panel checkBoxPanel= new Panel();
            double scrollOffset = 0;
            bool isChartFrozen = false;
            ToolTip toolTip = new ToolTip();
            Dictionary<string, Series> channelSeries = new Dictionary<string, Series>();
            InputConfig config = ConfigManager.searchConfig(InputName);
            string ComPort = config.ComPort;
            int BaudRate = config.BaudRate;
            var channels = config.ChannelConfig.Channels;
            // Create a new Chart control
            Chart chart = new Chart
            {
                Dock = DockStyle.Top
            };
            ChartArea chartArea = new ChartArea("MainArea");






            //***************************************Mouse Events on the Charts**********************************************//
            void Chart_MouseMove(object sender, MouseEventArgs e)
            {
                var chart_s = sender as Chart;
                if (chart_s == null) return;

                var result = chart_s.HitTest(e.X, e.Y);
                if (result.ChartElementType == ChartElementType.DataPoint)
                {
                    DataPoint point = chart_s.Series[0].Points[result.PointIndex];
                    toolTip.Show($"X: {point.XValue}, Y: {point.YValues[0]}", chart_s, e.X, e.Y - 15);
                }
                else
                {
                    toolTip.Hide(chart_s);
                }

            }

            void Chart_MouseWheel(object sender, MouseEventArgs e)
            {
                var chart_s = sender as Chart;
                if (chart_s == null) return;

                var chartArea_s = chart_s.ChartAreas[0];

                if (e.Delta > 0) // Scroll Up
                {
                    if (!isChartFrozen)
                    {
                        isChartFrozen = true; // Freeze when scrolling up
                        scrollOffset = 0; // Reset scroll position
                    }
                    else
                    {
                        // Move the viewport left (older data)
                        scrollOffset += 5; // Adjust this value for sensitivity
                        chartArea_s.AxisX.Minimum -= 5;
                        chartArea_s.AxisX.Maximum -= 5;
                    }
                }
                else if (e.Delta < 0) // Scroll Down
                {
                    if (isChartFrozen)
                    {
                        // Move the viewport right (newer data)
                        scrollOffset -= 5;
                        chartArea_s.AxisX.Minimum += 5;
                        chartArea_s.AxisX.Maximum += 5;

                        // If we scroll back to real-time, unfreeze
                        if (scrollOffset <= 0)
                        {
                            isChartFrozen = false;
                            ResetAxis(chart_s);
                        }
                    }
                    else
                    {
                        isChartFrozen = false; // Unfreeze when scrolling down
                    }
                }

                chart_s.Refresh();
            }


            



            // Extract COM port and Baud rate from shared configurations

            chart.MouseWheel += Chart_MouseWheel;
            chart.MouseMove += Chart_MouseMove;
            chart.Focus();
            // Configure chart area
            
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
            

            // Add chart to the panel
            panel.Controls.Clear(); // Clear any existing controls
            CreateCheckBoxes(checkBoxPanel, channels, channelCheckBoxes);
            
            foreach(CheckBox checkBox in channelCheckBoxes.Values)
            {
                checkBox.CheckedChanged += (s, evt) => chart.Refresh();
            }
            checkBoxPanel.Dock = DockStyle.Bottom; // Place below the chart
            checkBoxPanel.Height = 60; // Adjust height as needed

            

            // Add both chart and checkbox panel
            
            
            // Initialize and configure the SerialPort
            SerialPort serialPort = new SerialPort(ComPort, BaudRate)
            {
                DtrEnable = true, // Ensure data is received properly
                RtsEnable = true
            };
           
            string filePath = $"E:\\NCAIR\\Telemetry-Viewer\\Telemetry_demo\\test_logs\\{config.InputName}_log_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            csvWriter = new StreamWriter(filePath, true); // ✅ No 'new' modifier issue

            csvWriter.WriteLine("Timestamp,Ax,Ay,Az,Gx,Gy,Gz"); // CSV Header row
            csvWriter.Flush();


            Button DisconnectDevice = new Button
            {
                Text = "Disconnect",
                AutoSize = true
            };

            // Attach event handler and pass parameters
            DisconnectDevice.Click += (sender, e) => DisconnectDevice_Click(serialPort, csvWriter); panel.Controls.Clear(); 
            panel.Controls.Add(DisconnectDevice);
            panel.Controls.Add(checkBoxPanel); // Add checkboxes first
            panel.Controls.Add(chart); // Then add chart (so it doesn't overlap checkboxes)
            
            MessageBox.Show("Connected and logging started!", "Success");
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
                            ProcessCSVData(data, channels, channelSeries,panel,chart,isChartFrozen,allChannelData,channelCheckBoxes,csvWriter);                  }
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



        private void CreateCheckBoxes(Panel checkBoxPanel,List<string> channels, Dictionary<string, CheckBox> channelCheckBoxes)
        {
            checkBoxPanel.Controls.Clear();
            channelCheckBoxes.Clear();

            /*checkBoxPanel.AutoScroll = true;*/ // Enable scrolling if there are many checkboxes
            checkBoxPanel.Dock = DockStyle.Bottom; // Position the panel below the chart
            checkBoxPanel.Height = 50; // Set a reasonable height

            int yOffset = 5; // Vertical spacing

            foreach (string channel in channels)
            {
                CheckBox checkBox = new CheckBox
                {
                    Text = channel,
                    Tag = channel,
                    AutoSize = true,
                    Checked = true,
                    Location = new Point(5, yOffset) // Position each checkbox
                };
                yOffset += 25; // Space out checkboxes vertically

                channelCheckBoxes[channel] = checkBox;
                checkBoxPanel.Controls.Add(checkBox);
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


        
        public void ProcessCSVData(string data, List<string> channels, Dictionary<string, Series> channelSeries, Panel panel, Chart chart, bool isChartFrozen, Dictionary<string, List<DataPoint>> allChannelData, Dictionary<string, CheckBox> channelCheckBoxes,StreamWriter csvWriter)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            csvWriter.WriteLine($"{timestamp},{data}");
            csvWriter.Flush(); // Ensure immediate write
            string[] values = data.Split(',');
            if (values.Length != channels.Count) return;
            xCounter++;
            panel.Invoke(new Action(() =>
            {
                for (int i = 0; i < values.Length; i++)
                {
                    if (double.TryParse(values[i], out double parsedValue))
                    {
                        string channel = channels[i];

                        if (!channelSeries.ContainsKey(channel)) continue;
                        Series series = channelSeries[channel];

                        // Ensure only selected channels are plotted
                        if (!channelCheckBoxes[channel].Checked)
                        {

                            chart.Series[channel].Enabled = false;
                        }
                        else if (channelCheckBoxes[channel].Checked && !chart.Series[channel].Enabled)
                        {
                            chart.Series[channel].Enabled = true;
                        }

                        double xValue = series.Points.Count > 0 ? series.Points.Last().XValue + 1 : 0;
                        
                        Console.WriteLine(xValue);
                        Console.WriteLine(xValue);
                        // Store full data history for scrolling
                        if (!allChannelData.ContainsKey(channel))
                            allChannelData[channel] = new List<DataPoint>();

                        allChannelData[channel].Add(new DataPoint(xCounter, parsedValue));

                        if (!isChartFrozen) // Only update real-time when NOT frozen
                        {
                            if (series.Points.Count > 100) series.Points.RemoveAt(0);
                            series.Points.AddXY(xCounter, parsedValue);

                            // Auto-scroll X-axis unless frozen
                           
                                chart.ChartAreas[0].AxisX.Minimum = series.Points.First().XValue;
                                chart.ChartAreas[0].AxisX.Maximum = series.Points.Last().XValue;

                                
                            
                        }
                    }
                }
            }));
        }
        private void DisconnectDevice_Click(SerialPort port, StreamWriter writer)
        {
            if (port != null && port.IsOpen)
            {
                port.Close();
                port.Dispose();
            }

            writer?.Close();

            MessageBox.Show("Disconnected successfully!", "Info");
        }

    }
}
