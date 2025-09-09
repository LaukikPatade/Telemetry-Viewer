using System;
using System.Collections.ObjectModel;
using System.Timers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Avalonia;
using LiveChartsCore.Defaults;
using Avalonia.Threading;
using System.Text.Json;
using System.IO;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using System.Collections.Concurrent;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Telemetry_demo_Avalonia.Utils;

namespace Telemetry_demo_Avalonia.ViewModels;

public partial class TileViewModel : ViewModelBase
{


    // Reference to parent ViewModel for panel management
    public PlottingPageViewModel? ParentViewModel { get; set; }
    
    [ObservableProperty]
    private InputConfig? config;

    [ObservableProperty]
    private bool isStarted = false;

    [ObservableProperty]
    private bool isPaused = false;

    [ObservableProperty]
    private bool isActive = false;

    [ObservableProperty]
    private int windowSize = 1000;

    [ObservableProperty]
    private bool isManualXAxisRange = false;

    public ObservableCollection<ChannelPlot> Channels { get; } = new();
    public ObservableCollection<ISeries> Series { get; } = new();
    public Axis[] XAxes { get; private set; } = { new Axis { Name = "Sample", MinLimit = 0.0, MaxLimit = 1000.0 } };
    public Axis[] YAxes { get; private set; } = { new Axis { Name = "Value", MinLimit = -10000.0, MaxLimit = 10000.0 } };
    public ZoomAndPanMode ChartZoomMode { get; } = ZoomAndPanMode.X;

    // Commands
    public IRelayCommand StartCommand { get; }
    public IRelayCommand StopCommand { get; }
    public IRelayCommand PauseCommand { get; }
    public IRelayCommand ResumeCommand { get; }
    public IRelayCommand RollBackCommand { get; }
    public IRelayCommand RollForwardCommand { get; }

    // Data collection fields
    private SerialPort? _serialPort;
    private CancellationTokenSource? _serialCts;
    private UdpClient? _udpClient;
    private CancellationTokenSource? _udpCts;
    private IPEndPoint? _udpEndPoint;

    private readonly int UiUpdateIntervalMs = 100;
    private DateTime _lastUiUpdate = DateTime.MinValue;
    private List<List<ObservablePoint>> _pendingPoints = new();

    // Logging fields
    private BlockingCollection<string> _logQueue = new();
    private Task? _logTask;
    private string? _logFilePath;
    private CancellationTokenSource? _logCts;

    // History and scrolling
    private bool _isLiveView = true;
    private int _scrollStartIndex = 0;
    private List<double[]> _historyBuffer = new();
    private int _scrollPosition = 0;
    private int _maxScrollPosition = 0;
    private int _lastDisplayedSampleIndex = -1;
    private bool _resumeClicked = false;
    private int _pausedXMin = -1;
    private int _pausedXMax = -1;
    private int _rollbackStep = 50;
    private bool _hasReceivedData = false;
    private bool _hasReceivedAnyData = false; // Track if any data has been received at all
    private DateTime _lastDataReceivedTime = DateTime.MinValue;
    
    // Historical data navigation
    private int _currentHistoryWindowStart = 0; // Current window start index in log file
    private int _totalSamplesInLog = 0; // Total number of samples in the log file

    public bool IsLiveView
    {
        get => _isLiveView;
        set => SetProperty(ref _isLiveView, value);
    }

    public int ScrollPosition
    {
        get => _scrollPosition;
        set
        {
            if (SetProperty(ref _scrollPosition, value))
            {
                int newStart = value * WindowSize;
                LoadHistoryWindow(newStart);
            }
        }
    }

    public int MaxScrollPosition
    {
        get => _maxScrollPosition;
        set => SetProperty(ref _maxScrollPosition, value);
    }

    public IRelayCommand JumpToLiveCommand { get; }

    public TileViewModel()
    {
        StartCommand = new RelayCommand(Start, () => Config != null && !IsStarted);
        StopCommand = new RelayCommand(Stop, () => IsStarted);
        PauseCommand = new RelayCommand(() => {
            IsPaused = true;
            // Clear pending points to prevent backlog when resuming
            _pendingPoints.ForEach(list => list.Clear());
            if (XAxes.Length > 0 && XAxes[0].MinLimit.HasValue && XAxes[0].MaxLimit.HasValue)
            {
                _pausedXMin = (int)XAxes[0].MinLimit!.Value;
                _pausedXMax = (int)XAxes[0].MaxLimit!.Value;
                
                // Set the current history window start to the current X-axis position
                _currentHistoryWindowStart = _pausedXMin;
                Console.WriteLine($"Pause: Set current history window start to: {_currentHistoryWindowStart}");
            }
        }, () => !IsPaused);
        ResumeCommand = new RelayCommand(() => {
            IsPaused = false;
            _resumeClicked = true;
            // Reset UI update timer to prevent backlog
            _lastUiUpdate = DateTime.UtcNow;
            // Clear any pending points to prevent backlog
            _pendingPoints.ForEach(list => list.Clear());
        }, () => IsPaused);
        RollBackCommand = new RelayCommand(RollBack, () => IsPaused);
        RollForwardCommand = new RelayCommand(RollForward, () => IsPaused);
        JumpToLiveCommand = new RelayCommand(JumpToLive, () => !IsLiveView);

        PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(IsPaused))
            {
                (PauseCommand as RelayCommand)?.NotifyCanExecuteChanged();
                (ResumeCommand as RelayCommand)?.NotifyCanExecuteChanged();
                (RollBackCommand as RelayCommand)?.NotifyCanExecuteChanged();
                (RollForwardCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
            if (e.PropertyName == nameof(IsStarted))
            {
                (StartCommand as RelayCommand)?.NotifyCanExecuteChanged();
                (StopCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        };
    }

    partial void OnConfigChanged(InputConfig? value)
    {
        Console.WriteLine($"Config changed to: {value?.ConnectionName ?? "null"}");
        
        if (value != null)
        {
            Stop();
            IsPaused = true;
            
            // Initialize the Series for the chart
            Dispatcher.UIThread.Post(() =>
            {
                Channels.Clear();
                Series.Clear();
                foreach (var ch in value.Channels)
                {
                    var channel = new ChannelPlot { Name = ch.Name ?? "Channel", IsSelected = true };
                    Channels.Add(channel);
                    var lineSeries = new LineSeries<ObservablePoint>
                    {
                        Name = channel.Name,
                        Values = channel.Values,
                        GeometrySize = 0,
                        AnimationsSpeed = TimeSpan.Zero
                    };
                    Series.Add(lineSeries);
                    
                    // Subscribe to visibility changes
                    channel.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == nameof(ChannelPlot.IsVisible))
                        {
                            lineSeries.IsVisible = channel.IsVisible;
                        }
                    };
                }
            });
            
            // Don't start acquisition immediately - wait for Start() to be called
            // This ensures the chart is visible before data starts flowing
        }
    }
    
