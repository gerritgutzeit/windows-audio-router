using System.Windows.Input;
using AudioPresetSwitcher.ViewModels.Windows;

namespace AudioPresetSwitcher.Views.Controls;

public partial class CurrentAudioStatusView
{
    public CurrentAudioStatusView()
    {
        InitializeComponent();
    }

    private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is CurrentAudioStatusViewModel viewModel &&
            viewModel.OpenLiveStatusCommand.CanExecute(null))
        {
            viewModel.OpenLiveStatusCommand.Execute(null);
            e.Handled = true;
        }
    }
}
