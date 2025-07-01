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
using Newtonsoft.Json;

namespace Telemetry_demo
{
    /// <summary>
    /// UserControl for displaying and managing telemetry data visualization
    /// </summary>
    public partial class UserControl3 : UserControl
    {
        #region Constants
        private const int MaxBufferSize = 10000;    // Maximum number of points to keep per channel
        private const int VisibleWindowSize = 100;  // Number of points visible at a time
        private const int UpdateInterval = 100;     // Update chart every 100ms
        private const string LogDirectory = "logs"; // Directory for log files
        private const int CleanupInterval = 1000;   // Cleanup every 1 second
        private const int GarbageCollectionThreshold = 5000; // Trigger GC after this many points
        #endregion

        #region Private Fields
        private readonly List<InputConfig> inputConfigs;
        private StreamWriter csvWriter;
        private int xCounter = 0;
        private System.Windows.Forms.Timer updateTimer;
        private System.Windows.Forms.Timer cleanupTimer;
        private bool isLive = true;
        private double currentWindowStart = 0;
        private DateTime lastUpdateTime = DateTime.Now;
        private readonly Dictionary<string, StreamWriter> logWriters = new Dictionary<string, StreamWriter>();
        private readonly object chartLock = new object();
        private readonly Dictionary<string, List<DataPoint>> pendingUpdates = new Dictionary<string, List<DataPoint>>();
        private int totalPointsProcessed = 0;
        #endregion

