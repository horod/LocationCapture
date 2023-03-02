using LocationCapture.BL;
using LocationCapture.Client.MVVM.Services;
using LocationCapture.Enums;

namespace LocationCapture.Client.DotNetMaui.Services
{
    public class DataSourceGovernor : IDataSourceGovernor
    {
        private readonly IServiceProvider _serviceProvider;

        public DataSourceGovernor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task ChooseDataSourceAsync()
        {
            var miniaturesCache = _serviceProvider.GetService<IMiniaturesCache>();
            miniaturesCache.Clear();

            return Task.CompletedTask;
        }

        public async Task<DataSourceType> GetCurrentDataSourceTypeAsync()
        {
            var appSettingsProvider = _serviceProvider.GetService<IAppSettingsProvider>();
            var appSettings = await appSettingsProvider.GetAppSettingsAsync();

            return appSettings.UseWebApi ? DataSourceType.Remote : DataSourceType.Local;
        }
    }
}
