﻿using LocationCapture.Client.UWP.Infrastructure;
using LocationCapture.Client.UWP.Services;
using LocationCapture.Client.UWP.Views;
using LocationCapture.DAL.Sqlite;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace LocationCapture.Client.UWP
{
    sealed partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.UnhandledException += OnUnhandledException;
        }

        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {
            await SetUpLogger();

            await RunPendingDbMigrations();

            Frame rootFrame = Window.Current.Content as Frame;

            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                await StatusBar.GetForCurrentView().HideAsync();
            }

            if (rootFrame == null)
            {
                rootFrame = await new Bootstrapper().RunAsync();
                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load suspended app state
                }

                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    rootFrame.Navigate(typeof(LocationsView), e.Arguments);
                }
                ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
                Window.Current.Activate();
            }
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: store application state
            deferral.Complete();
        }

        private async void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            Log.Fatal(e.Exception, "Unhandled exception");
            await new DialogService().ShowAsync("Unhandled exception occurred. The application will terminate.");
            Exit();
        }

        private async Task SetUpLogger()
        {
            const string appFolderName = "LocationCapture";
            const string logFileName = "log.txt";

            var externalDevices = KnownFolders.RemovableDevices;
            var sdCard = (await externalDevices.GetFoldersAsync()).FirstOrDefault();
            StorageFolder rootFolder;
            if(sdCard != null && !sdCard.Attributes.HasFlag(Windows.Storage.FileAttributes.ReadOnly))
            {
                rootFolder = sdCard;
            }
            else
            {
                rootFolder = ApplicationData.Current.LocalCacheFolder;
            }

            var appLogsFolder = await rootFolder.TryGetItemAsync(appFolderName);
            if (appLogsFolder == null) appLogsFolder = await rootFolder.CreateFolderAsync(appFolderName);

            var logFilePath = Path.Combine(appLogsFolder.Path, logFileName);
            Log.Logger = new LoggerConfiguration()
                .WriteTo.RollingFile(logFilePath, Serilog.Events.LogEventLevel.Debug)
                .CreateLogger();
        }

        private async Task RunPendingDbMigrations()
        {
            var appSettings = await new AppSettingsProvider().GetAppSettingsAsync();
            using (var db = new SqliteLocationDbContext(appSettings.DbConnectionString))
            {
                db.Database.Migrate();
            }
        }
    }
}
