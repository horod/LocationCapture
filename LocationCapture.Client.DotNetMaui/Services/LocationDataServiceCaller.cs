using LocationCapture.BL;
using LocationCapture.Client.MVVM.Services;
using LocationCapture.Enums;

namespace LocationCapture.Client.DotNetMaui.Services
{
    public class LocationDataServiceCaller : ILocationDataService
    {
        private readonly IDataSourceGovernor _dataSourceGovernor;
        private readonly LocationDataService _locationDataService;
        private readonly LocationDataServiceProxy _locationDataServiceProxy;

        public LocationDataServiceCaller(
            IDataSourceGovernor dataSourceGovernor,
            LocationDataService locationDataService,
            LocationDataServiceProxy locationDataServiceProxy)
        {
            _dataSourceGovernor = dataSourceGovernor;
            _locationDataService = locationDataService;
            _locationDataServiceProxy = locationDataServiceProxy;
        }

        public async Task<Models.Location> AddLocationAsync(Models.Location locationToAdd)
        {
            if(await _dataSourceGovernor.GetCurrentDataSourceTypeAsync() == DataSourceType.Local)
            {
                return await _locationDataService.AddLocationAsync(locationToAdd);
            }
            else
            {
                return await _locationDataServiceProxy.AddLocationAsync(locationToAdd);
            }
        }

        public async Task<IEnumerable<Models.Location>> GetAllLocationsAsync()
        {
            if (await _dataSourceGovernor.GetCurrentDataSourceTypeAsync() == DataSourceType.Local)
            {
                return await _locationDataService.GetAllLocationsAsync();
            }
            else
            {
                return await _locationDataServiceProxy.GetAllLocationsAsync();
            }
        }

        public async Task<Models.Location> RemoveLocationAsync(int locationId)
        {
            if (await _dataSourceGovernor.GetCurrentDataSourceTypeAsync() == DataSourceType.Local)
            {
                return await _locationDataService.RemoveLocationAsync(locationId);
            }
            else
            {
                return await _locationDataServiceProxy.RemoveLocationAsync(locationId);
            }
        }

        public async Task<Models.Location> RenameLocationAsync(int locationId, string newName)
        {
            if (await _dataSourceGovernor.GetCurrentDataSourceTypeAsync() == DataSourceType.Local)
            {
                return await _locationDataService.RenameLocationAsync(locationId, newName);
            }
            else
            {
                return await _locationDataServiceProxy.RenameLocationAsync(locationId, newName);
            }
        }
    }
}
