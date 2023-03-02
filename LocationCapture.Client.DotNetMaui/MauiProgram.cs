using LocationCapture.Client.DotNetMaui.Infrastructure;
using MetroLog.MicrosoftExtensions;

namespace LocationCapture.Client.DotNetMaui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .RegisterViewModels()
            .RegisterAppServices()
            .RegisterViews()
            .Logging.AddStreamingFileLogger(opts =>
            {
                opts.RetainDays = 2;
                opts.FolderPath = Path.Combine(
                    FileSystem.CacheDirectory,
                    "MetroLogs");
            });

        return builder.Build();
	}
}
