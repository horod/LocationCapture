using LocationCapture.BL;
using LocationCapture.Client.UWP.Factories;
using LocationCapture.Client.UWP.Services;
using LocationCapture.Client.MVVM.Services;
using LocationCapture.DAL;
using LocationCapture.DAL.Sqlite2;
using Microsoft.Practices.Unity;
using Prism.Events;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace LocationCapture.Client.UWP.Infrastructure
{
    public class Bootstrapper
    {
        public async Task<Frame> RunAsync()
        {
            var container = new UnityContainer();
            var frame = new DiFrame(container);

            container.RegisterType<IDataSourceGovernor, DataSourceGovernor>(new ContainerControlledLifetimeManager(), new InjectionConstructor(container));
            container.RegisterType<INavigationService, NavigationService>(new ContainerControlledLifetimeManager(), new InjectionConstructor(frame));
            container.RegisterType<ILocationContextFactory, LocationContextFactory>();
            container.RegisterType<IBitmapConverter, BitmapConverter>();
            container.RegisterType<ICameraService, CameraService>();
            container.RegisterType<ILocationService, LocationService>();
            container.RegisterType<IDialogService, DialogService>();
            container.RegisterType<IMapService, MapService>();
            container.RegisterType<IWebClient, WebClient>();
            container.RegisterType<IConnectivityService, ConnectivityService>();
            container.RegisterType<IWeatherDataService, WeatherDataService>();
            container.RegisterType<IAppSettingsProvider, AppSettingsProvider>(new ContainerControlledLifetimeManager());
            container.RegisterType<IAppStateProvider, AppStateProvider>(new ContainerControlledLifetimeManager());
            container.RegisterType<ICameraRotationHelper, CameraRotationHelper>();
            container.RegisterType<IEventAggregator, EventAggregator>(new ContainerControlledLifetimeManager());
            container.RegisterType<IDataServiceFactory, DataServiceFactory>();
            container.RegisterType<ILocationDataImporter, LocationDataImporter>();
            container.RegisterType<ISnapshotPackageManager, SnapshotPackageManager>();
            container.RegisterType<IFilePickerService, FilePickerService>();
            container.RegisterType<IMiniaturesCache, MiniaturesCache>(new ContainerControlledLifetimeManager());
            container.RegisterType<ILoggingService, LoggingService>();
            container.RegisterType<IPlatformSpecificActions, PlatformSpecificActions>();

            await container.Resolve<IDataSourceGovernor>().ChooseDataSourceAsync();

            return frame;
        }
    }
}
