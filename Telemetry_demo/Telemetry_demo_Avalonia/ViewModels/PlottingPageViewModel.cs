
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using Telemetry_demo_Avalonia.Utils;
using System.Threading.Tasks; // Added for Task.Delay

namespace Telemetry_demo_Avalonia.ViewModels;

public partial class ChannelPlot : ObservableObject
{
    public string Name { get; set; } = "Channel";
    
    [ObservableProperty]
    private bool isSelected = true;
    
    [ObservableProperty]
    private bool isVisible = true;
    
    public ObservableCollection<ObservablePoint> Values { get; set; } = new();
    
    partial void OnIsSelectedChanged(bool value)
    {
        // When checkbox is toggled, update visibility
        IsVisible = value;
    }
}

public partial class ActiveConfigPanel : ObservableObject
{
    [ObservableProperty]
    private InputConfig config;

    [ObservableProperty]
    private TileViewModel tile;

    [ObservableProperty]
    private bool isExpanded = true;

    [ObservableProperty]
    private int xAxisRange = 1000;
    
    [ObservableProperty]
    private double yAxisMin = -10000;
    
    [ObservableProperty]
    private double yAxisMax = 10000;
    
    [ObservableProperty]
    private string skipToIndexText = "0";
    
    public IRelayCommand SetBothAxisRangeCommand { get; }
    public IRelayCommand ResetBothAxisRangeCommand { get; }
    public IRelayCommand DisconnectCommand { get; }
    public IRelayCommand SkipToIndexCommand { get; }
    
    public ActiveConfigPanel(InputConfig config, TileViewModel tile)
    {
        Config = config;
        Tile = tile;
        XAxisRange = tile.WindowSize; // Initialize with current window size
        
        // Initialize Y-axis range from current chart limits if available
        if (tile.YAxes.Length > 0)
        {
            YAxisMin = tile.YAxes[0].MinLimit.GetValueOrDefault(-10000);
            YAxisMax = tile.YAxes[0].MaxLimit.GetValueOrDefault(10000);
        }
        
        SetBothAxisRangeCommand = new RelayCommand(SetBothAxisRange);
        ResetBothAxisRangeCommand = new RelayCommand(ResetBothAxisRange);
        DisconnectCommand = new RelayCommand(Disconnect);
        SkipToIndexCommand = new RelayCommand(ExecuteSkipToIndex);
    }
    
    // Removed OnXAxisRangeChanged to prevent automatic updates
    // Changes will only be applied when Set button is clicked
    
    private void SetBothAxisRange()
    {
        // Apply both X-axis and Y-axis range changes when Set button is clicked
        if (Tile != null)
        {
            Console.WriteLine($"SetBothAxisRange: XAxisRange={XAxisRange}, YAxisMin={YAxisMin}, YAxisMax={YAxisMax}");
            Console.WriteLine($"SetBothAxisRange: Tile.IsLiveView={Tile.IsLiveView}, Tile.IsPaused={Tile.IsPaused}");
            
            // Apply X-axis range
            if (XAxisRange > 0)
            {
                Console.WriteLine($"SetBothAxisRange: Setting X-axis range to {XAxisRange}");
                
                // Update the tile's window size
                Tile.WindowSize = XAxisRange;
                
                // Set the manual flag to prevent auto-updates
                Tile.IsManualXAxisRange = true;
                
                // Force update the X-axis limits
                var currentIndex = Tile.GetLatestSampleIndex();
                Console.WriteLine($"SetBothAxisRange: Current sample index = {currentIndex}");
                
                if (currentIndex >= 0)
                {
                    int startIndex = Math.Max(0, currentIndex - XAxisRange + 1);
                    Console.WriteLine($"SetBothAxisRange: Setting X-axis limits from {startIndex} to {currentIndex}");
                    Tile.ForceSetXAxisLimits(startIndex, currentIndex);
                }
                
                // Only call LoadHistoryWindow if we're not in live mode or if the tile is paused
                if (!Tile.IsLiveView || Tile.IsPaused)
                {
                    var liveIndex = Tile.GetLatestSampleIndex();
                    if (liveIndex >= 0)
                    {
                        int startIndex = Math.Max(0, liveIndex - XAxisRange + 1);
                        Tile.LoadHistoryWindow(startIndex);
                    }
                }
            }
            
            // Apply Y-axis range with a small delay for Windows compatibility
            if (YAxisMin < YAxisMax)
            {
                Console.WriteLine($"SetBothAxisRange: Setting Y-axis limits from {YAxisMin} to {YAxisMax}");
                
                // Use a small delay to ensure Windows processes the UI updates properly
                Task.Delay(50).ContinueWith(_ =>
                {
                    Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                    {
                        Tile.SetYAxisLimits(YAxisMin, YAxisMax);
                        Console.WriteLine($"SetBothAxisRange: Y-axis limits applied successfully");
                    });
                });
            }
        }
    }
    
    private void ResetBothAxisRange()
    {
        // Reset both X-axis and Y-axis to automatic updates
        if (Tile != null)
        {
            Console.WriteLine("ResetBothAxisRange: Resetting both X and Y axis ranges");
            
            XAxisRange = 1000;
            YAxisMin = -10000;
            YAxisMax = 10000;
            Tile.WindowSize = XAxisRange;
            Tile.ResetManualXAxisRange();
            
            // Use a small delay for Windows compatibility
            Task.Delay(50).ContinueWith(_ =>
            {
                Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                {
                    Tile.ResetYAxisLimits();
                    Console.WriteLine("ResetBothAxisRange: Y-axis limits reset successfully");
                });
            });
        }
    }
    
    private void Disconnect()
    {
        // Stop the tile and close the connection
        if (Tile != null)
        {
            Tile.Stop();
        }
    }
    
    private void ExecuteSkipToIndex()
    {
        // Jump to a specific index in historical data
        if (Tile != null && int.TryParse(SkipToIndexText, out int index))
        {
            Tile.JumpToIndex(index);
        }
    }
}