    partial void OnWindowSizeChanged(int value)
    {
        // Update X-axis limits when window size changes
        if (XAxes.Length > 0)
        {
            XAxes[0].MaxLimit = value;
            OnPropertyChanged(nameof(XAxes));
        }
    }
    
    // Method to reset manual X-axis range flag (for when user wants to go back to auto)
    public void ResetManualXAxisRange()
    {
        IsManualXAxisRange = false;
    }
    
    // Helper method to update X-axis limits - keeps window moving even in manual mode
    private void UpdateXAxisLimitsIfNotManual(int minLimit, int maxLimit)
    {
        if (XAxes.Length > 0)
        {
            if (!IsManualXAxisRange)
            {
                // Auto mode: use the calculated limits
                XAxes[0].MinLimit = minLimit;
                XAxes[0].MaxLimit = maxLimit;
            }
            else
            {
                // Manual mode: keep the window moving but maintain the user's specified range
                int windowSize = WindowSize;
                int currentMax = maxLimit; // This is the latest sample index
                int newMin = Math.Max(0, currentMax - windowSize + 1);
                XAxes[0].MinLimit = newMin;
                XAxes[0].MaxLimit = currentMax;
            }
        }
    }
    
    // Method to force set X-axis limits (for manual mode)
    public void ForceSetXAxisLimits(int minLimit, int maxLimit)
    {
        if (XAxes.Length > 0)
        {
            XAxes[0].MinLimit = minLimit;
            XAxes[0].MaxLimit = maxLimit;
            // Ensure the window size is maintained
            if (maxLimit - minLimit + 1 != WindowSize)
            {
                WindowSize = maxLimit - minLimit + 1;
            }
            
            // Force UI update by triggering property change
            OnPropertyChanged(nameof(XAxes));
        }
    }
    
    // Method to set Y-axis limits (for manual mode)
    public void SetYAxisLimits(double minLimit, double maxLimit)
    {
        Console.WriteLine($"SetYAxisLimits called: min={minLimit}, max={maxLimit}");
        
        if (YAxes.Length > 0)
        {
            // Ensure we're on the UI thread
            Dispatcher.UIThread.Post(() =>
            {
                YAxes[0].MinLimit = minLimit;
                YAxes[0].MaxLimit = maxLimit;
                
                Console.WriteLine($"Y-axis limits set to: {YAxes[0].MinLimit} - {YAxes[0].MaxLimit}");
                
                // Force UI update by triggering property change
                OnPropertyChanged(nameof(YAxes));
                
                // Also trigger Series change to force chart refresh
                OnPropertyChanged(nameof(Series));
            });
        }
    }
    
    // Method to reset Y-axis limits to automatic mode
    public void ResetYAxisLimits()
    {
        Console.WriteLine("ResetYAxisLimits called");
        
        if (YAxes.Length > 0)
        {
            // Ensure we're on the UI thread
            Dispatcher.UIThread.Post(() =>
            {
                YAxes[0].MinLimit = null;
                YAxes[0].MaxLimit = null;
                
                Console.WriteLine("Y-axis limits reset to automatic mode");
                
                // Force UI update by triggering property change
                OnPropertyChanged(nameof(YAxes));
                
                // Also trigger Series change to force chart refresh
                OnPropertyChanged(nameof(Series));
            });
        }
    }

    private async void Start()
    {
        Console.WriteLine($"Start() called for config: {Config?.ConnectionName ?? "null"}");
        
        if (Config == null)
        {
            AppLogger.LogWarning("No configuration selected for telemetry", showMessageBox: false);
            return;
        }

        try
        {
            AppLogger.LogInfo($"Starting telemetry for config: {Config.ConnectionName}", showMessageBox: false);
            
            IsStarted = true;
            IsPaused = false;
            
            // Add to active config panels
            ParentViewModel?.AddActiveConfigPanel(Config, this);
            
            // Try to start live data acquisition first
            bool liveAcquisitionStarted = false;
            
            if (Config.ConnectionType == "UART")
            {
                liveAcquisitionStarted = TryStartUartAcquisition(Config);
                if (liveAcquisitionStarted)
                {
                    // Initialize _pendingPoints for the number of channels
                    int channelCount = Config.Channels?.Count ?? 0;
                    _pendingPoints = Enumerable.Range(0, channelCount).Select(_ => new List<ObservablePoint>()).ToList();
                    Console.WriteLine($"Initialized _pendingPoints for {channelCount} channels");
                    
                    // Reset data reception flags for new connection
                    _hasReceivedAnyData = false;
                    _lastDataReceivedTime = DateTime.MinValue;
                }
            }
            else if (Config.ConnectionType == "UDP")
            {
                Console.WriteLine("Attempting UDP acquisition...");
                liveAcquisitionStarted = TryStartUdpAcquisition(Config);
                Console.WriteLine($"UDP acquisition result: {liveAcquisitionStarted}");
                
                // For UDP, we'll show the dialog after a delay if no data is received
                if (liveAcquisitionStarted)
                {
                    // Initialize _pendingPoints for the number of channels
                    int channelCount = Config.Channels?.Count ?? 0;
                    _pendingPoints = Enumerable.Range(0, channelCount).Select(_ => new List<ObservablePoint>()).ToList();
                    Console.WriteLine($"Initialized _pendingPoints for {channelCount} channels");
                    
                    // Reset data reception flags for new connection
                    _hasReceivedAnyData = false;
                    _lastDataReceivedTime = DateTime.MinValue;
                    
                    Console.WriteLine("Starting data reception check...");
                    // Start a background task to check for data reception
                    _ = Task.Run(async () => await CheckForDataReception());
                }
            }
            
            // If live acquisition failed immediately, show dialog to user
            if (!liveAcquisitionStarted)
            {
                AppLogger.LogWarning($"Live acquisition failed for config: {Config.ConnectionName}", showMessageBox: false);
                await ShowConnectionFailedDialog();
            }
            else
            {
                AppLogger.LogSuccess($"Telemetry started successfully for config: {Config.ConnectionName}", showMessageBox: false);
            }
        }
        catch (Exception ex)
        {
            AppLogger.LogError($"Error starting telemetry for config: {Config.ConnectionName}", ex, showMessageBox: true);
        }
    }
    
