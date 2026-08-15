using AudioPresetSwitcher.ViewModels.Pages;
using Wpf.Ui.Abstractions.Controls;

namespace AudioPresetSwitcher.Views.Pages;

public partial class DevicesPage : INavigableView<DevicesViewModel>
{
    public DevicesPage(DevicesViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;
        InitializeComponent();
    }

    public DevicesViewModel ViewModel { get; }
}
