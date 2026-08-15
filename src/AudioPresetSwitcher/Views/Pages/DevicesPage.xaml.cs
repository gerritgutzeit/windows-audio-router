using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
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
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    public DevicesViewModel ViewModel { get; }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            if (FindViewport() is not { ActualHeight: > 0 } viewport)
            {
                return;
            }

            SetBinding(MaxHeightProperty, new Binding(nameof(ActualHeight)) { Source = viewport });
        }
        catch
        {
            BindingOperations.ClearBinding(this, MaxHeightProperty);
        }
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        BindingOperations.ClearBinding(this, MaxHeightProperty);
    }

    private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (e.Handled || DeviceListScrollViewer is null)
        {
            return;
        }

        var target = DeviceListScrollViewer.ScrollableHeight > 0
            ? DeviceListScrollViewer
            : FindViewport() as ScrollViewer;

        if (target is null || target.ScrollableHeight <= 0)
        {
            return;
        }

        e.Handled = true;
        target.RaiseEvent(
            new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
            {
                RoutedEvent = MouseWheelEvent,
                Source = target
            });
    }

    private FrameworkElement? FindViewport()
    {
        DependencyObject current = this;
        while (VisualTreeHelper.GetParent(current) is { } parent)
        {
            if (parent is ScrollViewer scrollViewer)
            {
                return scrollViewer;
            }

            current = parent;
        }

        return Parent as FrameworkElement;
    }
}
