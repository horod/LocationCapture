namespace LocationCapture.Client.DotNetMaui;

public partial class App : Application
{
	public App(AppShell shell)
	{
		InitializeComponent();

		MainPage = shell;
	}
}
