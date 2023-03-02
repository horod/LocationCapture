using LocationCapture.BL;
using LocationCapture.Client.DotNetMaui.Services;
using LocationCapture.DAL;
using LocationCapture.Enums;
using LocationCapture.Client.MVVM.Services;

namespace LocationCapture.Client.DotNetMaui.Infrastructure
{
    public class DataServiceFactory : IDataServiceFactory
    {
        private readonly ILocationContextFactory _dataContextFactory;
        private readonly IAppSettingsProvider _appSettingsProvider;
        private readonly IWebClient _webClient;
        private readonly IMiniaturesCache _miniaturesCache;

        public DataServiceFactory(ILocationContextFactory dataContextFactory,
            IAppSettingsProvider appSettingsProvider,
            IWebClient webClient,
            IMiniaturesCache miniaturesCache)
        {
            _dataContextFactory = dataContextFactory;
            _appSettingsProvider = appSettingsProvider;
            _webClient = webClient;
            _miniaturesCache = miniaturesCache;
        }

        public ILocationDataService CreateLocationDataService(DataSourceType dataStorageType)
        {
            switch (dataStorageType)
            {
                case DataSourceType.Local:
                    return new LocationDataService(_dataContextFactory);
                case DataSourceType.Remote:
                    return new LocationDataServiceProxy(_appSettingsProvider, _webClient);
                default:
                    throw new ArgumentOutOfRangeException("Could not recognize the specified data source type.");
            }
        }

        public ILocationSnapshotDataService CreateLocationSnapshotDataService(DataSourceType dataStorageType)
        {
            switch (dataStorageType)
            {
                case DataSourceType.Local:
                    return new LocationSnapshotDataService(_dataContextFactory);
                case DataSourceType.Remote:
                    return new LocationSnapshotDataServiceProxy(_appSettingsProvider, _webClient);
                default:
                    throw new ArgumentOutOfRangeException("Could not recognize the specified data source type.");
            }
        }

        public IPictureService CreatePictureService(DataSourceType dataStorageType)
        {
            switch (dataStorageType)
            {
                case DataSourceType.Local:
                    return new PictureService(_miniaturesCache, CreateLocationSnapshotDataService(DataSourceType.Local));
                case DataSourceType.Remote:
                    return new PictureServiceProxy(_appSettingsProvider, _webClient, _miniaturesCache, CreateLocationSnapshotDataService(DataSourceType.Remote));
                default:
                    throw new ArgumentOutOfRangeException("Could not recognize the specified data source type.");
            }
        }
    }
}
