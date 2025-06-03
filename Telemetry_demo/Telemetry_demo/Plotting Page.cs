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
using System.Net.Sockets;
using System.Net;


namespace Telemetry_demo
{
    public partial class UserControl3 : UserControl
    {
        /*private SerialPort serialPort;*/
        
        List<InputConfig> inputConfigs=ConfigManager.LoadConfigs();
        private StreamWriter csvWriter;
        string filePath = "test.csv";
        private int xCounter = 0;
        private const int MaxBufferSize = 10000; // Maximum number of points to keep per channel
        private const int VisibleWindowSize = 100; // Number of points visible at a time
        private bool isLive = true; // <-- Add this as a field
        private double currentWindowStart = 0; // Track the left edge of the visible window
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
            InputConfig config = ConfigManager.searchConfig(InputName);
            int BaudRate = config.BaudRate;
            var channels = config.ChannelConfig.Channels;
            string Port = config.Port;

            // Remove local declaration of isLive here
            Button btnLiveToggle = null;

            //######################################## CHART UTILITIES ###########################################//

            //***************************************Declarations**********************************************//
            Dictionary<string, CheckBox> channelCheckBoxes = new Dictionary<string, CheckBox>();
            Dictionary<string, Queue<DataPoint>> allChannelData = new Dictionary<string, Queue<DataPoint>>(); // Use Queue for fixed-size buffer
            Panel checkBoxPanel = new Panel();
            double scrollOffset = 0;
            bool isChartFrozen = false;
            ToolTip toolTip = new ToolTip();
            Dictionary<string, Series> channelSeries = new Dictionary<string, Series>();
            string filePath = $"C:\\LAUKIK\\Telemetry\\Telemetry-Viewer\\Telemetry_demo\\test_logs\\{config.InputName}_log_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            csvWriter = new StreamWriter(filePath, true);

