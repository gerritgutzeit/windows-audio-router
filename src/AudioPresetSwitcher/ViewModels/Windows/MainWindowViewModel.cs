using System.Windows.Media;
using AudioPresetSwitcher.Services;
using AudioPresetSwitcher.Views.Pages;
using Wpf.Ui.Controls;

namespace AudioPresetSwitcher.ViewModels.Windows;

public partial class MainWindowViewModel : ObservableObject
{
    public MainWindowViewModel()
    {
        ApplicationTitle = "AudioPresetSwitcher";

        MenuItems =
        [
            new NavigationViewItem
            {
                Content = "Presets",
                Icon = new SymbolIcon { Symbol = SymbolRegular.HeadphonesSoundWave24 },
                TargetPageType = typeof(PresetsPage)
            },
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
                Tag = WindowService.ExitTag
            }
        ];
    }

    [ObservableProperty]
    private string _applicationTitle = "AudioPresetSwitcher";

    [ObservableProperty]
    private ObservableCollection<object> _menuItems = [];

    [ObservableProperty]
    private ObservableCollection<object> _footerMenuItems = [];
}
