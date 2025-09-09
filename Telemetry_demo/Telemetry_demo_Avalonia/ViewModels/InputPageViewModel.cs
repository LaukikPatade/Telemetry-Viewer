using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text.Json;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Telemetry_demo_Avalonia.Utils;
using System.Collections.Generic;

namespace Telemetry_demo_Avalonia.ViewModels;

public class Channel
{
    public string? Name { get; set; }
    public string? Length { get; set; }
    public Color Color { get; set; } = Colors.Blue;
}

public class InputConfig
{
    public string? ConnectionName { get; set; }
    public string? ConnectionType { get; set; }
    public string? ComPort { get; set; }
    public string? UdpPort { get; set; }
    public string? BaudRate { get; set; }
    public string? Mode { get; set; }
    public string? SyncByte { get; set; }
    public List<Channel> Channels { get; set; } = new();
    
    public override string ToString()
    {
        return ConnectionName ?? "Unknown Config";
    }
}

public partial class InputPageViewModel : ViewModelBase
{


    public ObservableCollection<string> ComPorts { get; } = new();
    public ObservableCollection<string> Modes { get; } = new() { "CSV Mode", "Binary Mode" };
    public ObservableCollection<string> ConnectionTypes { get; } = new() { "UART", "UDP" };
    public ObservableCollection<Channel> Channels { get; } = new();
    public ObservableCollection<string> ChannelLengths { get; } = new() { "int8", "int16", "int32", "uint8", "uint16", "uint32" };
    public ObservableCollection<string> BaudRates { get; } = new() { "9600", "19200", "38400", "57600", "115200", "230400", "460800", "921600", "Custom..." };

    public ObservableCollection<InputConfig> SavedConfigs { get; } = new();

    [ObservableProperty]
    private string? selectedMode = "CSV Mode";
    [ObservableProperty]
    private string? selectedConnectionType = "UART";
    [ObservableProperty]
    private string? connectionName;
    [ObservableProperty]
    private string? selectedComPort;
    [ObservableProperty]
    private string? udpPort;
    [ObservableProperty]
    private string? baudRate;
    [ObservableProperty]
    private string? customBaudRate;
    [ObservableProperty]
    private bool isCustomBaudRate = false;
    [ObservableProperty]
    private string? syncByte;

    public bool IsBinaryMode => SelectedMode == "Binary Mode";
    public bool IsUART => SelectedConnectionType == "UART";
    public bool IsUDP => SelectedConnectionType == "UDP";

    public IRelayCommand AddChannelCommand { get; }
    public IRelayCommand SaveInputConfigCommand { get; }

    partial void OnSelectedModeChanged(string? value)
    {
        OnPropertyChanged(nameof(IsBinaryMode));
    }
    partial void OnSelectedConnectionTypeChanged(string? value)
    {
        OnPropertyChanged(nameof(IsUART));
        OnPropertyChanged(nameof(IsUDP));
    }

    partial void OnBaudRateChanged(string? value)
    {
        // Check if "Custom..." was selected
        if (value == "Custom...")
        {
            IsCustomBaudRate = true;
            // Set a default custom baud rate if none is set
            if (string.IsNullOrEmpty(CustomBaudRate))
            {
                CustomBaudRate = "115200";
            }
        }
        else
        {
            IsCustomBaudRate = false;
        }
        
        // Validate baud rate in real-time
        if (!string.IsNullOrEmpty(value) && value != "Custom...")
        {
            if (!int.TryParse(value, out int baudRateValue) || baudRateValue <= 0)
            {
                // You could set a validation error property here if you want to show visual feedback
                Console.WriteLine($"Invalid baud rate entered: {value}");
            }
        }
    }

    public InputPageViewModel()
    {
        // Ensure all required directories exist
        AppPaths.EnsureAllDirectoriesExist();
        
        RefreshComPorts();
        Channels.Add(new Channel { Name = "", Length = "int8", Color = Colors.Blue });
        Channels.Add(new Channel { Name = "Test", Length = "int16", Color = Colors.Red });
        AddChannelCommand = new RelayCommand(AddChannel);
        SaveInputConfigCommand = new RelayCommand(SaveInputConfig);
        LoadSavedConfigs();
        
        AppLogger.LogInfo("InputPageViewModel initialized", showMessageBox: false);
    }

    private void LoadSavedConfigs()
    {
        SavedConfigs.Clear();
        var filePath = AppPaths.ConfigsFile;
        if (File.Exists(filePath))
        {
            try
            {
                var json = File.ReadAllText(filePath);
                var loaded = JsonSerializer.Deserialize<List<InputConfig>>(json);
                if (loaded != null)
                {
                    foreach (var cfg in loaded)
                        SavedConfigs.Add(cfg);
                    
                    AppLogger.LogInfo($"Loaded {loaded.Count} saved configurations", showMessageBox: false);
                }
            }
            catch (Exception ex)
            {
                AppLogger.LogError("Failed to load saved configurations", ex, showMessageBox: false);
            }
        }
        else
        {
            AppLogger.LogInfo("No saved configurations file found", showMessageBox: false);
        }
    }

    private void AddChannel()
    {
        Channels.Add(new Channel { Name = "", Length = ChannelLengths[0], Color = Colors.Blue });
    }

    private void SaveInputConfig()
    {
        try
        {
            // Determine the actual baud rate to use
            string actualBaudRate = BaudRate;
            if (BaudRate == "Custom...")
            {
                actualBaudRate = CustomBaudRate;
            }
            
            // Validate baud rate if provided
            if (!string.IsNullOrEmpty(actualBaudRate))
            {
                if (!int.TryParse(actualBaudRate, out int baudRateValue) || baudRateValue <= 0)
                {
                    AppLogger.LogError("Invalid baud rate. Please enter a valid positive number.", showMessageBox: true);
                    return;
                }
            }
            
            var config = new InputConfig
            {
                ConnectionName = ConnectionName,
                ConnectionType = SelectedConnectionType,
                ComPort = SelectedComPort,
                UdpPort = UdpPort,
                BaudRate = actualBaudRate,
                Mode = SelectedMode,
                SyncByte = SyncByte,
                Channels = Channels.Select(c => new Channel { Name = c.Name, Length = c.Length, Color = c.Color }).ToList()
            };
            
            var filePath = AppPaths.ConfigsFile;
            List<InputConfig> configs = new();
            if (File.Exists(filePath))
            {
                try
                {
                    var json = File.ReadAllText(filePath);
                    var loaded = JsonSerializer.Deserialize<List<InputConfig>>(json);
                    if (loaded != null)
                        configs = loaded;
                }
                catch (Exception ex)
                {
                    AppLogger.LogError("Failed to load existing configs", ex, showMessageBox: false);
                }
            }
            
            configs.Add(config);
            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(filePath, JsonSerializer.Serialize(configs, options));
            LoadSavedConfigs();
            
            AppLogger.LogSuccess($"Configuration saved: {config.ConnectionName}", showMessageBox: false);
        }
        catch (Exception ex)
        {
            AppLogger.LogError("Failed to save configuration", ex, showMessageBox: true);
        }
    }

    public void RefreshComPorts()
    {
        ComPorts.Clear();
        foreach (var port in SerialPort.GetPortNames())
        {
            ComPorts.Add(port);
        }
    }
} 