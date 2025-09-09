using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using Telemetry_demo_Avalonia.Views;

namespace Telemetry_demo_Avalonia.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool isSidebarCollapsed = false;

    [ObservableProperty]
    private object? currentPage;

    public string SidebarIcon => IsSidebarCollapsed ? "☰" : "»";

    partial void OnIsSidebarCollapsedChanged(bool value)
    {
        OnPropertyChanged(nameof(SidebarIcon));
    }

    public IRelayCommand ToggleSidebarCommand { get; }
    public IRelayCommand ShowInputPageCommand { get; }
    public IRelayCommand ShowPlottingPageCommand { get; }
    public IRelayCommand ShowSettingsPageCommand { get; }

    private readonly InputPageViewModel inputPage = new();
    private readonly PlottingPageViewModel plottingPage = new();
    private readonly SettingsPageViewModel settingsPage = new();

    public MainWindowViewModel()
    {
        ToggleSidebarCommand = new RelayCommand(ToggleSidebar);
        ShowInputPageCommand = new RelayCommand(() => CurrentPage = inputPage);
        ShowPlottingPageCommand = new RelayCommand(() => CurrentPage = plottingPage);
        ShowSettingsPageCommand = new RelayCommand(() => CurrentPage = settingsPage);
        CurrentPage = inputPage;
    }

    private void ToggleSidebar()
    {
        IsSidebarCollapsed = !IsSidebarCollapsed;
    }
}
