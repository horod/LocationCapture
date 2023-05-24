using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;

namespace LocationCapture.Client.DotNetMaui
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;

            Window.AddFlags(WindowManagerFlags.Fullscreen);
            Window.ClearFlags(WindowManagerFlags.ForceNotFullscreen);
        }

        private static void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
        {
            var newExc = new Exception("TaskSchedulerOnUnobservedTaskException", unobservedTaskExceptionEventArgs.Exception);
            LogUnhandledException(newExc);
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            var newExc = new Exception("CurrentDomainOnUnhandledException", unhandledExceptionEventArgs.ExceptionObject as Exception);
            LogUnhandledException(newExc);
        }

        internal static void LogUnhandledException(Exception exception)
        {
            try
            {
                var errorMessage = String.Format("\r\nTime: {0}\r\nError: Unhandled Exception\r\n{1}",
                    DateTime.Now, exception.ToString());

                var logsFolder = Path.Combine(
                    FileSystem.CacheDirectory,
                    "MetroLogs");

                if (Directory.Exists(logsFolder))
                {
                    var logFiles = Directory.GetFiles(logsFolder)
                        .OrderByDescending(x => x)
                        .ToList();

                    if (logFiles.Any())
                    {
                        File.AppendAllText(logFiles.First(), errorMessage);
                    }
                }
            }
            catch
            {
                // just suppress any error logging exceptions
            }
        }
    }
}