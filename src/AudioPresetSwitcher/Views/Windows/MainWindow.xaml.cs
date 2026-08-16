using System.Windows;
using AudioPresetSwitcher.Services;
using AudioPresetSwitcher.ViewModels.Windows;
using AudioPresetSwitcher.Views.Pages;
using Wpf.Ui;
using Wpf.Ui.Abstractions;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace AudioPresetSwitcher.Views.Windows;

public partial class MainWindow : INavigationWindow
{
    private readonly IWindowService _windows;
    private readonly IContentDialogService _contentDialogService;
    private readonly ISnackbarService _snackbarService;

    private readonly IThemeSettingsService _theme;

    public MainWindow(
        MainWindowViewModel viewModel,
        CurrentAudioStatusViewModel statusViewModel,
        INavigationViewPageProvider navigationViewPageProvider,
        INavigationService navigationService,
        IContentDialogService contentDialogService,
        ISnackbarService snackbarService,
        IWindowService windows,
        IThemeSettingsService theme)
    {
        ViewModel = viewModel;
        StatusViewModel = statusViewModel;
        DataContext = this;
        _windows = windows;
        _contentDialogService = contentDialogService;
        _snackbarService = snackbarService;
        _theme = theme;

        // Keep studio brass; ThemeSettingsService re-applies accent on theme changes.
        SystemThemeWatcher.Watch(this, WindowBackdropType.Mica, updateAccents: false);
        InitializeComponent();
        CurrentAudioStatus.DataContext = StatusViewModel;
        SetPageService(navigationViewPageProvider);
        navigationService.SetNavigationControl(RootNavigation);
        _snackbarService.SetSnackbarPresenter(SnackbarPresenter);
        _contentDialogService.SetDialogHost(RootContentDialog);

        // Navigate only after the NavigationView template is applied; early Navigate NREs.
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnLoaded;
        _theme.Refresh();
        _ = Navigate(typeof(PresetsPage));
    }

    public MainWindowViewModel ViewModel { get; }

    public CurrentAudioStatusViewModel StatusViewModel { get; }

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

    protected override void OnStateChanged(EventArgs e)
    {
        base.OnStateChanged(e);
        if (WindowState == WindowState.Minimized && !_windows.ExitRequested)
        {
            _windows.HideDashboard();
        }
    }

    private void OnCloseClicked(object sender, RoutedEventArgs e)
    {
        _windows.HideDashboard();
    }
}
