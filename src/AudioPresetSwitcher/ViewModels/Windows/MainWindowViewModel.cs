using AudioPresetSwitcher.Services;
using AudioPresetSwitcher.Views.Pages;
using Wpf.Ui.Controls;

namespace AudioPresetSwitcher.ViewModels.Windows;

public partial class MainWindowViewModel : ObservableObject
{
    public MainWindowViewModel(IWindowService windows)
    {
        ApplicationTitle = AppIdentity.Name;

        MenuItems =
        [
            new NavigationViewItem
            {
                Content = "Presets",
                Icon = new SymbolIcon { Symbol = SymbolRegular.HeadphonesSoundWave24 },
                TargetPageType = typeof(PresetsPage)
            },
            // Nav label "Live Status"; page type remains DevicesPage (routes/DI unchanged).
            new NavigationViewItem
            {
                Content = "Live Status",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Pulse24 },
                TargetPageType = typeof(DevicesPage)
            },
            new NavigationViewItem
            {
                Content = "Settings",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
                TargetPageType = typeof(SettingsPage)
            }
        ];

        FooterMenuItems =
        [
            new NavigationViewItem
            {
                Content = "Exit",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Power24 },
                Command = new RelayCommand(windows.Exit)
            }
        ];
    }

    [ObservableProperty]
    private string _applicationTitle = AppIdentity.Name;

    [ObservableProperty]
    private ObservableCollection<NavigationViewItem> _menuItems = [];

    [ObservableProperty]
    private ObservableCollection<NavigationViewItem> _footerMenuItems = [];
}
