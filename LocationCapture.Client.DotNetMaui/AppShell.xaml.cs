using LocationCapture.Client.DotNetMaui.Services;
using LocationCapture.Client.MVVM.Infrastructure;

namespace LocationCapture.Client.DotNetMaui;

public partial class AppShell : Shell
{
	public AppShell()
	{
        InitializeComponent();

        NavigationService.RegisterRoutes();
    }

    protected override bool OnBackButtonPressed()
    {
        var navTarget = CurrentPage?.BindingContext as INavigationTarget;
        navTarget?.GoBack();

        return true;
    }
}