        #region Constructor
        public UserControl3()
        {
            try
            {
                InitializeComponent();
                inputConfigs = ConfigManager.LoadConfigs();
                Console.WriteLine($"Loaded {inputConfigs.Count} input configurations.");
                InitializeLogDirectory();
            }
            catch (Exception ex)
            {
                HandleError("Failed to initialize Plotting Page", ex);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Creates a grid of panels for chart placement
        /// </summary>
        private void BtnAddGrid_Click(object sender, EventArgs e)
        {
            int rows = Convert.ToInt32(txtRows.Text);
            int columns = Convert.ToInt32(txtColumns.Text);

            var grid = CreateGridLayout(rows, columns);
            AddPanelsToGrid(grid, rows, columns);
            this.Controls.Add(grid);
            grid.BringToFront();
        }

        /// <summary>
        /// Disconnects the serial port and closes the CSV writer
        /// </summary>
        private void DisconnectDevice_Click(SerialPort port, StreamWriter writer)
        {
            if (port?.IsOpen == true)
            {
                port.Close();
                port.Dispose();
            }

            writer?.Close();
            MessageBox.Show("Disconnected successfully!", "Info");
        }

        public void LoadSavedConfigs()
        {
            try
            {
                ConfigManager.SetConfigPath();
                inputConfigs.Clear();
                inputConfigs.AddRange(ConfigManager.LoadConfigs());
                Console.WriteLine($"Loaded {inputConfigs.Count} input configurations (reloaded). ");
                // If you have a UI element to show configs, update it here.
            }
            catch (Exception ex)
            {
                HandleError("Failed to load saved configurations", ex);
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Creates a new chart and initializes its components
        /// </summary>
        private void AddChartToPanel(Panel panel, string inputName)
        {
            var config = ConfigManager.searchConfig(inputName);
            var channels = config.ChannelConfig.Channels;

            // Initialize chart components
            var chartComponents = InitializeChartComponents(config, channels);
            var chart = chartComponents.chart;
            var channelSeries = chartComponents.channelSeries;
            var allChannelData = chartComponents.allChannelData;
            var channelCheckBoxes = chartComponents.channelCheckBoxes;

            // Setup UI controls
            SetupChartControls(panel, chart, channelCheckBoxes, config);

            // Start data collection
            StartDataCollection(config, channels, channelSeries, panel, chart, allChannelData, channelCheckBoxes);

            // Initialize update timer
            InitializeUpdateTimer(chart, channelSeries, allChannelData, channelCheckBoxes);
        }

        /// <summary>
        /// Updates the chart with new data points
        /// </summary>
        private void UpdateChart(Chart chart, Dictionary<string, Series> channelSeries,
            Dictionary<string, Queue<DataPoint>> allChannelData, Dictionary<string, CheckBox> channelCheckBoxes)
        {
            if (chart.InvokeRequired)
            {
                try
                {
                    chart.Invoke(new Action(() => UpdateChart(chart, channelSeries, allChannelData, channelCheckBoxes)));
                }
                catch (Exception ex)
                {
                    HandleError("Failed to invoke chart update", ex);
                }
                return;
            }

            try
            {
                lock (chartLock)
                {
                    chart.SuspendLayout();
                    UpdateChartSeries(chart, channelSeries, allChannelData, channelCheckBoxes);
                    UpdateChartAxis(chart);
                }
            }
            catch (Exception ex)
            {
                HandleError("Failed to update chart", ex);
            }
            finally
            {
                try
                {
                    lock (chartLock)
                    {
                        chart.ResumeLayout();
                    }
                }
                catch (Exception ex)
                {
                    HandleError("Failed to resume chart layout", ex);
                }
            }
        }

        /// <summary>
        /// Processes incoming CSV data and updates the data buffers
        /// </summary>
        private void ProcessCSVData(string data, List<ChannelInfo> channels, Dictionary<string, Series> channelSeries,
            Panel panel, Chart chart, bool isChartFrozen, Dictionary<string, Queue<DataPoint>> allChannelData,
            Dictionary<string, CheckBox> channelCheckBoxes, StreamWriter csvWriter)
        {
            Console.WriteLine($"Received data: {data}");
            try
            {
                if (string.IsNullOrWhiteSpace(data))
                {
                    Console.WriteLine("Received empty data");
                    LogWarning("Received empty data");
                    return;
                }

                string[] values = data.Split(',');
                if (values.Length != channels.Count)
                {
                    Console.WriteLine($"Invalid data format. Expected {channels.Count} values, got {values.Length}");
                    LogWarning($"Invalid data format. Expected {channels.Count} values, got {values.Length}");
                    return;
                }
                
                double timestamp = xCounter++;
                Console.WriteLine($"Processing data: {data} at timestamp {timestamp}");
                ProcessChannelData(values, channels, timestamp, allChannelData);
                WriteToCSV(values, csvWriter);
            }
            catch (Exception ex)
            {
                HandleError("Failed to process CSV data", ex);
            }
        }

        /// <summary>
        /// Processes incoming binary data and updates the data buffers
        /// </summary>
        private void ProcessBinaryData(byte[] data, InputConfig config, List<ChannelInfo> channels, Dictionary<string, Series> channelSeries,
            Panel panel, Chart chart, bool isChartFrozen, Dictionary<string, Queue<DataPoint>> allChannelData,
            Dictionary<string, CheckBox> channelCheckBoxes, StreamWriter csvWriter)
        {
            try
            {
                if (data == null || data.Length == 0)
                {
                    LogWarning("Received empty binary data");
                    return;
                }

                // Check for sync byte
                if (config.SyncByte.HasValue && data[0] != config.SyncByte.Value)
                {
                    LogWarning($"Sync byte mismatch. Expected: 0x{config.SyncByte.Value:X2}, Got: 0x{data[0]:X2}");
                    return;
                }

                // Calculate expected packet size: sync byte (1) + number of channels * sizeof(int16)
                int expectedSize = 1 + (channels.Count * 2); // 2 bytes per int16_t
                if (data.Length != expectedSize)
                {
                    LogWarning($"Invalid binary packet size. Expected: {expectedSize}, Got: {data.Length}");
                    return;
                }

                // Parse int16 values starting after sync byte
                string[] values = new string[channels.Count];
                int dataIndex = 1; // Start after sync byte

                for (int i = 0; i < channels.Count; i++)
                {
                    if (dataIndex + 2 <= data.Length)
                    {
                        short value = BitConverter.ToInt16(data, dataIndex);
                        values[i] = value.ToString();
                        dataIndex += 2;
                    }
                    else
                    {
                        return;
                    }
                }

                double timestamp = xCounter++;
                ProcessChannelData(values, channels, timestamp, allChannelData);
                WriteToCSV(values, csvWriter);
            }
            catch (Exception ex)
            {
                HandleError("Failed to process binary data", ex);
            }
        }

        /// <summary>
        /// Creates checkboxes for channel selection
        /// </summary>
        private void CreateCheckBoxes(Panel checkBoxPanel, List<ChannelInfo> channels, Dictionary<string, CheckBox> channelCheckBoxes)
        {
            checkBoxPanel.Controls.Clear();
            channelCheckBoxes.Clear();
            checkBoxPanel.Dock = DockStyle.Bottom;
            checkBoxPanel.Height = Math.Min(channels.Count * 25 + 10, 150);

            var scrollPanel = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
            var flowPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                Padding = new Padding(5)
            };

            foreach (var channel in channels)
            {
                var checkBox = new CheckBox
                {
                    Text = channel.Name,
                    Checked = true,
                    AutoSize = true,
                    Margin = new Padding(3)
                };
                channelCheckBoxes[channel.Name] = checkBox;
                flowPanel.Controls.Add(checkBox);
            }

            scrollPanel.Controls.Add(flowPanel);
            checkBoxPanel.Controls.Add(scrollPanel);
        }
        #endregion

        #region Helper Methods
        private TableLayoutPanel CreateGridLayout(int rows, int columns)
        {
            var grid = new TableLayoutPanel
            {
                RowCount = rows,
                ColumnCount = columns,
                Dock = DockStyle.Fill
            };

            for (int i = 0; i < rows; i++)
                grid.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / rows));
            for (int i = 0; i < columns; i++)
                grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / columns));

