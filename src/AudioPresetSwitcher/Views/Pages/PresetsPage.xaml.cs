using System.Windows;
using System.Windows.Controls;
using AudioPresetSwitcher.ViewModels;
using AudioPresetSwitcher.ViewModels.Pages;
using Wpf.Ui.Abstractions.Controls;

namespace AudioPresetSwitcher.Views.Pages;

public partial class PresetsPage : INavigableView<PresetsViewModel>
{
    public PresetsPage(PresetsViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;
        InitializeComponent();
    }

    public PresetsViewModel ViewModel { get; }

    private void OnMoreClick(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement element || element.ContextMenu is null)
        {
            return;
        }

        element.ContextMenu.DataContext = element.DataContext;
        element.ContextMenu.PlacementTarget = element;
        element.ContextMenu.IsOpen = true;
    }

    private async void OnEditClick(object sender, RoutedEventArgs e)
    {
        if (FindCard(sender) is { } card)
        {
            await ViewModel.EditCommand.ExecuteAsync(card);
        }
    }

    private void OnDuplicateClick(object sender, RoutedEventArgs e)
    {
        if (FindCard(sender) is { } card)
        {
            ViewModel.DuplicateCommand.Execute(card);
        }
    }

    private void OnCreateShortcutClick(object sender, RoutedEventArgs e)
    {
        if (FindCard(sender) is { } card)
        {
            ViewModel.CreateShortcutCommand.Execute(card);
        }
    }

    private async void OnDeleteClick(object sender, RoutedEventArgs e)
    {
        if (FindCard(sender) is { } card)
        {
            await ViewModel.DeleteCommand.ExecuteAsync(card);
        }
    }

    private static PresetCardViewModel? FindCard(object sender)
    {
        if (sender is FrameworkElement { DataContext: PresetCardViewModel card })
        {
            return card;
        }

        if (sender is MenuItem { Parent: ContextMenu { DataContext: PresetCardViewModel fromMenu } })
        {
            return fromMenu;
        }

        return null;
    }
}
