using System.Windows;
using System.Windows.Controls;
using AudioPresetSwitcher.Services;
using AudioPresetSwitcher.ViewModels.Windows;
using Wpf.Ui;
using Wpf.Ui.Abstractions;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace AudioPresetSwitcher.Views.Windows;

public partial class MainWindow : INavigationWindow
{
    private readonly WindowService _windows;
    private readonly IContentDialogService _contentDialogService;
    private readonly ISnackbarService _snackbarService;

    public MainWindow(
        MainWindowViewModel viewModel,
        INavigationViewPageProvider navigationViewPageProvider,
        INavigationService navigationService,
        IContentDialogService contentDialogService,
        ISnackbarService snackbarService,
        WindowService windows)
    {
        ViewModel = viewModel;
        DataContext = this;
        _windows = windows;
        _contentDialogService = contentDialogService;
        _snackbarService = snackbarService;

        SystemThemeWatcher.Watch(this);
        InitializeComponent();
        SetPageService(navigationViewPageProvider);
        navigationService.SetNavigationControl(RootNavigation);
        _snackbarService.SetSnackbarPresenter(SnackbarPresenter);
        _contentDialogService.SetDialogHost(RootContentDialog);
    }

    public MainWindowViewModel ViewModel { get; }

    public INavigationView GetNavigation() => RootNavigation;

    public bool Navigate(Type pageType) => RootNavigation.Navigate(pageType);

    public void SetPageService(INavigationViewPageProvider navigationViewPageProvider) =>
        RootNavigation.SetPageProviderService(navigationViewPageProvider);

    public void SetServiceProvider(IServiceProvider serviceProvider)
    {
    }

    public void ShowWindow() => _windows.ShowDashboard();

    public void CloseWindow() => _windows.HideDashboard();

    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
        if (!_windows.ExitRequested)
        {
            e.Cancel = true;
            _windows.HideDashboard();
            return;
        }

        base.OnClosing(e);
    }

    private void OnCloseClicked(object sender, RoutedEventArgs e)
    {
        _windows.HideDashboard();
    }

    private void OnNavigationSelectionChanged(object sender, RoutedEventArgs e)
    {
        if (RootNavigation.SelectedItem is NavigationViewItem { Tag: WindowService.ExitTag })
        {
            _windows.Exit();
        }
    }
}