public partial class PlottingPageViewModel : ViewModelBase
{


    public ObservableCollection<InputConfig> Configs { get; } = new();
    [ObservableProperty]
    private InputConfig? selectedConfig;

    [ObservableProperty]
    private int activeTileIndex = -1; // -1 means no tile is active

    [ObservableProperty]
    private bool isSidebarCollapsed = false;

    // Active configuration panels for sidebar
    public ObservableCollection<ActiveConfigPanel> ActiveConfigPanels { get; } = new();

    public IRelayCommand ToggleSidebarCommand { get; }

    // Modular tile system
    public ObservableCollection<TileViewModel> Tiles { get; } = new();
    public IRelayCommand<object?> ActivateTileCommand { get; }
    public IRelayCommand AddTileCommand { get; }
    public IRelayCommand<object?> RemoveTileCommand { get; }

    public bool ShowAddTileButton => Tiles.Count < 10; // Allow more tiles for dynamic grid

    private void ActivateTile(object? param)
    {
        if (param == null) return;
        if (int.TryParse(param.ToString(), out int idx) && idx >= 0 && idx < Tiles.Count)
        {
            ActiveTileIndex = idx;
            // Deactivate other tiles
            for (int i = 0; i < Tiles.Count; i++)
            {
                Tiles[i].IsActive = (i == idx);
            }
        }
    }

    private void AddTile()
    {
        if (Tiles.Count < 10) // Match the ShowAddTileButton limit
        {
            var newTile = new TileViewModel();
            newTile.ParentViewModel = this;
            Tiles.Add(newTile);
            OnPropertyChanged(nameof(ShowAddTileButton));
        }
    }

    private void RemoveTile(object? param)
    {
        if (param == null) return;
        if (int.TryParse(param.ToString(), out int idx) && idx >= 0 && idx < Tiles.Count && Tiles.Count > 1)
        {
            var tileToRemove = Tiles[idx];
            tileToRemove.Stop(); // Stop any running acquisitions
            
            // Remove from active config panels if this tile was active
            var panelToRemove = ActiveConfigPanels.FirstOrDefault(p => p.Tile == tileToRemove);
            if (panelToRemove != null)
            {
                ActiveConfigPanels.Remove(panelToRemove);
            }
            
            Tiles.RemoveAt(idx);
            
            // Adjust active tile index
            if (ActiveTileIndex >= Tiles.Count)
                ActiveTileIndex = Tiles.Count - 1;
            
            OnPropertyChanged(nameof(ShowAddTileButton));
        }
    }

    // Method to add a configuration panel when telemetry starts
    public void AddActiveConfigPanel(InputConfig config, TileViewModel tile)
    {
        // Check if this config is already active for this tile
        var existingPanel = ActiveConfigPanels.FirstOrDefault(p => p.Config == config && p.Tile == tile);
        if (existingPanel == null)
        {
            ActiveConfigPanels.Add(new ActiveConfigPanel(config, tile));
        }
    }

    // Method to remove a configuration panel when telemetry stops
    public void RemoveActiveConfigPanel(InputConfig config, TileViewModel tile)
    {
        var panelToRemove = ActiveConfigPanels.FirstOrDefault(p => p.Config == config && p.Tile == tile);
        if (panelToRemove != null)
        {
            ActiveConfigPanels.Remove(panelToRemove);
        }
    }

    public PlottingPageViewModel()
    {
        // Ensure all required directories exist
        AppPaths.EnsureAllDirectoriesExist();
        
        LoadConfigs();
        ActivateTileCommand = new RelayCommand<object?>(ActivateTile);
        AddTileCommand = new RelayCommand(AddTile);
        RemoveTileCommand = new RelayCommand<object?>(RemoveTile);
        ToggleSidebarCommand = new RelayCommand(ToggleSidebar);
        
        // Initialize with one tile (1x2 grid: 1 plottable + 1 add button)
        var initialTile = new TileViewModel();
        initialTile.ParentViewModel = this;
        Tiles.Add(initialTile);
        ActiveTileIndex = 0;
        Tiles[0].IsActive = true;
        
        PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(ActiveTileIndex))
            {
                OnPropertyChanged(nameof(ShowAddTileButton));
            }
        };
        
        AppLogger.LogInfo("PlottingPageViewModel initialized", showMessageBox: false);
    }

    private void ToggleSidebar()
    {
        IsSidebarCollapsed = !IsSidebarCollapsed;
    }

    private void LoadConfigs()
    {
        Configs.Clear();
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
                        Configs.Add(cfg);
                    
                    AppLogger.LogInfo($"Loaded {loaded.Count} configurations", showMessageBox: false);
                }
            }
            catch (Exception ex)
            {
                AppLogger.LogError("Failed to load configurations", ex, showMessageBox: false);
            }
        }
        else
        {
            AppLogger.LogInfo("No configuration file found, starting with empty configs", showMessageBox: false);
        }
    }

    partial void OnSelectedConfigChanged(InputConfig? value)
    {
        if (value != null && ActiveTileIndex >= 0 && ActiveTileIndex < Tiles.Count)
        {
            // Set the selected config to the active tile
            Tiles[ActiveTileIndex].Config = value;
        }
    }
} 