            csvWriter.WriteLine("Timestamp,Ax,Ay,Az,Gx,Gy,Gz");
            csvWriter.Flush();










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
                double minX = double.MaxValue;
                double maxX = double.MinValue;
                foreach (var series in chart_s.Series)
                {
                    if (series.Points.Count > 0)
                    {
                        minX = Math.Min(minX, series.Points.First().XValue);
                        maxX = Math.Max(maxX, series.Points.Last().XValue);
                    }
                }
                double windowSize = VisibleWindowSize;
                if (e.Delta > 0) // Scroll Up (older data)
                {
                    if (isLive)
                    {
                        isLive = false;
                        btnLiveToggle.Text = "Go Live";
                    }
                    isChartFrozen = true;
                    // Move window left
                    currentWindowStart -= 10; // Scroll step
                    if (currentWindowStart < minX) currentWindowStart = minX;
                }
                else if (e.Delta < 0) // Scroll Down (newer data)
                {
                    if (isChartFrozen)
                    {
                        // Move window right
                        currentWindowStart += 10; // Scroll step
                        if (currentWindowStart > maxX - (windowSize - 1))
                            currentWindowStart = Math.Max(minX, maxX - (windowSize - 1));
                    }
                }
                // Always update the window in paused mode
                if (!isLive && minX != double.MaxValue && maxX != double.MinValue)
                {
                    chartArea_s.AxisX.Minimum = currentWindowStart;
                    chartArea_s.AxisX.Maximum = currentWindowStart + windowSize - 1;
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
            foreach (string channel in channels)
            {
                Series series = new Series(channel)
                {
                    ChartType = SeriesChartType.Line,
                    BorderWidth = 2
                };
                channelSeries[channel] = series;
                chart.Series.Add(series);
                allChannelData[channel] = new Queue<DataPoint>(); // Initialize buffer
            }
            

            // Add chart to the panel
            panel.Controls.Clear();
            CreateCheckBoxes(checkBoxPanel, channels, channelCheckBoxes);
            
            foreach(CheckBox checkBox in channelCheckBoxes.Values)
            {
                checkBox.CheckedChanged += (s, evt) => chart.Refresh();
            }
            checkBoxPanel.Dock = DockStyle.Bottom; // Place below the chart
            checkBoxPanel.Height = Math.Min(channels.Count * 25 + 10, 150); // Limit height to 150px or less


            Button DisconnectDevice = new Button
            {
                Text = "Disconnect",
                AutoSize = true
            };

            // Add persistent toggle button
            btnLiveToggle = new Button
            {
                Text = "Pause",
                Dock = DockStyle.Top,
                Height = 30,
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLiveToggle.FlatAppearance.BorderSize = 0;
            btnLiveToggle.Click += (s, e) =>
            {
                isLive = !isLive;
                btnLiveToggle.Text = isLive ? "Pause" : "Go Live";
                if (isLive)
                {
                    // Jump to latest data
                    foreach (var series in channelSeries.Values)
                    {
                        if (series.Points.Count > 0)
                        {
                            chartArea.AxisX.Minimum = series.Points.First().XValue;
                            chartArea.AxisX.Maximum = series.Points.Last().XValue;
                        }
                    }
                }
            };

            // Add both chart and checkbox panel


            // Initialize and configure the SerialPort
            panel.Controls.Clear();
            panel.Controls.Add(DisconnectDevice);
            panel.Controls.Add(checkBoxPanel); // Add checkboxes first
            panel.Controls.Add(btnLiveToggle);
            panel.Controls.Add(chart); // Then add chart (so it doesn't overlap checkboxes)
            
            MessageBox.Show("Connected and logging started!", "Success");
            try
            {
           
                






                // Attach event handler and pass parameters
                

                // Run data reading in a separate task
                Task.Run(() =>
                {
                if (config.ConnectionType == "UART")
                    {
                        SerialPort serialPort = new SerialPort(Port, BaudRate)
                        {
                            DtrEnable = true, // Ensure data is received properly
                            RtsEnable = true
                        };
                        DisconnectDevice.Click += (sender, e) => DisconnectDevice_Click(serialPort, csvWriter); 
                        serialPort.Open();
                        while (serialPort.IsOpen)
                        {
                            try
                            {
                                string data = serialPort.ReadLine(); // Read incoming data
                                ProcessCSVData(data, channels, channelSeries, panel, chart, isChartFrozen, allChannelData, channelCheckBoxes, csvWriter);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error reading from {Port}: {ex.Message}");
                            }
                        }
                    }
                    else if (config.ConnectionType == "UDP")
                    {
                            int udpPort = int.Parse(Port);
                            UdpClient udpClient = new UdpClient();
                            udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                            udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, udpPort));

                            DisconnectDevice.Click += (sender, e) =>
                            {
                                udpClient.Close(); // Stop receiving
                                panel.Controls.Clear();
                            };

                            Console.WriteLine($"Listening for UDP packets on port {udpPort}...");

                            while (true)
                            {
                                try
                                {
                                    IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                                    byte[] data = udpClient.Receive(ref remoteEP);
                                    string message = Encoding.ASCII.GetString(data);
                                    //Console.WriteLine(message);
                                // Make sure this runs on the UI thread if it updates UI controls
                                    ProcessCSVData(message, channels, channelSeries, panel, chart, isChartFrozen, allChannelData, channelCheckBoxes, csvWriter);
                                //panel.Invoke((MethodInvoker)delegate
                                //{
                                //    ProcessCSVData(message, channels, channelSeries, panel, chart, isChartFrozen, allChannelData, channelCheckBoxes, csvWriter);
                                //});
                            }
                                catch (ObjectDisposedException)
                                {
                                    // Thrown when udpClient is closed during Receive
                                    break;
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"UDP Receive error: {ex.Message}");
                                }
                            }
                        }
                    
                    
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open {Port}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void CreateCheckBoxes(Panel checkBoxPanel, List<string> channels, Dictionary<string, CheckBox> channelCheckBoxes)
        {
            checkBoxPanel.Controls.Clear();
            channelCheckBoxes.Clear();
            checkBoxPanel.Dock = DockStyle.Bottom;
            checkBoxPanel.Height = Math.Min(channels.Count * 25 + 10, 150); // Limit height to 150px or less

            // Create a scrollable panel for checkboxes
            Panel scrollPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };

            // Create a flow layout panel inside the scroll panel for better checkbox organization
            FlowLayoutPanel flowPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top, // Important: Top, not Fill
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                Padding = new Padding(5)
            };

            // Add checkboxes to the flow panel
            foreach (string channel in channels)
            {
                CheckBox checkBox = new CheckBox
                {
                    Text = channel,
                    Checked = true,
                    AutoSize = true,
                    Margin = new Padding(3)
                };
                channelCheckBoxes[channel] = checkBox;
                flowPanel.Controls.Add(checkBox);
            }

            // Add the flow panel to the scroll panel
            scrollPanel.Controls.Add(flowPanel);

            // Add the scroll panel to the main panel
            checkBoxPanel.Controls.Add(scrollPanel);
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


        
        public void ProcessCSVData(string data, List<string> channels, Dictionary<string, Series> channelSeries, Panel panel, Chart chart, bool isChartFrozen, Dictionary<string, Queue<DataPoint>> allChannelData, Dictionary<string, CheckBox> channelCheckBoxes, StreamWriter csvWriter)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            csvWriter.WriteLine($"{timestamp},{data}");
            csvWriter.Flush();
            string[] values = data.Split(',');
            if (values.Length != channels.Count)
            {
                Console.WriteLine("Your data and input config(channel length) don't match");
                return;
            }
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
                        // Store full data history for scrolling (fixed-size buffer)
                        if (!allChannelData.ContainsKey(channel))
                            allChannelData[channel] = new Queue<DataPoint>();
                        var buffer = allChannelData[channel];
                        buffer.Enqueue(new DataPoint(xCounter, parsedValue));
                        while (buffer.Count > MaxBufferSize)
                        {
                            buffer.Dequeue(); // Remove oldest
                        }
                        // Update chart points from buffer
                        series.Points.Clear();
                        foreach (var pt in buffer)
                        {
                            series.Points.AddXY(pt.XValue, pt.YValues[0]);
                        }
                        // Auto-scroll X-axis unless paused
                        if (isLive)
                        {
                            if (series.Points.Count > 0)
                            {
                                double latestX = series.Points.Last().XValue;
                                double minX = Math.Max(series.Points.First().XValue, latestX - (VisibleWindowSize - 1));
                                currentWindowStart = minX;
                                chart.ChartAreas[0].AxisX.Minimum = currentWindowStart;
                                chart.ChartAreas[0].AxisX.Maximum = latestX;
                            }
                        }
                        else // Paused mode: always use currentWindowStart
                        {
                            if (series.Points.Count > 0)
                            {
                                double minX = series.Points.First().XValue;
                                double maxX = series.Points.Last().XValue;
                                // Clamp currentWindowStart to buffer
                                if (currentWindowStart < minX) currentWindowStart = minX;
                                if (currentWindowStart > maxX - (VisibleWindowSize - 1)) currentWindowStart = Math.Max(minX, maxX - (VisibleWindowSize - 1));
                                chart.ChartAreas[0].AxisX.Minimum = currentWindowStart;
                                chart.ChartAreas[0].AxisX.Maximum = currentWindowStart + VisibleWindowSize - 1;
                            }
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
