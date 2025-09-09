using Avalonia.Controls;
using LiveChartsCore.Measure;
using Avalonia.Input;
using Telemetry_demo_Avalonia.ViewModels;
using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Layout;
using Avalonia.Media;
using System.Linq;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.Defaults;

namespace Telemetry_demo_Avalonia.Views;

public partial class PlottingPageView : UserControl
{
    private Grid? _tileGrid;
    private Dictionary<int, Border> _tileBorders = new();
    private Border? _addTileButton;
    private List<GridSplitter> _gridSplitters = new();

    public PlottingPageView()
    {
        InitializeComponent();
        _tileGrid = this.FindControl<Grid>("TileGrid");
        if (_tileGrid == null)
        {
            throw new InvalidOperationException("TileGrid not found in XAML");
        }
        
        // Create initial add tile button
        CreateAddTileButton();
        
        // Set up initial 2x2 grid (will be adjusted dynamically)
        SetupGrid(2, 2);
        
        // Subscribe to DataContext changes
        this.DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is PlottingPageViewModel vm)
        {
            // Subscribe to tiles collection changes
            vm.Tiles.CollectionChanged += OnTilesCollectionChanged;
            
            // Initial setup
            UpdateGridLayout();
        }
    }

    private void OnTilesCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        UpdateGridLayout();
    }

    private void UpdateGridLayout()
    {
        if (DataContext is not PlottingPageViewModel vm || _tileGrid == null || _addTileButton == null) return;

        int tileCount = vm.Tiles.Count;
        
        // Dynamic grid layout logic
        int columns, rows;
        if (tileCount == 1)
        {
            // 1 tile: 1x2 layout
            columns = 1;
            rows = 2;
        }
        else if (tileCount == 2)
        {
            // 2 tiles: 2x2 layout
            columns = 2;
            rows = 2;
        }
        else if (tileCount == 3)
        {
            // 3 tiles: 2x3 layout
            columns = 2;
            rows = 3;
        }
        else if (tileCount == 4)
        {
            // 4 tiles: 2x2 layout
            columns = 2;
            rows = 2;
        }
        else
        {
            // 5+ tiles: expand to 2x3, 2x4, etc.
            columns = 2;
            rows = (tileCount + 1 + 1) / 2; // +1 for add button, +1 for ceiling division
        }
        
        // For 1x2 layout, ensure we have at least 1 row
        if (rows < 1) rows = 1;
        
        SetupGrid(rows, columns);
        
        // Only remove tiles that no longer exist
        var tilesToRemove = _tileBorders.Keys.Where(key => key >= tileCount).ToList();
        foreach (var key in tilesToRemove)
        {
            if (_tileBorders.TryGetValue(key, out var border))
            {
                _tileGrid.Children.Remove(border);
                _tileBorders.Remove(key);
            }
        }
        
        // Remove existing splitters
        foreach (var splitter in _gridSplitters)
        {
            _tileGrid.Children.Remove(splitter);
        }
        _gridSplitters.Clear();
        
        // Add only new tiles (preserve existing ones)
        for (int i = 0; i < tileCount; i++)
        {
            if (!_tileBorders.ContainsKey(i))
            {
                // Only create new tile borders for tiles that don't exist
                var tileBorder = CreateTileBorder(vm.Tiles[i], i);
                
                // Calculate position based on dynamic layout
                int row, col;
                if (tileCount == 1)
                {
                    // 1 tile: spans entire first row
                    row = 0;
                    col = 0;
                    Grid.SetColumnSpan(tileBorder, 2);
                }
                else if (tileCount == 2)
                {
                    // 2 tiles: first spans full width, second in second row
                    if (i == 0)
                    {
                        row = 0;
                        col = 0;
                        Grid.SetColumnSpan(tileBorder, 2);
                    }
                    else
                    {
                        row = 1;
                        col = 0;
                    }
                }
                else if (tileCount == 3)
                {
                    // 3 tiles: 2x3 layout
                    if (i == 0)
                    {
                        // First tile spans entire first row
                        row = 0;
                        col = 0;
                        Grid.SetColumnSpan(tileBorder, 2);
                    }
                    else if (i == 1)
                    {
                        // Second tile in second row, first column
                        row = 1;
                        col = 0;
                    }
                    else // i == 2
                    {
                        // Third tile in second row, second column
                        row = 1;
                        col = 1;
                    }
                }
                else if (tileCount == 4)
                {
                    // 4 tiles: first spans full width, rest in second row
                    if (i == 0)
                    {
                        row = 0;
                        col = 0;
                        Grid.SetColumnSpan(tileBorder, 2);
                    }
                    else
                    {
                        row = 1;
                        col = i - 1;
                    }
                }
                else
                {
                    // 5+ tiles: expand to multiple rows
                    row = i / 2;
                    col = i % 2;
                }
                
                Grid.SetRow(tileBorder, row);
                Grid.SetColumn(tileBorder, col);
                _tileGrid.Children.Add(tileBorder);
                _tileBorders[i] = tileBorder;
            }
            else
            {
                // Update position of existing tiles
                var tileBorder = _tileBorders[i];
                
                // Calculate position based on dynamic layout
                int row, col;
                if (tileCount == 1)
                {
                    // 1 tile: spans entire first row
                    row = 0;
                    col = 0;
                    Grid.SetColumnSpan(tileBorder, 2);
                }
                else if (tileCount == 2)
                {
                    // 2 tiles: first spans full width, second in second row
                    if (i == 0)
                    {
                        row = 0;
                        col = 0;
                        Grid.SetColumnSpan(tileBorder, 2);
                    }
                    else
                    {
                        row = 1;
                        col = 0;
                        Grid.SetColumnSpan(tileBorder, 1);
                    }
                }
                else if (tileCount == 3)
                {
                    // 3 tiles: 2x3 layout
                    if (i == 0)
                    {
                        // First tile spans entire first row
                        row = 0;
                        col = 0;
                        Grid.SetColumnSpan(tileBorder, 2);
                    }
                    else if (i == 1)
                    {
                        // Second tile in second row, first column
                        row = 1;
                        col = 0;
                        Grid.SetColumnSpan(tileBorder, 1);
                    }
                    else // i == 2
                    {
                        // Third tile in second row, second column
                        row = 1;
                        col = 1;
                        Grid.SetColumnSpan(tileBorder, 1);
                    }
                }
                else if (tileCount == 4)
                {
                    // 4 tiles: first spans full width, rest in second row
                    if (i == 0)
                    {
                        row = 0;
                        col = 0;
                        Grid.SetColumnSpan(tileBorder, 2);
                    }
                    else
                    {
                        row = 1;
                        col = i - 1;
                        Grid.SetColumnSpan(tileBorder, 1);
                    }
                }
                else
                {
                    // 5+ tiles: expand to multiple rows
                    row = i / 2;
                    col = i % 2;
                    Grid.SetColumnSpan(tileBorder, 1);
                }
                
                Grid.SetRow(tileBorder, row);
                Grid.SetColumn(tileBorder, col);
            }
        }
        
        // No splitters - clean layout
        // AddGridSplitters(rows, columns);
        
        // Update the add button position
        int addButtonRow, addButtonCol;
        if (tileCount == 1)
        {
            // Add button in second row, first column
            addButtonRow = 1;
            addButtonCol = 0;
        }
        else if (tileCount == 2)
        {
            // Add button in second row, second column
            addButtonRow = 1;
            addButtonCol = 1;
        }
        else if (tileCount == 3)
        {
            // Add button in third row, first column
            addButtonRow = 2;
            addButtonCol = 0;
        }
        else if (tileCount == 4)
        {
            // Add button in second row, second column
            addButtonRow = 1;
            addButtonCol = 1;
        }
        else
        {
            // 5+ tiles: Add button in last row, last column
            addButtonRow = rows - 1;
            addButtonCol = 1;
        }
        
        Grid.SetRow(_addTileButton, addButtonRow);
        Grid.SetColumn(_addTileButton, addButtonCol);
        
        // Make sure add button is in the grid
        if (!_tileGrid.Children.Contains(_addTileButton))
        {
            _tileGrid.Children.Add(_addTileButton);
        }
        
        // Ensure add button is visible and enabled
        if (_addTileButton?.Child is Button addButton)
        {
            addButton.IsEnabled = vm.ShowAddTileButton;
        }
    }

    private void SetupGrid(int rows, int columns)
    {
        if (_tileGrid == null) return;
        
        _tileGrid.RowDefinitions.Clear();
        _tileGrid.ColumnDefinitions.Clear();
        
        // Simple grid without splitters
        for (int i = 0; i < rows; i++)
        {
            _tileGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star), MinHeight = 150 });
        }
        
        for (int i = 0; i < columns; i++)
        {
            _tileGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star), MinWidth = 200 });
        }
    }



    private void ShowConfigSelection(TileViewModel tile, Button plotButton, Grid grid)
    {
        // Remove the plot button
        grid.Children.Remove(plotButton);
        
        // Create config selection panel
        var configPanel = new StackPanel
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        
        var configCombo = new ComboBox
        {
            Width = 200,
            Margin = new Thickness(0, 0, 0, 12),
            PlaceholderText = "Select Configuration..."
        };
        
        var startButton = new Button
        {
            Content = "Start",
            Width = 120,
            Height = 36,
            FontWeight = Avalonia.Media.FontWeight.Bold,
            Background = new SolidColorBrush(Color.Parse("#4F8EF7")),
            Foreground = new SolidColorBrush(Colors.White),
            IsEnabled = false
        };
        
        if (DataContext is PlottingPageViewModel vm)
        {
            configCombo.ItemsSource = vm.Configs;
            configCombo.SelectedItem = tile.Config;
            configCombo.SelectionChanged += (s, e) => 
            {
                tile.Config = configCombo.SelectedItem as InputConfig;
                if (tile.Config != null)
                {
                    startButton.IsEnabled = true;
                }
            };
        }
        
        startButton.Click += (s, e) => 
        {
            if (tile.Config != null)
            {
                // Hide config panel
                grid.Children.Remove(configPanel);
                
                // Show chart and controls
                ShowChartAndControls(tile, grid);
                
                // Start the tile (this will set IsStarted = true and allow data to be displayed)
                tile.StartCommand.Execute(null);
            }
        };
        
        // Subscribe to tile's IsStarted property to update header when stopped
        tile.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(TileViewModel.IsStarted) && !tile.IsStarted)
            {
                // Reset header when tile is stopped
                foreach (var child in grid.Children)
                {
                    if (child is Grid headerGrid && headerGrid.Children.Count > 0 && 
                        headerGrid.Children[0] is TextBlock titleText)
                    {
                        titleText.Text = "Tile";
                        headerGrid.Background = new SolidColorBrush(Color.Parse("#4F8EF7")); // Reset to default blue
                        break;
                    }
                }
            }
        };
        
        configPanel.Children.Add(configCombo);
        configPanel.Children.Add(startButton);
        
        grid.Children.Add(configPanel);
        Grid.SetRow(configPanel, 1);
    }

    private void ShowChartAndControls(TileViewModel tile, Grid grid)
    {
        Console.WriteLine($"ShowChartAndControls: Showing chart for tile with config: {tile.Config?.ConnectionName}");
        
        // Find and show the chart
        foreach (var child in grid.Children)
        {
            if (child is LiveChartsCore.SkiaSharpView.Avalonia.CartesianChart chart)
            {
                chart.IsVisible = true;
                Console.WriteLine($"ShowChartAndControls: Chart made visible, Series count: {tile.Series.Count}");
                break;
            }
        }
        
        // Find and show the controls
        foreach (var child in grid.Children)
        {
            if (child is StackPanel panel && panel.Children.Count > 0 && 
                panel.Children[0] is Button btn && btn.Content?.ToString() == "⏸")
            {
                panel.IsVisible = true;
                break;
            }
        }
        
        // Update the title to show the config name
        foreach (var child in grid.Children)
        {
            if (child is Grid headerGrid && headerGrid.Children.Count > 0 && 
                headerGrid.Children[0] is TextBlock titleText)
            {
                string configName = tile.Config?.ConnectionName ?? "Tile";
                titleText.Text = configName;
                // Make the header more prominent when a config is active
                headerGrid.Background = new SolidColorBrush(Color.Parse("#2E7D32")); // Green background for active config
                break;
            }
        }
        
        // Note: Chart should now be visible and ready to display data
        // The tile.StartCommand.Execute(null) will set IsStarted = true
        // which allows the data acquisition loop to add data to the chart
    }



    private Border CreateTileBorder(TileViewModel tile, int index)
    {
        var border = new Border
        {
            Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#F5F6FA")),
            BorderBrush = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#4F8EF7")),
            BorderThickness = new Thickness(2),
            CornerRadius = new CornerRadius(12),
            Margin = new Thickness(4),
            MinWidth = 200,
            MinHeight = 150
        };

        var grid = new Grid();
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Header
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // Content
        
        // Tile header
        var headerGrid = new Grid
        {
            Height = 40,
            Background = new SolidColorBrush(Color.Parse("#4F8EF7"))
        };
        
        var titleText = new TextBlock
        {
            Text = "Tile",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            FontWeight = Avalonia.Media.FontWeight.Bold,
            Foreground = new SolidColorBrush(Colors.White),
            FontSize = 14
        };
        headerGrid.Children.Add(titleText);
        
        // Remove button
        var removeButton = new Button
        {
            Content = "×",
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center,
            Width = 30,
            Height = 30,
            Margin = new Thickness(4),
            FontWeight = Avalonia.Media.FontWeight.Bold
        };
        removeButton.Click += (s, e) => RemoveTile(index);
        headerGrid.Children.Add(removeButton);
        
        grid.Children.Add(headerGrid);
        Grid.SetRow(headerGrid, 0);
        
        // Initial Plot button (shown when no config is selected)
        var plotButton = new Button
        {
            Content = "Plot",
            Width = 120,
            Height = 40,
            FontWeight = Avalonia.Media.FontWeight.Bold,
            FontSize = 16,
            Background = new SolidColorBrush(Color.Parse("#4F8EF7")),
            Foreground = new SolidColorBrush(Colors.White),
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        
        plotButton.Click += (s, e) => ShowConfigSelection(tile, plotButton, grid);
        
        grid.Children.Add(plotButton);
        Grid.SetRow(plotButton, 1); // Plot button goes in the content row (row 1)
        
        // Chart (initially hidden)
        var chart = new LiveChartsCore.SkiaSharpView.Avalonia.CartesianChart
        {
            Series = tile.Series,
            XAxes = tile.XAxes,
            YAxes = tile.YAxes,
            ZoomMode = tile.ChartZoomMode,
            IsVisible = false
        };
        
        // Subscribe to Series changes to force chart refresh
        tile.Series.CollectionChanged += (s, e) => {
            Console.WriteLine($"Chart Series changed: {e.Action}, Count: {tile.Series.Count}");
            // Force chart refresh by triggering a property change
            Avalonia.Threading.Dispatcher.UIThread.Post(() => {
                chart.InvalidateArrange();
                chart.InvalidateMeasure();
            });
        };
        
        grid.Children.Add(chart);
        Grid.SetRow(chart, 1); // Chart goes in the content row (row 1)
        
        // Tile controls overlay (initially hidden)
        var controlsPanel = new StackPanel
        {
            VerticalAlignment = VerticalAlignment.Top,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(8),
            IsVisible = false
        };
        
        var pauseButton = new Button
        {
            Content = "⏸",
            Width = 30,
            Height = 30
        };
        pauseButton.Click += (s, e) => tile.PauseCommand.Execute(null);
        
        var resumeButton = new Button
        {
            Content = "▶",
            Width = 30,
            Height = 30,
            Margin = new Thickness(0, 4, 0, 0)
        };
        resumeButton.Click += (s, e) => tile.ResumeCommand.Execute(null);
        
        controlsPanel.Children.Add(pauseButton);
        controlsPanel.Children.Add(resumeButton);
        
        grid.Children.Add(controlsPanel);
        Grid.SetRow(controlsPanel, 1); // Controls overlay goes in the content row (row 1)
        
        border.Child = grid;
        return border;
    }

    private void CreateAddTileButton()
    {
        _addTileButton = new Border
        {
            Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#F5F6FA")),
            BorderBrush = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#4F8EF7")),
            BorderThickness = new Thickness(2),
            CornerRadius = new CornerRadius(12),
            Margin = new Thickness(4)
        };

        var addButton = new Button
        {
            Content = "+",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            FontWeight = Avalonia.Media.FontWeight.Bold,
            FontSize = 28,
            Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#4F8EF7")),
            Foreground = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Colors.White),
            BorderThickness = new Thickness(0),
            Width = 60,
            Height = 60
        };
        
        addButton.Click += (s, e) => AddTile();
        _addTileButton.Child = addButton;
    }

    private void AddTile()
    {
        if (DataContext is PlottingPageViewModel vm)
        {
            vm.AddTileCommand.Execute(null);
        }
    }

    private void RemoveTile(int index)
    {
        if (DataContext is PlottingPageViewModel vm)
        {
            vm.RemoveTileCommand.Execute(index);
        }
    }
} 