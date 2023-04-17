using LocationCapture.BL;
using LocationCapture.Client.DotNetMaui.Services;
using LocationCapture.Client.MVVM.Services;
using LocationCapture.Client.MVVM.ViewModels;
using LocationCapture.Client.DotNetMaui.Views;
using Prism.Events;
using LocationCapture.DAL;
using LocationCapture.DAL.Sqlite2;

namespace LocationCapture.Client.DotNetMaui.Infrastructure
{
    public static class RegistrationHelpers
    {
        public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddTransient<LocationsViewModel>();
            mauiAppBuilder.Services.AddTransient<SnapshotsViewModel>();
            mauiAppBuilder.Services.AddTransient<SnapshotDetailsViewModel>();
            mauiAppBuilder.Services.AddTransient<CameraViewModel>();
            mauiAppBuilder.Services.AddTransient<GeolocationViewModel>();
            mauiAppBuilder.Services.AddTransient<WeatherViewModel>();
            mauiAppBuilder.Services.AddTransient<PropertiesViewModel>();
            mauiAppBuilder.Services.AddTransient<LogsViewModel>();
            mauiAppBuilder.Services.AddTransient<SuggestionsViewModel>();

            return mauiAppBuilder;
        }

        public static MauiAppBuilder RegisterAppServices(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddTransient<ILocationContextFactory, LocationContextFactory>();
            mauiAppBuilder.Services.AddTransient<IDataServiceFactory, DataServiceFactory>();
            mauiAppBuilder.Services.AddTransient<LocationDataService>();
            mauiAppBuilder.Services.AddTransient<LocationDataServiceProxy>();
            mauiAppBuilder.Services.AddTransient<ILocationDataService, LocationDataServiceCaller>();
            mauiAppBuilder.Services.AddTransient<LocationSnapshotDataService>();
            mauiAppBuilder.Services.AddTransient<LocationSnapshotDataServiceProxy>();
            mauiAppBuilder.Services.AddTransient<ILocationSnapshotDataService, LocationSnapshotDataServiceCaller>();
            mauiAppBuilder.Services.AddTransient<PictureService>();
            mauiAppBuilder.Services.AddTransient<PictureServiceProxy>();
            mauiAppBuilder.Services.AddTransient<IPictureService, PictureServiceCaller>();
            mauiAppBuilder.Services.AddSingleton<INavigationService, NavigationService>();
            mauiAppBuilder.Services.AddTransient<IDialogService, DialogService>();
            mauiAppBuilder.Services.AddTransient<ILocationDataImporter, LocationDataImporter>();
            mauiAppBuilder.Services.AddTransient<IDataSourceGovernor, DataSourceGovernor>();
            mauiAppBuilder.Services.AddTransient<IConnectivityService, ConnectivityService>();
            mauiAppBuilder.Services.AddTransient<IPlatformSpecificActions, PlatformSpecificActions>();
            mauiAppBuilder.Services.AddTransient<IBitmapConverter, BitmapConverter>();
            mauiAppBuilder.Services.AddTransient<ICameraService, CameraService>();
            mauiAppBuilder.Services.AddTransient<ILocationService, LocationService>();
            mauiAppBuilder.Services.AddTransient<IMapService, MapService>();
            mauiAppBuilder.Services.AddSingleton<IEventAggregator, EventAggregator>();
            mauiAppBuilder.Services.AddSingleton<IAppSettingsProvider, AppSettingsProvider>();
            mauiAppBuilder.Services.AddSingleton<IAppStateProvider, AppStateProvider>();
            mauiAppBuilder.Services.AddTransient<IWeatherDataService, WeatherDataService>();
            mauiAppBuilder.Services.AddTransient<ILoggingService, LoggingService>();
            mauiAppBuilder.Services.AddTransient<IWebClient, WebClient>();
            mauiAppBuilder.Services.AddTransient<ISuggestionsService, SuggestionsService>();
            mauiAppBuilder.Services.AddSingleton<IMiniaturesCache, MiniaturesCache>();

            return mauiAppBuilder;
        }

        public static MauiAppBuilder RegisterViews(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddTransient<AppShell>();
            mauiAppBuilder.Services.AddTransient<LocationsView>();
            mauiAppBuilder.Services.AddTransient<SnapshotsView>();
            mauiAppBuilder.Services.AddTransient<SnapshotDetailsView>();
            mauiAppBuilder.Services.AddTransient<CameraView>();
            mauiAppBuilder.Services.AddTransient<WeatherView>();
            mauiAppBuilder.Services.AddTransient<PropertiesView>();
            mauiAppBuilder.Services.AddTransient<LogsView>();
            mauiAppBuilder.Services.AddTransient<SuggestionsView>();

            return mauiAppBuilder;
        }
    }
}