    private async Task CheckForDataReception()
    {
        Console.WriteLine("CheckForDataReception: Waiting 3 seconds...");
        // Wait for 3 seconds to see if any data is received
        await Task.Delay(3000);
        
        Console.WriteLine($"CheckForDataReception: After 3 seconds - hasReceivedAnyData: {_hasReceivedAnyData}, IsStarted: {IsStarted}");
        
        // Check if we've received any data
        if (!_hasReceivedAnyData && IsStarted)
        {
            Console.WriteLine("CheckForDataReception: No data received, showing connection failed dialog");
            AppLogger.LogWarning($"No data received after 3 seconds for config: {Config?.ConnectionName ?? "Unknown"}", showMessageBox: false);
            
            // No data received, show the dialog
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                await ShowConnectionFailedDialog();
            });
        }
        else if (_hasReceivedAnyData)
        {
            Console.WriteLine("CheckForDataReception: Data reception confirmed");
            AppLogger.LogInfo($"Data reception confirmed for config: {Config?.ConnectionName ?? "Unknown"}", showMessageBox: false);
        }
    }
    
    private async Task ShowConnectionFailedDialog()
    {
        Console.WriteLine($"ShowConnectionFailedDialog() called - data received: {_hasReceivedAnyData}, is started: {IsStarted}");
        
        // Don't show the dialog if we've already received data or if telemetry is no longer started
        if (_hasReceivedAnyData || !IsStarted)
        {
            AppLogger.LogInfo($"Skipping connection failed dialog - data received: {_hasReceivedAnyData}, is started: {IsStarted}", showMessageBox: false);
            return;
        }
        
        AppLogger.LogWarning($"Connection failed for config: {Config?.ConnectionName ?? "Unknown"}", showMessageBox: false);
        
        var shouldLoadLogs = await AppLogger.ShowConfirmationAsync(
            $"The system can't connect to the specified data source ({Config?.ConnectionName}).\n\nWould you like to load the last recorded log of this config?",
            "Connection Failed"
        );
        
        if (shouldLoadLogs)
        {
            AppLogger.LogInfo($"Loading previous logs for config: {Config?.ConnectionName ?? "Unknown"}", showMessageBox: false);
            LoadLastLogFile(Config!);
        }
        else
        {
            AppLogger.LogInfo("User chose not to load previous logs, resetting plotting page", showMessageBox: false);
            ResetPlottingPage();
        }
    }
    
    private void ResetPlottingPage()
    {
        // Stop any ongoing processes
        Stop();
        
        // Reset the tile to initial state
        IsStarted = false;
        IsPaused = false;
        IsLiveView = true;
        
        // Clear any loaded data
        _historyBuffer.Clear();
        Channels.Clear();
        Series.Clear();
        
        // Reset X and Y axes to default
        if (XAxes.Length > 0)
        {
            XAxes[0].MinLimit = 0;
            XAxes[0].MaxLimit = WindowSize;
        }
        if (YAxes.Length > 0)
        {
            YAxes[0].MinLimit = -10000;
            YAxes[0].MaxLimit = 10000;
        }
        
        // Remove from active config panels
        if (Config != null)
        {
            ParentViewModel?.RemoveActiveConfigPanel(Config, this);
        }
        
        // Reset config
        Config = null;
        
        // Reset data reception flags
        _hasReceivedAnyData = false;
        _lastDataReceivedTime = DateTime.MinValue;
    }
    
    private bool IsDataSourceConnected(InputConfig config)
    {
        try
        {
            if (config.ConnectionType == "UART")
            {
                // Check if serial port is available
                var ports = System.IO.Ports.SerialPort.GetPortNames();
                return ports.Contains(config.ComPort);
            }
            else if (config.ConnectionType == "UDP")
            {
                // For UDP, we'll try to detect if data is being received
                // Since UDP is connectionless, we'll assume it's connected and let the acquisition loop handle failures
                return true; // Assume UDP is connected and let the acquisition handle connection issues
            }
            return false;
        }
        catch
        {
            return false;
        }
    }
    
    private void LoadLastLogFile(InputConfig config)
    {
        Console.WriteLine($"LoadLastLogFile() called for config: {config.ConnectionName}");
        
        try
        {
            Console.WriteLine($"Loading last log file for config: {config.ConnectionName}");
            
            // Find the most recent log file for this config
            var logDir = AppPaths.GetLogDirectory(config.ConnectionName ?? "default");
            Console.WriteLine($"Looking for log files in: {logDir}");
            
            if (Directory.Exists(logDir))
            {
                // Filter out empty log files (files with only header or no data)
                var logFiles = Directory.GetFiles(logDir, "*.csv")
                    .Where(file => 
                    {
                        try
                        {
                            var lines = File.ReadAllLines(file);
                            // Check if file has more than just the header (at least 2 lines)
                            // Also check that the second line is not empty
                            return lines.Length > 1 && !string.IsNullOrWhiteSpace(lines[1]);
                        }
                        catch
                        {
                            return false;
                        }
                    })
                    .OrderByDescending(f => File.GetLastWriteTime(f))
                    .ToList();
                
                Console.WriteLine($"Found {logFiles.Count} non-empty log files");
                
                if (logFiles.Any())
                {
                    var latestLogFile = logFiles.First();
                    Console.WriteLine($"Loading latest log file: {latestLogFile}");
                    
                    // Set the log file path for future reference
                    _logFilePath = latestLogFile;
                    Console.WriteLine($"Set log file path for historical navigation: {_logFilePath}");
                    AppLogger.LogInfo($"Set log file path for historical navigation: {_logFilePath}", showMessageBox: false);
                    
                    LoadLogFileIntoHistory(latestLogFile);
                    
                    Console.WriteLine($"Loaded {_historyBuffer.Count} samples into history buffer");
                    
                    // Set to historical view mode
                    Console.WriteLine("Setting IsLiveView to false for historical data");
                    IsLiveView = false;
                    IsPaused = true; // Start in paused mode for historical data
                    
                    // Calculate max scroll position
                    if (_historyBuffer.Count > 0)
                    {
                        MaxScrollPosition = Math.Max(0, (_historyBuffer.Count - 1) / WindowSize);
                        ScrollPosition = MaxScrollPosition; // Start at the end
                        
                        Console.WriteLine($"MaxScrollPosition: {MaxScrollPosition}, ScrollPosition: {ScrollPosition}");
                        
                        // Load the data into the chart
                        LoadHistoryWindow(ScrollPosition * WindowSize);
                        
                        // Force UI update
                        OnPropertyChanged(nameof(Channels));
                        OnPropertyChanged(nameof(Series));
                    }
                }
                else
                {
                    Console.WriteLine("No non-empty log files found");
                    // Show a message to the user that no historical data is available
                    ShowNoHistoricalDataMessage();
                }
            }
            else
            {
                Console.WriteLine($"Log directory does not exist: {logDir}");
                ShowNoHistoricalDataMessage();
            }
        }
        catch (Exception ex)
        {
            // Handle error - could show a message to user
            Console.WriteLine($"Error loading log file: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            ShowNoHistoricalDataMessage();
        }
    }
    
    private void LoadLogFileIntoHistory(string logFilePath)
    {
        _historyBuffer.Clear();
        
        try
        {
            Console.WriteLine($"Reading log file: {logFilePath}");
            var lines = File.ReadAllLines(logFilePath);
            Console.WriteLine($"Found {lines.Length} lines in log file");
            
            if (lines.Length > 0)
            {
                Console.WriteLine($"Header: {lines[0]}");
            }
            
            // Calculate total samples and find the latest sample index
            int latestSampleIndex = -1;
            foreach (var line in lines.Skip(1)) // Skip header
            {
                var parts = line.Split(',');
                if (parts.Length > 1)
                {
                    // Parse the sample index first
                    if (!int.TryParse(parts[0], out int sampleIndex))
                    {
                        Console.WriteLine($"Failed to parse sample index from line: {line}");
                        continue;
                    }
                    
                    var values = new double[parts.Length]; // Include sample index
                    values[0] = sampleIndex; // Store sample index as first element
                    
                    for (int i = 1; i < parts.Length; i++)
                    {
                        if (double.TryParse(parts[i], out double value))
                        {
                            values[i] = value;
                        }
                        else
                        {
                            Console.WriteLine($"Failed to parse value at position {i} from line: {line}");
                            values[i] = 0; // Default to 0 if parsing fails
                        }
                    }
                    _historyBuffer.Add(values);
                }
            }
            
            Console.WriteLine($"Successfully loaded {_historyBuffer.Count} samples");
            if (_historyBuffer.Count > 0)
            {
                Console.WriteLine($"First sample index: {_historyBuffer[0][0]}, Last sample index: {_historyBuffer[_historyBuffer.Count - 1][0]}");
                
                // Set up historical navigation
                _totalSamplesInLog = _historyBuffer.Count;
                int lastSampleIndex = (int)_historyBuffer[_historyBuffer.Count - 1][0];
                _currentHistoryWindowStart = Math.Max(0, lastSampleIndex - WindowSize + 1);
                
                Console.WriteLine($"Total samples in log: {_totalSamplesInLog}, Last sample index: {lastSampleIndex}, Current window start: {_currentHistoryWindowStart}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading log file: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }

    public void Stop()
    {
        AppLogger.LogInfo($"Stopping telemetry for config: {Config?.ConnectionName ?? "Unknown"}", showMessageBox: false);
        
        IsStarted = false;
        
        // Remove from active config panels
        if (Config != null)
        {
            ParentViewModel?.RemoveActiveConfigPanel(Config, this);
        }
        
        StopUartAcquisition();
        StopUdpAcquisition();
        
        AppLogger.LogInfo($"Telemetry stopped for config: {Config?.ConnectionName ?? "Unknown"}", showMessageBox: false);
    }

    private bool TryStartUartAcquisition(InputConfig config)
    {
        StopUartAcquisition();
        if (string.IsNullOrWhiteSpace(config.ComPort) || string.IsNullOrWhiteSpace(config.BaudRate))
            return false;
        
        try
        {
            _serialPort = new SerialPort(config.ComPort, int.Parse(config.BaudRate));
            _serialPort.Open();
            _serialCts = new CancellationTokenSource();
            StartLogging(config.ConnectionName ?? "default", config.Channels);
            Task.Run(() => SerialReadLoop(config, _serialCts.Token));
            return true;
        }
        catch (Exception)
        {
            // Connection failed
            return false;
        }
    }
    
    private void StartUartAcquisition(InputConfig config)
    {
        TryStartUartAcquisition(config);
    }

    private void StopUartAcquisition()
    {
        try
        {
            _serialCts?.Cancel();
            _serialPort?.Close();
        }
        catch (Exception) { }
        _serialPort = null;
        _serialCts = null;
        StopLogging();
    }

    private bool TryStartUdpAcquisition(InputConfig config)
    {
        StopUdpAcquisition();
        if (string.IsNullOrWhiteSpace(config.UdpPort))
            return false;
        
        try
        {
            if (!int.TryParse(config.UdpPort, out int port))
                return false;
            
            _udpClient = new UdpClient(port);
            _udpEndPoint = new IPEndPoint(IPAddress.Any, port);
            _udpCts = new CancellationTokenSource();
            
            StartLogging(config.ConnectionName ?? "default", config.Channels);
            
            Task.Run(() => UdpReadLoop(config, _udpCts.Token));
            return true;
        }
        catch (Exception)
        {
            // Connection failed
            return false;
        }
    }
    

    
    private void StartUdpAcquisition(InputConfig config)
    {
        TryStartUdpAcquisition(config);
    }

    private void StopUdpAcquisition()
    {
        try
        {
            _udpCts?.Cancel();
            _udpClient?.Close();
        }
        catch (Exception) { }
        _udpClient = null;
        _udpCts = null;
        _udpEndPoint = null;
        StopLogging();
    }

    private void StartLogging(string configName, List<Channel> configChannels)
    {
        try
        {
            AppLogger.LogInfo($"Starting logging for config: {configName}", showMessageBox: false);
            
            _logCts = new CancellationTokenSource();
            var logsDir = AppPaths.GetLogDirectory(configName);
            AppPaths.EnsureConnectionLogDirectoryExists(configName);
            var logFile = Path.Combine(logsDir, $"log_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
            _logFilePath = logFile;
            Console.WriteLine($"StartLogging: Set log file path to: {_logFilePath}");
            
            // Don't create the header file immediately - wait for first data
            _logQueue = new BlockingCollection<string>(new ConcurrentQueue<string>());
            _logTask = Task.Run(() => LogWriterLoop(_logCts.Token));
            
            // Set a flag to track if we've received any data
            _hasReceivedData = false;
            
            AppLogger.LogSuccess($"Logging started for config: {configName} at {logFile}", showMessageBox: false);
        }
        catch (Exception ex)
        {
            AppLogger.LogError($"Failed to start logging for config: {configName}", ex, showMessageBox: false);
        }
    }

    private void StopLogging()
    {
        _logCts?.Cancel();
        _logQueue?.CompleteAdding();
        try { _logTask?.Wait(1000); } catch { }
        
        // If no data was received, delete the empty log file
        if (!_hasReceivedData && !string.IsNullOrEmpty(_logFilePath) && File.Exists(_logFilePath))
        {
            try
            {
                File.Delete(_logFilePath);
                AppLogger.LogInfo($"Deleted empty log file: {_logFilePath}", showMessageBox: false);
            }
            catch (Exception ex)
            {
                AppLogger.LogError($"Error deleting empty log file: {_logFilePath}", ex, showMessageBox: false);
            }
        }
        
        _logTask = null;
        _logCts = null;
        _hasReceivedData = false;
        _hasReceivedAnyData = false;
        _lastDataReceivedTime = DateTime.MinValue;
        
        AppLogger.LogInfo("Logging stopped", showMessageBox: false);
    }

    private void LogWriterLoop(CancellationToken token)
    {
        try
        {
            if (string.IsNullOrEmpty(_logFilePath)) return;
            
            bool headerWritten = false;
            using var sw = new StreamWriter(_logFilePath, append: false);
            var batch = new List<string>(100);
            
            while (!token.IsCancellationRequested || !_logQueue.IsCompleted)
            {
                batch.Clear();
                while (batch.Count < 100 && _logQueue.TryTake(out var line, 100))
                {
                    batch.Add(line);
                }
                if (batch.Count > 0)
                {
                    // Write header only when we receive first data
                    if (!headerWritten && Config?.Channels != null)
                    {
                        var header = "Sample";
                        if (Config.Channels.Count > 0)
                            header += "," + string.Join(",", Config.Channels.Select(ch => ch.Name ?? "Channel"));
                        sw.WriteLine(header);
                        headerWritten = true;
                    }
                    
                    foreach (var line in batch)
                        sw.WriteLine(line);
                    sw.Flush();
                }
            }
        }
        catch { }
    }

    private void LogSample(int sampleIndex, double[] values)
    {
        if (_logQueue != null && !_logQueue.IsAddingCompleted)
        {
            _hasReceivedData = true; // Mark that we've received data
            _hasReceivedAnyData = true; // Mark that we've received any data at all
            _lastDataReceivedTime = DateTime.UtcNow; // Update the last data received time
            var line = sampleIndex.ToString() + "," + string.Join(",", values);
            _logQueue.TryAdd(line);
        }
    }

    // Data reading methods (similar to the main ViewModel but for this tile)
    private void SerialReadLoop(InputConfig config, CancellationToken token)
    {
        Console.WriteLine($"SerialReadLoop: Starting UART reading for config: {config.ConnectionName}");
        
        if (_serialPort == null) return;
        
        int maxSamples = WindowSize;
        int sampleIndex = 0;
        
        try
        {
            if (config.Mode == "Binary Mode")
                SerialReadBinary(config, _serialPort, maxSamples, token, ref sampleIndex);
            else
                SerialReadCsv(config, _serialPort, maxSamples, token, ref sampleIndex);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SerialReadLoop error: {ex.Message}");
        }
    }

    private void UdpReadLoop(InputConfig config, CancellationToken token)
    {
        if (_udpClient == null || _udpEndPoint == null) return;
        
        int maxSamples = WindowSize;
        int sampleIndex = 0;
        
        try
        {
            if (config.Mode == "Binary Mode")
                UdpReadBinary(config, _udpClient, _udpEndPoint, maxSamples, token, ref sampleIndex);
            else
                UdpReadCsv(config, _udpClient, _udpEndPoint, maxSamples, token, ref sampleIndex);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"UdpReadLoop error: {ex.Message}");
        }
    }

    // Shared plotting method for both UDP and UART
    private void ProcessAndPlotData(int sampleIndex, double[] values, int maxSamples, int channelCount, string source)
    {
        if (_resumeClicked)
        {
            _resumeClicked = false;
        }
        
        LogSample(sampleIndex, values);
        Console.WriteLine($"{source}: Received sample {sampleIndex} with values: [{string.Join(", ", values)}]");
        
        if (!IsPaused && IsStarted)
        {
            for (int i = 0; i < channelCount; i++)
            {
                _pendingPoints[i].Add(new ObservablePoint(sampleIndex, values[i]));
            }
            
            if ((DateTime.UtcNow - _lastUiUpdate).TotalMilliseconds >= UiUpdateIntervalMs)
            {
                var pendingCopy = _pendingPoints.Select(list => list.ToList()).ToList();
                _pendingPoints.ForEach(list => list.Clear());
                _lastUiUpdate = DateTime.UtcNow;
                Console.WriteLine($"{source}: UI update - pending points: [{string.Join(", ", pendingCopy.Select(list => list.Count))}]");
                Dispatcher.UIThread.Post(() =>
                {
                    for (int i = 0; i < channelCount && i < Channels.Count; i++)
                    {
                        var toAdd = pendingCopy[i].Count > 10 ? pendingCopy[i].Skip(pendingCopy[i].Count - 10) : pendingCopy[i];
                        foreach (var pt in toAdd)
                        {
                            if (Channels[i].Values.Count > maxSamples)
                                Channels[i].Values.RemoveAt(0);
                            Channels[i].Values.Add(pt);
                        }
                        Console.WriteLine($"{source}: Channel {i} now has {Channels[i].Values.Count} points");
                    }
                    if (pendingCopy[0].Count > 0)
                    {
                        int idx2 = (int)pendingCopy[0].Last().X;
                        XAxes[0].MinLimit = Math.Max(0, idx2 - maxSamples + 1);
                        XAxes[0].MaxLimit = idx2;
                        Console.WriteLine($"{source}: Updated X-axis limits to {XAxes[0].MinLimit} - {XAxes[0].MaxLimit}");
                    }
                    
                    // Force chart update
                    Console.WriteLine($"{source}: Forcing chart update notification");
                    OnPropertyChanged(nameof(Series));
                });
            }
        }
    }

    // Binary and CSV reading methods (copy from main ViewModel)
    private void SerialReadBinary(InputConfig config, SerialPort port, int maxSamples, CancellationToken token, ref int sampleIndex)
    {
        Console.WriteLine($"SerialReadBinary: Starting UART binary mode reading");
        byte syncByte = 0xAA;
        if (!string.IsNullOrWhiteSpace(config.SyncByte))
        {
            try { syncByte = Convert.ToByte(config.SyncByte, 16); } catch { }
        }
        int channelCount = config.Channels?.Count ?? 0;
        int[] byteLengths = config.Channels?.Select(ch => GetTypeByteLength(ch?.Length)).ToArray() ?? new int[0];
        int packetSize = byteLengths.Sum();
        var buf = new byte[packetSize];
        
        Console.WriteLine($"SerialReadBinary: Channel count: {channelCount}, Packet size: {packetSize}, Sync byte: 0x{syncByte:X2}");
        
        while (!token.IsCancellationRequested)
        {
            try
            {
                int b = port.ReadByte();
                if (b < 0) continue;
                if ((byte)b != syncByte) continue;
                
                int read = 0;
                while (read < packetSize)
                {
                    int r = port.Read(buf, read, packetSize - read);
                    if (r <= 0) break;
                    read += r;
                }
                if (read != packetSize) continue;
                
                var values = new double[channelCount];
                int offset = 0;
                for (int i = 0; i < channelCount; i++)
                {
                    values[i] = ParseValue(buf, offset, config.Channels?[i]?.Length);
                    offset += byteLengths[i];
                }
                int idx = sampleIndex++;
                
                ProcessAndPlotData(idx, values, maxSamples, channelCount, "SerialReadBinary");
            }
            catch (Exception) { Thread.Sleep(1); }
        }
    }

    private void SerialReadCsv(InputConfig config, SerialPort port, int maxSamples, CancellationToken token, ref int sampleIndex)
    {
        Console.WriteLine($"SerialReadCsv: Starting UART CSV mode reading");
        string? sync = config.SyncByte;
        int channelCount = config.Channels?.Count ?? 0;
        
        while (!token.IsCancellationRequested)
        {
            try
            {
                var line = port.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;
                var parts = line.Split(',');
                if (parts.Length < channelCount + (string.IsNullOrWhiteSpace(sync) ? 0 : 1)) continue;
                
                int offset = 0;
                if (!string.IsNullOrWhiteSpace(sync))
                {
                    if (parts[0] != sync) continue;
                    offset = 1;
                }
                
                var values = new double[channelCount];
                for (int i = 0; i < channelCount; i++)
                {
                    double.TryParse(parts[i + offset], out values[i]);
                }
                int idx = sampleIndex++;
                
                ProcessAndPlotData(idx, values, maxSamples, channelCount, "SerialReadCsv");
            }
            catch (Exception) { Thread.Sleep(1); }
        }
    }

    private void UdpReadBinary(InputConfig config, UdpClient client, IPEndPoint endPoint, int maxSamples, CancellationToken token, ref int sampleIndex)
    {
        byte syncByte = 0xAA;
        if (!string.IsNullOrWhiteSpace(config.SyncByte))
        {
            try { syncByte = Convert.ToByte(config.SyncByte, 16); } catch { }
        }
        int channelCount = config.Channels?.Count ?? 0;
        int[] byteLengths = config.Channels?.Select(ch => GetTypeByteLength(ch.Length)).ToArray() ?? new int[0];
        int packetSize = byteLengths.Sum();
        var buf = new byte[packetSize];
        
        while (!token.IsCancellationRequested)
        {
            try
            {
                var result = client.Receive(ref endPoint);
                if (result.Length < packetSize + 1) continue;
                
                if (result[0] != syncByte) continue;
                
                Array.Copy(result, 1, buf, 0, packetSize);
                
                var values = new double[channelCount];
                int offset = 0;
                for (int i = 0; i < channelCount; i++)
                {
                    values[i] = ParseValue(buf, offset, config.Channels?[i]?.Length);
                    offset += byteLengths[i];
                }
                int idx = sampleIndex++;
                
                ProcessAndPlotData(idx, values, maxSamples, channelCount, "UdpReadBinary");
            }
            catch (Exception) { Thread.Sleep(1); }
        }
    }

    private void UdpReadCsv(InputConfig config, UdpClient client, IPEndPoint endPoint, int maxSamples, CancellationToken token, ref int sampleIndex)
    {
        string? sync = config.SyncByte;
        int channelCount = config.Channels?.Count ?? 0;
        
        while (!token.IsCancellationRequested)
        {
            try
            {
                var result = client.Receive(ref endPoint);
                var line = System.Text.Encoding.UTF8.GetString(result);
                if (string.IsNullOrWhiteSpace(line)) continue;
                var parts = line.Split(',');
                if (parts.Length < channelCount + (string.IsNullOrWhiteSpace(sync) ? 0 : 1)) continue;
                
                int offset = 0;
                if (!string.IsNullOrWhiteSpace(sync))
                {
                    if (parts[0] != sync) continue;
                    offset = 1;
                }
                
                var values = new double[channelCount];
                for (int i = 0; i < channelCount; i++)
                {
                    double.TryParse(parts[i + offset], out values[i]);
                }
                int idx = sampleIndex++;
                
                ProcessAndPlotData(idx, values, maxSamples, channelCount, "UdpReadCsv");
            }
            catch (Exception) { Thread.Sleep(1); }
        }
    }

    // Utility methods
    private int GetTypeByteLength(string? type)
    {
        return type?.ToLower() switch
        {
            "int8" or "uint8" => 1,
            "int16" or "uint16" => 2,
            "int32" or "uint32" => 4,
            _ => 2
        };
    }

    private double ParseValue(byte[] buf, int offset, string? type)
    {
        return type?.ToLower() switch
        {
            "int8" => (sbyte)buf[offset],
            "uint8" => buf[offset],
            "int16" => BitConverter.ToInt16(buf, offset),
            "uint16" => BitConverter.ToUInt16(buf, offset),
            "int32" => BitConverter.ToInt32(buf, offset),
            "uint32" => BitConverter.ToUInt32(buf, offset),
            _ => BitConverter.ToInt16(buf, offset)
        };
    }

    // History and scrolling methods
    public void LoadHistoryWindow(int startIndex, bool skipChartUpdate = false)
    {
        Console.WriteLine($"LoadHistoryWindow called with startIndex: {startIndex}, historyBuffer.Count: {_historyBuffer.Count}, IsLiveView: {IsLiveView}");
        
        if (IsLiveView)
        {
            // In live view, treat startIndex as array index
            if (_historyBuffer.Count == 0) 
            {
                Console.WriteLine("History buffer is empty, returning");
                return;
            }
            
            var samples = new List<double[]>();
            int endIndex = Math.Min(startIndex + WindowSize, _historyBuffer.Count);
            
            Console.WriteLine($"Loading samples from array index {startIndex} to {endIndex}");
            
            for (int i = startIndex; i < endIndex; i++)
            {
                samples.Add(_historyBuffer[i]);
            }
            
            Console.WriteLine($"Collected {samples.Count} samples to display");
            
            if (!skipChartUpdate)
            {
                Console.WriteLine("Updating chart with historical data");
                Dispatcher.UIThread.Post(() =>
                {
                    Console.WriteLine($"UI Thread: Clearing {Channels.Count} channels");
                    for (int ch = 0; ch < Channels.Count; ch++)
                        Channels[ch].Values.Clear();

                    Console.WriteLine($"UI Thread: Adding {samples.Count} samples to chart");
                    foreach (var arr in samples)
                    {
                        int sampleIdx = (int)arr[0];
                        for (int ch = 0; ch < Channels.Count && ch + 1 < arr.Length; ch++)
                        {
                            Channels[ch].Values.Add(new ObservablePoint(sampleIdx, arr[ch + 1]));
                        }
                    }

                    if (XAxes.Length > 0)
                    {
                        XAxes[0].MinLimit = startIndex;
                        XAxes[0].MaxLimit = startIndex + WindowSize;
                        Console.WriteLine($"UI Thread: Set X-axis limits to {startIndex} - {startIndex + WindowSize}");
                    }
                    
                    Console.WriteLine($"UI Thread: Chart update complete. Channel 0 has {Channels[0].Values.Count} points");
                });
            }

            _scrollStartIndex = startIndex;
            _lastDisplayedSampleIndex = samples.Count > 0 ? (int)samples.Last()[0] : -1;
            Console.WriteLine($"LoadHistoryWindow complete. Last displayed sample index: {_lastDisplayedSampleIndex}");
        }
        else
        {
            // In historical view, treat startIndex as sample index and use log file
            Console.WriteLine("Using log file approach for LoadHistoryWindow");
            var samples = LoadWindowFromLogFile(startIndex, WindowSize);
            if (samples.Count > 0)
            {
                UpdateChartWithHistoricalData(samples, startIndex);
            }
        }
    }

    public void JumpToLive()
    {
        var latestIndex = GetLatestSampleIndex();
        if (latestIndex >= 0)
        {
            int startIndex = Math.Max(0, latestIndex - WindowSize + 1);
            
            if (IsLiveView)
            {
                // If in live view, use the in-memory buffer
                LoadHistoryWindow(startIndex);
            }
            else
            {
                // If in historical view, load from log file
                Console.WriteLine($"Jumping to live data - Using log file: {_logFilePath}");
                AppLogger.LogInfo($"Jumping to live data - Using log file: {_logFilePath}", showMessageBox: false);
                var samples = LoadWindowFromLogFile(startIndex, WindowSize);
                if (samples.Count > 0)
                {
                    _currentHistoryWindowStart = startIndex;
                    UpdateChartWithHistoricalData(samples, startIndex);
                    AppLogger.LogSuccess($"Jumped to live data at index {startIndex} with {samples.Count} samples", showMessageBox: false);
                }
                else
                {
                    AppLogger.LogWarning($"No live data found at index {startIndex}", showMessageBox: false);
                }
            }
        }
    }
    
    public void JumpToIndex(int index)
    {
        if (IsLiveView)
        {
            // If in live view, use the in-memory buffer
            if (index >= 0 && index < _historyBuffer.Count)
            {
                int scrollPos = index / WindowSize;
                ScrollPosition = Math.Min(scrollPos, MaxScrollPosition);
                LoadHistoryWindow(index);
            }
        }
        else
        {
            // If in historical view, load from log file
            if (index >= 0)
            {
                Console.WriteLine($"Jumping to index {index} - Using log file: {_logFilePath}");
                AppLogger.LogInfo($"Jumping to index {index} - Using log file: {_logFilePath}", showMessageBox: false);
                var samples = LoadWindowFromLogFile(index, WindowSize);
                if (samples.Count > 0)
                {
                    _currentHistoryWindowStart = index;
                    UpdateChartWithHistoricalData(samples, index);
                    AppLogger.LogSuccess($"Jumped to index {index} with {samples.Count} samples", showMessageBox: false);
                }
                else
                {
                    AppLogger.LogWarning($"No data found at index {index}", showMessageBox: false);
                }
            }
        }
    }

    public void RollBack()
    {
        Console.WriteLine($"RollBack called - IsLiveView: {IsLiveView}, IsPaused: {IsPaused}");
        
        if (IsLiveView && !IsPaused)
        {
            // In live view and not paused, use the existing in-memory buffer approach
            if (XAxes.Length > 0 && XAxes[0].MinLimit.HasValue)
            {
                int currentMin = (int)XAxes[0].MinLimit!.Value;
                int newMin = Math.Max(0, currentMin - _rollbackStep);
                XAxes[0].MinLimit = newMin;
                XAxes[0].MaxLimit = newMin + WindowSize;
                LoadHistoryWindow(newMin);
            }
        }
        else
        {
            // In historical view OR live view but paused, use log file approach
            Console.WriteLine("Using log file approach for roll back");
            
            // If _currentHistoryWindowStart is not initialized, use the current X-axis position
            if (_currentHistoryWindowStart == 0 && XAxes.Length > 0 && XAxes[0].MinLimit.HasValue)
            {
                _currentHistoryWindowStart = (int)XAxes[0].MinLimit!.Value;
                Console.WriteLine($"Roll back: Initialized current history window start to: {_currentHistoryWindowStart}");
            }
            
            // Calculate step size as WindowSize/5
            int stepSize = WindowSize / 5;
            Console.WriteLine($"Roll back step size: {stepSize} (WindowSize: {WindowSize})");
            
            // Calculate the new window start (go back by step size)
            int newWindowStart = Math.Max(0, _currentHistoryWindowStart - stepSize);
            
            if (newWindowStart == _currentHistoryWindowStart)
            {
                Console.WriteLine("Already at the beginning of the log file");
                return; // Already at the beginning
            }
            
            Console.WriteLine($"Loading previous window from log file: start={newWindowStart}, step size={stepSize}");
            var samples = LoadWindowFromLogFile(newWindowStart, WindowSize);
            if (samples.Count > 0)
            {
                _currentHistoryWindowStart = newWindowStart;
                UpdateChartWithHistoricalData(samples, newWindowStart);
                Console.WriteLine($"Successfully loaded {samples.Count} samples for roll back");
            }
        }
    }

    public void RollForward()
    {
        Console.WriteLine($"RollForward called - IsLiveView: {IsLiveView}, IsPaused: {IsPaused}");
        
        if (IsLiveView && !IsPaused)
        {
            // In live view and not paused, use the existing in-memory buffer approach
            if (XAxes.Length > 0 && XAxes[0].MinLimit.HasValue)
            {
                int currentMin = (int)XAxes[0].MinLimit!.Value;
                int newMin = currentMin + _rollbackStep;
                XAxes[0].MinLimit = newMin;
                XAxes[0].MaxLimit = newMin + WindowSize;
                LoadHistoryWindow(newMin);
            }
        }
        else
        {
            // In historical view OR live view but paused, use log file approach
            Console.WriteLine("Using log file approach for roll forward");
            
            // If _currentHistoryWindowStart is not initialized, use the current X-axis position
            if (_currentHistoryWindowStart == 0 && XAxes.Length > 0 && XAxes[0].MinLimit.HasValue)
            {
                _currentHistoryWindowStart = (int)XAxes[0].MinLimit!.Value;
                Console.WriteLine($"Roll forward: Initialized current history window start to: {_currentHistoryWindowStart}");
            }
            
            // Calculate step size as WindowSize/5
            int stepSize = WindowSize / 5;
            Console.WriteLine($"Roll forward step size: {stepSize} (WindowSize: {WindowSize})");
            
            // Calculate the new window start (go forward by step size)
            int newWindowStart = _currentHistoryWindowStart + stepSize;
            
            // Check if we're at the end of the log file
            int latestSampleIndex = GetLatestSampleIndex();
            if (newWindowStart >= latestSampleIndex)
            {
                Console.WriteLine("Already at the end of the log file");
                return; // Already at the end
            }
            
            Console.WriteLine($"Loading next window from log file: start={newWindowStart}, step size={stepSize}");
            var samples = LoadWindowFromLogFile(newWindowStart, WindowSize);
            if (samples.Count > 0)
            {
                _currentHistoryWindowStart = newWindowStart;
                UpdateChartWithHistoricalData(samples, newWindowStart);
                Console.WriteLine($"Successfully loaded {samples.Count} samples for roll forward");
            }
        }
    }



    public int GetLatestSampleIndex()
    {
        if (string.IsNullOrEmpty(_logFilePath) || !File.Exists(_logFilePath))
            return -1;
            
        int latestSampleIndex = -1;
        using (var sr = new StreamReader(_logFilePath))
        {
            var header = sr.ReadLine(); // skip header
            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                var parts = line.Split(',');
                if (parts.Length < 2) continue;
                if (int.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out int idx))
                    latestSampleIndex = idx;
            }
        }
        return latestSampleIndex;
    }



    /// <summary>
    /// Updates the chart with historical data loaded from the log file
    /// </summary>
    /// <param name="samples">The samples to display</param>
    /// <param name="startIndex">The starting index for the data</param>
    private void UpdateChartWithHistoricalData(List<double[]> samples, int startIndex)
    {
        if (samples.Count == 0) return;
        
        Console.WriteLine($"UpdateChartWithHistoricalData: Updating chart with {samples.Count} samples starting from index {startIndex}");
        
        Dispatcher.UIThread.Post(() =>
        {
            // Clear existing data
            foreach (var channel in Channels)
            {
                channel.Values.Clear();
            }
            
            // Add the new data
            foreach (var sample in samples)
            {
                int sampleIdx = (int)sample[0];
                for (int ch = 0; ch < Channels.Count && ch + 1 < sample.Length; ch++)
                {
                    Channels[ch].Values.Add(new ObservablePoint(sampleIdx, sample[ch + 1]));
                }
            }
            
            // Update X-axis limits
            if (XAxes.Length > 0)
            {
                XAxes[0].MinLimit = startIndex;
                XAxes[0].MaxLimit = startIndex + WindowSize;
                Console.WriteLine($"Updated X-axis limits to: {XAxes[0].MinLimit} - {XAxes[0].MaxLimit}");
            }
            
            // Force multiple property changes to ensure UI updates on Windows
            OnPropertyChanged(nameof(Channels));
            OnPropertyChanged(nameof(Series));
            OnPropertyChanged(nameof(XAxes));
            OnPropertyChanged(nameof(YAxes));
            
            AppLogger.LogInfo($"Chart updated with {samples.Count} samples starting from index {startIndex}", showMessageBox: false);
        });
    }

    /// <summary>
    /// Loads the previous window of data from the log file (for roll back)
    /// </summary>
    private void LoadPreviousWindowFromLog()
    {
        Console.WriteLine($"LoadPreviousWindowFromLog: Checking log file: {_logFilePath}");
        Console.WriteLine($"LoadPreviousWindowFromLog: File exists: {File.Exists(_logFilePath)}");
        
        if (string.IsNullOrEmpty(_logFilePath) || !File.Exists(_logFilePath))
        {
            AppLogger.LogWarning("No log file available for loading previous window", showMessageBox: false);
            return;
        }

        try
        {
            // Print which log file we're using
            Console.WriteLine($"Rolling BACK - Using log file: {_logFilePath}");
            AppLogger.LogInfo($"Rolling BACK - Using log file: {_logFilePath}", showMessageBox: false);
            
            // Calculate the new window start (go back by window size)
            int newWindowStart = Math.Max(0, _currentHistoryWindowStart - WindowSize);
            
            if (newWindowStart == _currentHistoryWindowStart)
            {
                AppLogger.LogInfo("Already at the beginning of the log file", showMessageBox: false);
                return; // Already at the beginning
            }

            AppLogger.LogInfo($"Loading previous window from log file: start={newWindowStart}, window size={WindowSize}", showMessageBox: false);
            
            var samples = LoadWindowFromLogFile(newWindowStart, WindowSize);
            if (samples.Count > 0)
            {
                _currentHistoryWindowStart = newWindowStart;
                UpdateChartWithHistoricalData(samples, newWindowStart);
                AppLogger.LogSuccess($"Successfully loaded previous window with {samples.Count} samples", showMessageBox: false);
            }
            else
            {
                AppLogger.LogWarning("No data found in previous window", showMessageBox: false);
            }
        }
        catch (Exception ex)
        {
            AppLogger.LogError($"Error loading previous window from log file: {ex.Message}", ex, showMessageBox: false);
        }
    }

    /// <summary>
    /// Loads the next window of data from the log file (for roll forward)
    /// </summary>
    private void LoadNextWindowFromLog()
    {
        Console.WriteLine($"LoadNextWindowFromLog: Checking log file: {_logFilePath}");
        Console.WriteLine($"LoadNextWindowFromLog: File exists: {File.Exists(_logFilePath)}");
        
        if (string.IsNullOrEmpty(_logFilePath) || !File.Exists(_logFilePath))
        {
            AppLogger.LogWarning("No log file available for loading next window", showMessageBox: false);
            return;
        }

        try
        {
            // Print which log file we're using
            Console.WriteLine($"Rolling FORWARD - Using log file: {_logFilePath}");
            AppLogger.LogInfo($"Rolling FORWARD - Using log file: {_logFilePath}", showMessageBox: false);
            
            // Calculate the new window start (go forward by window size)
            int newWindowStart = _currentHistoryWindowStart + WindowSize;
            
            // Check if we're at the end of the log file
            int latestSampleIndex = GetLatestSampleIndex();
            if (newWindowStart >= latestSampleIndex)
            {
                AppLogger.LogInfo("Already at the end of the log file", showMessageBox: false);
                return; // Already at the end
            }

            AppLogger.LogInfo($"Loading next window from log file: start={newWindowStart}, window size={WindowSize}", showMessageBox: false);
            
            var samples = LoadWindowFromLogFile(newWindowStart, WindowSize);
            if (samples.Count > 0)
            {
                _currentHistoryWindowStart = newWindowStart;
                UpdateChartWithHistoricalData(samples, newWindowStart);
                AppLogger.LogSuccess($"Successfully loaded next window with {samples.Count} samples", showMessageBox: false);
            }
            else
            {
                AppLogger.LogWarning("No data found in next window", showMessageBox: false);
            }
        }
        catch (Exception ex)
        {
            AppLogger.LogError($"Error loading next window from log file: {ex.Message}", ex, showMessageBox: false);
        }
    }

    /// <summary>
    /// Loads a specific window of data from the log file efficiently
    /// </summary>
    /// <param name="startIndex">The starting sample index</param>
    /// <param name="count">The number of samples to load</param>
    /// <returns>List of samples for the specified window</returns>
    private List<double[]> LoadWindowFromLogFile(int startIndex, int count)
    {
        var samples = new List<double[]>();
        int endIndex = startIndex + count;
        
        using (var sr = new StreamReader(_logFilePath))
        {
            var header = sr.ReadLine(); // skip header
            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                var parts = line.Split(',');
                if (parts.Length < 2) continue;
                
                if (int.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out int sampleIdx))
                {
                    // Check if this sample is in our target range
                    if (sampleIdx >= startIndex && sampleIdx < endIndex)
                    {
                        var values = new double[parts.Length];
                        for (int i = 0; i < parts.Length; i++)
                        {
                            if (double.TryParse(parts[i], NumberStyles.Any, CultureInfo.InvariantCulture, out double val))
                                values[i] = val;
                        }
                        samples.Add(values);
                    }
                    else if (sampleIdx >= endIndex)
                    {
                        // We've gone past our target range, stop reading
                        break;
                    }
                }
            }
        }
        
        return samples;
    }

    private async void ShowNoHistoricalDataMessage()
    {
        AppLogger.LogWarning($"No historical data available for config: {Config?.ConnectionName ?? "Unknown"}", showMessageBox: true);
        
        // Reset the plotting page since no historical data is available
        ResetPlottingPage();
    }
} 