            return grid;
        }

        private void AddPanelsToGrid(TableLayoutPanel grid, int rows, int columns)
        {
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    var panel = new Panel
                    {
                        Dock = DockStyle.Fill,
                        BorderStyle = BorderStyle.FixedSingle
                    };

                    var inputSelect = new ComboBox { Dock = DockStyle.Left };
                    foreach (var config in inputConfigs)
                        inputSelect.Items.Add(config.InputName);

                    var addChartButton = new Button
                    {
                        Text = "Add Chart",
                        Dock = DockStyle.Right
                    };
                    addChartButton.Click += (s, evt) => AddChartToPanel(panel, inputSelect.Text);

                    panel.Controls.Add(addChartButton);
                    panel.Controls.Add(inputSelect);
                    grid.Controls.Add(panel, col, row);
                }
            }
        }

        private (Chart chart, Dictionary<string, Series> channelSeries, Dictionary<string, Queue<DataPoint>> allChannelData, Dictionary<string, CheckBox> channelCheckBoxes) 
            InitializeChartComponents(InputConfig config, List<ChannelInfo> channels)
        {
            var chart = new Chart { Dock = DockStyle.Top };
            var chartArea = new ChartArea("MainArea");
            chart.ChartAreas.Add(chartArea);

            var channelSeries = new Dictionary<string, Series>();
            var allChannelData = new Dictionary<string, Queue<DataPoint>>();
            var channelCheckBoxes = new Dictionary<string, CheckBox>();

            foreach (var channel in channels)
            {
                var series = new Series(channel.Name)
                {
                    ChartType = SeriesChartType.Line,
                    BorderWidth = 2
                };
                channelSeries[channel.Name] = series;
                chart.Series.Add(series);
                allChannelData[channel.Name] = new Queue<DataPoint>();
            }

            return (chart, channelSeries, allChannelData, channelCheckBoxes);
        }

        private void SetupChartControls(Panel panel, Chart chart, Dictionary<string, CheckBox> channelCheckBoxes, InputConfig config)
        {
            var checkBoxPanel = new Panel();
            CreateCheckBoxes(checkBoxPanel, config.ChannelConfig.Channels, channelCheckBoxes);

            var disconnectButton = new Button { Text = "Disconnect", AutoSize = true };
            var liveToggleButton = new Button
            {
                Text = "Pause",
                Dock = DockStyle.Top,
                Height = 30,
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            liveToggleButton.Click += (s, e) => ToggleLiveMode(liveToggleButton, chart);

            panel.Controls.Clear();
            panel.Controls.Add(disconnectButton);
            panel.Controls.Add(checkBoxPanel);
            panel.Controls.Add(liveToggleButton);
            panel.Controls.Add(chart);
        }

        private void StartDataCollection(InputConfig config, List<ChannelInfo> channels, Dictionary<string, Series> channelSeries,
            Panel panel, Chart chart, Dictionary<string, Queue<DataPoint>> allChannelData, Dictionary<string, CheckBox> channelCheckBoxes)
        {
            string logFolder = GetLogFolderPath();
            string filePath = Path.Combine(logFolder, $"telemetry_logs/{config.InputName}/log_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            csvWriter = new StreamWriter(filePath, true);
            csvWriter.WriteLine("Timestamp,Ax,Ay,Az,Gx,Gy,Gz");
            csvWriter.Flush();
            Task.Run(() => CollectData(config, channels, channelSeries, panel, chart, allChannelData, channelCheckBoxes));
        }

        private void InitializeUpdateTimer(Chart chart, Dictionary<string, Series> channelSeries,
            Dictionary<string, Queue<DataPoint>> allChannelData, Dictionary<string, CheckBox> channelCheckBoxes)
        {
            try
            {
                // Initialize update timer
                updateTimer = new System.Windows.Forms.Timer();
                updateTimer.Interval = UpdateInterval;
                updateTimer.Tick += (s, e) => UpdateChart(chart, channelSeries, allChannelData, channelCheckBoxes);
                updateTimer.Start();

                // Initialize cleanup timer
                cleanupTimer = new System.Windows.Forms.Timer();
                cleanupTimer.Interval = CleanupInterval;
                cleanupTimer.Tick += (s, e) => PerformCleanup(chart, channelSeries, allChannelData);
                cleanupTimer.Start();
            }
            catch (Exception ex)
            {
                HandleError("Failed to initialize timers", ex);
            }
        }

        private void PerformCleanup(Chart chart, Dictionary<string, Series> channelSeries,
            Dictionary<string, Queue<DataPoint>> allChannelData)
        {
            try
            {
                lock (chartLock)
                {
                    // Clean up old data points
                    foreach (var channel in channelSeries.Keys)
                    {
                        var queue = allChannelData[channel];
                        lock (queue)
                        {
                            while (queue.Count > MaxBufferSize)
                            {
                                queue.Dequeue();
                            }
                        }
                    }

                    // Force garbage collection if we've processed many points
                    if (totalPointsProcessed > GarbageCollectionThreshold)
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        totalPointsProcessed = 0;
                    }

                    // Clear any pending updates
                    pendingUpdates.Clear();
                }
            }
            catch (Exception ex)
            {
                HandleError("Failed to perform cleanup", ex);
            }
        }

        private void UpdateChartSeries(Chart chart, Dictionary<string, Series> channelSeries,
            Dictionary<string, Queue<DataPoint>> allChannelData, Dictionary<string, CheckBox> channelCheckBoxes)
        {
            try
            {
                lock (chartLock)
                {
                    // Create a copy of the channels to avoid modification during enumeration
                    var channels = channelSeries.Keys.ToList();
                    
                    foreach (var channel in channels)
                    {
                        if (!channelCheckBoxes[channel].Checked) continue;

                        var series = channelSeries[channel];
                        var dataQueue = allChannelData[channel];
                        
                        // Create a copy of the data points safely
                        var points = new List<DataPoint>();
                        lock (dataQueue)
                        {
                            // Only take the most recent points up to MaxBufferSize
                            int count = Math.Min(dataQueue.Count, MaxBufferSize);
                            points.AddRange(dataQueue.Skip(dataQueue.Count - count));
                        }
                        
                        // Update series points
                        series.Points.Clear();
                        foreach (var point in points)
                        {
                            series.Points.AddXY(point.XValue, point.YValues[0]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HandleError("Failed to update chart series", ex);
            }
        }

        private void UpdateChartAxis(Chart chart)
        {
            try
            {
                if (isLive && chart.Series.Count > 0 && chart.Series[0].Points.Count > 0)
                {
                    var chartArea = chart.ChartAreas[0];
                    double maxX = chart.Series[0].Points.Last().XValue;
                    chartArea.AxisX.Minimum = Math.Max(0, maxX - VisibleWindowSize);
                    chartArea.AxisX.Maximum = maxX;
                }
            }
            catch (Exception ex)
            {
                HandleError("Failed to update chart axis", ex);
            }
        }

        private void ProcessChannelData(string[] values, List<ChannelInfo> channels, double timestamp, Dictionary<string, Queue<DataPoint>> allChannelData)
        {
            try
            {
                for (int i = 0; i < channels.Count; i++)
                {
                    string channel = channels[i].Name;
                    if (double.TryParse(values[i], out double value))
                    {
                        var dataPoint = new DataPoint(timestamp, value);
                        var queue = allChannelData[channel];
                        lock (queue)
                        {
                            queue.Enqueue(dataPoint);
                            while (queue.Count > MaxBufferSize)
                            {
                                queue.Dequeue();
                            }
                        }
                        totalPointsProcessed++;
                    }
                    else
                    {
                        LogWarning($"Failed to parse value '{values[i]}' for channel {channel}");
                    }
                }
            }
            catch (Exception ex)
            {
                HandleError("Failed to process channel data", ex);
            }
        }

        private void WriteToCSV(string[] values, StreamWriter csvWriter)
        {
            try
            {
                if (csvWriter == null)
                {
                    LogWarning("CSV writer is null");
                    return;
                }

                csvWriter.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff},{string.Join(",", values)}");
                csvWriter.Flush();
            }
            catch (Exception ex)
            {
                HandleError("Failed to write to CSV", ex);
            }
        }

        private void ToggleLiveMode(Button btnLiveToggle, Chart chart)
        {
            isLive = !isLive;
            btnLiveToggle.Text = isLive ? "Pause" : "Go Live";
            if (isLive)
            {
                foreach (var series in chart.Series)
                {
                    if (series.Points.Count > 0)
                    {
                        chart.ChartAreas[0].AxisX.Minimum = series.Points.First().XValue;
                        chart.ChartAreas[0].AxisX.Maximum = series.Points.Last().XValue;
                    }
                }
            }
        }

        private void CollectData(InputConfig config, List<ChannelInfo> channels, Dictionary<string, Series> channelSeries,
            Panel panel, Chart chart, Dictionary<string, Queue<DataPoint>> allChannelData, Dictionary<string, CheckBox> channelCheckBoxes)
        {
            Console.WriteLine($"Starting data collection for {config.InputName} on {config.ConnectionType}...");
            try
            {
               
                if (config.ConnectionType == "UART")
                {
                    CollectUARTData(config, channels, channelSeries, panel, chart, allChannelData, channelCheckBoxes);
                }
                else if (config.ConnectionType == "UDP")
                {
                    
                    CollectUDPData(config, channels, channelSeries, panel, chart, allChannelData, channelCheckBoxes);
                }
                else
                {
                    throw new InvalidOperationException($"Unsupported connection type: {config.ConnectionType}");
                }
            }
            catch (Exception ex)
            {
                HandleError($"Failed to collect data for {config.InputName}", ex);
            }
        }

        private void CollectUARTData(InputConfig config, List<ChannelInfo> channels, Dictionary<string, Series> channelSeries,
            Panel panel, Chart chart, Dictionary<string, Queue<DataPoint>> allChannelData, Dictionary<string, CheckBox> channelCheckBoxes)
        {
            using (var serialPort = new SerialPort(config.Port, config.BaudRate) { DtrEnable = true, RtsEnable = true })
            {
                Console.WriteLine($"Opening serial port {config.Port} at {config.BaudRate} baud...");
                try
                {
                    serialPort.Open();
                    int frameSize = 1 + (channels.Count * 2); // 1 sync + N*2 bytes
                    List<byte> buffer = new List<byte>();
                    byte syncByte = config.SyncByte ?? 0xAA;
                    while (serialPort.IsOpen)
                    {
                        try
                        {
                            int bytesToRead = serialPort.BytesToRead;
                            if (bytesToRead > 0)
                            {
                                byte[] temp = new byte[bytesToRead];
                                int read = serialPort.Read(temp, 0, bytesToRead);
                                buffer.AddRange(temp.Take(read));
                            }

                            // Try to extract frames
                            while (buffer.Count >= frameSize)
                            {
                                // Find sync byte
                                int syncIndex = buffer.IndexOf(syncByte);
                                if (syncIndex < 0)
                                {
                                    // No sync byte, discard all
                                    buffer.Clear();
                                    break;
                                }
                                if (buffer.Count - syncIndex < frameSize)
                                {
                                    // Not enough data for a full frame
                                    break;
                                }
                                // Extract frame
                                byte[] frame = buffer.Skip(syncIndex).Take(frameSize).ToArray();
                                buffer.RemoveRange(0, syncIndex + frameSize);
                                // Process frame
                                ProcessBinaryData(frame, config, channels, channelSeries, panel, chart, false, allChannelData, channelCheckBoxes, csvWriter);
                            }
                        }
                        catch (TimeoutException)
                        {
                            // Ignore timeout exceptions as they're expected
                            continue;
                        }
                        catch (Exception ex)
                        {
                            LogWarning($"Error reading from {config.Port}: {ex.Message}");
                        }
                        System.Threading.Thread.Sleep(1); // Prevent tight loop
                    }
                }
                catch (Exception ex)
                {
                    HandleError($"Failed to open serial port {config.Port}", ex);
                }
            }
        }

        private void CollectUDPData(InputConfig config, List<ChannelInfo> channels, Dictionary<string, Series> channelSeries,
            Panel panel, Chart chart, Dictionary<string, Queue<DataPoint>> allChannelData, Dictionary<string, CheckBox> channelCheckBoxes)
        {
            if (!int.TryParse(config.Port, out int udpPort))
            {
                throw new InvalidOperationException($"Invalid UDP port: {config.Port}");
            }
            Console.WriteLine($"Starting UDP listener on port {udpPort}...");
            using (var udpClient = new UdpClient())
            {
                try
                {
                    udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, udpPort));

                    LogInfo($"Listening for UDP packets on port {udpPort}...");

                    while (true)
                    {
                        try
                        {
                            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                            byte[] data = udpClient.Receive(ref remoteEP);
                            
                            
                            
                            // Process data based on input mode
                            if (config.InputMode == "Binary Mode")
                            {
                                ProcessBinaryData(data, config, channels, channelSeries, panel, chart, false, allChannelData, channelCheckBoxes, csvWriter);
                            }
                            else
                            {
                                // CSV Mode - convert bytes to string
                                string message = Encoding.ASCII.GetString(data);
                                ProcessCSVData(message, channels, channelSeries, panel, chart, false, allChannelData, channelCheckBoxes, csvWriter);
                            }
                        }
                        catch (ObjectDisposedException)
                        {
                            break;
                        }
                        catch (Exception ex)
                        {
                            LogWarning($"UDP Receive error: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    HandleError($"Failed to initialize UDP client on port {udpPort}", ex);
                }
            }
        }

        private string GetLogFolderPath()
        {
            try
            {
                string settingsPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "TelemetryViewer",
                    "settings.json"
                );
                if (File.Exists(settingsPath))
                {
                    var json = File.ReadAllText(settingsPath);
                    dynamic settings = JsonConvert.DeserializeObject(json);
                    string logPath = settings?.logPath;
                    if (!string.IsNullOrWhiteSpace(logPath))
                        return logPath;
                }
            }
            catch { }
            // fallback to default if not set
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
        }
        #endregion

        #region Initialization Methods
        private void InitializeLogDirectory()
        {
            try
            {
                if (!Directory.Exists(LogDirectory))
                {
                    Directory.CreateDirectory(LogDirectory);
                }
            }
            catch (Exception ex)
            {
                HandleError("Failed to initialize log directory", ex);
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

        private void LogError(string message, Exception ex)
        {
            try
            {
                string logPath = Path.Combine(LogDirectory, $"error_{DateTime.Now:yyyyMMdd}.log");
                string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ERROR: {message}\nStack Trace: {ex.StackTrace}\n\n";
                File.AppendAllText(logPath, logMessage);
            }
            catch
            {
                // If logging fails, we can't do much about it
            }
        }

        private void LogWarning(string message)
        {
            try
            {
                string logPath = Path.Combine(LogDirectory, $"warning_{DateTime.Now:yyyyMMdd}.log");
                string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] WARNING: {message}\n";
                File.AppendAllText(logPath, logMessage);
            }
            catch
            {
                // If logging fails, we can't do much about it
            }
        }

        private void LogInfo(string message)
        {
            try
            {
                string logPath = Path.Combine(LogDirectory, $"info_{DateTime.Now:yyyyMMdd}.log");
                string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] INFO: {message}\n";
                File.AppendAllText(logPath, logMessage);
            }
            catch
            {
                // If logging fails, we can't do much about it
            }
        }
        #endregion

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (this.Visible)
            {
                Console.WriteLine("HURRAHHHHHHHHHHH");
                LoadSavedConfigs();
            }
        }
    }
}
