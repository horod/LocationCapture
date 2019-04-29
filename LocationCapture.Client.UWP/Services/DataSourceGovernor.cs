using LocationCapture.BL;
using LocationCapture.Enums;
using Microsoft.Practices.Unity;
using System;
using System.Threading.Tasks;
using LocationCapture.Client.MVVM.Services;

namespace LocationCapture.Client.UWP.Services
{
    public class DataSourceGovernor : IDataSourceGovernor
    {
        private readonly IUnityContainer _container;

        [Dependency]
        public IAppSettingsProvider AppSettingsProvider { get; set; }

        public DataSourceGovernor(IUnityContainer container)
        {
            _container = container;
        }

        public async Task ChooseDataSourceAsync()
        {
            var appSettings = await AppSettingsProvider.GetAppSettingsAsync();

            var miniaturesCache = _container.Resolve<IMiniaturesCache>();
            miniaturesCache.Clear();

            if (appSettings.UseWebApi)
            {
                _container.RegisterType<ILocationDataService, LocationDataServiceProxy>();
                _container.RegisterType<ILocationSnapshotDataService, LocationSnapshotDataServiceProxy>();
                _container.RegisterType<IPictureService, PictureServiceProxy>();
            }
            else
            {
                _container.RegisterType<ILocationDataService, LocationDataService>();
                _container.RegisterType<ILocationSnapshotDataService, LocationSnapshotDataService>();
                _container.RegisterType<IPictureService, PictureService>();
            }
        }

        public async Task<DataSourceType> GetCurrentDataSourceTypeAsync()
        {
            var appSettings = await AppSettingsProvider.GetAppSettingsAsync();

            return appSettings.UseWebApi ? DataSourceType.Remote : DataSourceType.Local;
        }
    }
}
