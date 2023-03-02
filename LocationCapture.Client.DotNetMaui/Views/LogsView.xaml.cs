using LocationCapture.Client.DotNetMaui.Views;
using LocationCapture.Client.MVVM.Enums;
using LocationCapture.Client.MVVM.Infrastructure;
using LocationCapture.Client.MVVM.Services;
using LocationCapture.Client.MVVM.ViewModels;
using LocationCapture.Models;

namespace LocationCapture.Client.DotNetMaui.Views;

public partial class LogsView : ViewBase
{
	public LogsView(LogsViewModel viewModel)
	{
		BindingContext = viewModel;

		InitializeComponent();
    }
}

public class LogsViewModel : NotificationBase, INavigationTarget
{
    private readonly INavigationService _navigationService;

    public object NavigationParam { get; set; }

    private string _Logs;
    public string Logs
    {
        get { return _Logs; }
        set { SetProperty(ref _Logs, value); }
    }

    public LogsViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    public void GoBack()
    {
        _navigationService.GoTo(AppViews.Locations, NavigationParam);
    }

    public Task OnNavigatedTo()
    {
        var logsFolder = Path.Combine(
            FileSystem.CacheDirectory,
            "MetroLogs");

        if (Directory.Exists(logsFolder))
        {
            var logFiles = Directory.GetFiles(logsFolder);

            if (logFiles.Any())
            {
                var logs = File.ReadAllText(logFiles[0]);

                Logs = logs;
            }
        }

        Logs = Logs ?? "Nothing has been logged yet.";

        return Task.CompletedTask;
    }
}