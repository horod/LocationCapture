using LocationCapture.BL;
using LocationCapture.Client.MVVM.Infrastructure;
using LocationCapture.Client.MVVM.Services;

namespace LocationCapture.Client.DotNetMaui;

public partial class App : Application
{
    private readonly ILoggingService _loggingService;
    private readonly INavigationService _navigationService;
    private readonly IAppStateProvider _appStateProvider;

    public App(AppShell shell, ILoggingService loggingService, INavigationService navigationService, IAppStateProvider appStateProvider)
	{
		InitializeComponent();

		MainPage = shell;

        _loggingService = loggingService;
        _navigationService = navigationService;
        _appStateProvider = appStateProvider;
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        Window window = base.CreateWindow(activationState);

        window.Destroying += (s, e) =>
        {
            _loggingService.Warning("The app is being destroyed.");

            window.Stopped -= WindowStopped;
            window.Created -= WindowCreated;
        };

        window.Stopped += WindowStopped;
        window.Created += WindowCreated;

        return window;
    }

    private async void WindowCreated(object sender, EventArgs e)
    {
        try
        {
            var appState = await _appStateProvider.GetAppStateAsync();

            _navigationService.GoTo(appState.CurrentView, appState.NavigationParam);
        }
        catch (Exception ex)
        {
            _loggingService.Error(ex, "Failed to load the app state. Details: {Ex}");
        }
    }

    private async void WindowStopped(object sender, EventArgs e)
    {
        var shell = (AppShell)MainPage;
        var navTarget = shell?.CurrentPage?.BindingContext as INavigationTarget;
        await navTarget?.SaveState();
    }
}
