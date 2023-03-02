using LocationCapture.BL;
using LocationCapture.Client.MVVM.Services;
using LocationCapture.Enums;
using LocationCapture.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationCapture.Client.DotNetMaui.Services
{
    public class LocationSnapshotDataServiceCaller : ILocationSnapshotDataService
    {
        private readonly IDataSourceGovernor _dataSourceGovernor;
        private readonly LocationSnapshotDataService _dataService;
        private readonly LocationSnapshotDataServiceProxy _dataServiceProxy;

        public LocationSnapshotDataServiceCaller(
            IDataSourceGovernor dataSourceGovernor,
            LocationSnapshotDataService dataService,
            LocationSnapshotDataServiceProxy dataServiceProxy)
        {
            _dataSourceGovernor = dataSourceGovernor;
            _dataService = dataService;
            _dataServiceProxy = dataServiceProxy;
        }

        public async Task<LocationSnapshot> AddSnapshotAsync(LocationSnapshot snapshotToAdd)
        {
            if (await _dataSourceGovernor.GetCurrentDataSourceTypeAsync() == DataSourceType.Local)
            {
                return await _dataService.AddSnapshotAsync(snapshotToAdd);
            }
            else
            {
                return await _dataServiceProxy.AddSnapshotAsync(snapshotToAdd);
            }
        }

        public Func<Task<IEnumerable<SnapshotGroup>>> ChooseGroupByOperation(GroupByCriteria groupBy)
        {
            if (_dataSourceGovernor.GetCurrentDataSourceTypeAsync().Result == DataSourceType.Local)
            {
                return _dataService.ChooseGroupByOperation(groupBy);
            }
            else
            {
                return _dataServiceProxy.ChooseGroupByOperation(groupBy);
            }
        }

        public async Task<IEnumerable<LocationSnapshot>> GetSnapshotsByIdsAsync(IEnumerable<int> snapshotIds)
        {
            if (await _dataSourceGovernor.GetCurrentDataSourceTypeAsync() == DataSourceType.Local)
            {
                return await _dataService.GetSnapshotsByIdsAsync(snapshotIds);
            }
            else
            {
                return await _dataServiceProxy.GetSnapshotsByIdsAsync(snapshotIds);
            }
        }

        public async Task<IEnumerable<LocationSnapshot>> GetSnapshotsByLocationIdAsync(int locationId)
        {
            if (await _dataSourceGovernor.GetCurrentDataSourceTypeAsync() == DataSourceType.Local)
            {
                return await _dataService.GetSnapshotsByLocationIdAsync(locationId);
            }
            else
            {
                return await _dataServiceProxy.GetSnapshotsByLocationIdAsync(locationId);
            }
        }

        public async Task<LocationSnapshot> RemoveSnapshotAsync(int snapshotId)
        {
            if (await _dataSourceGovernor.GetCurrentDataSourceTypeAsync() == DataSourceType.Local)
            {
                return await _dataService.RemoveSnapshotAsync(snapshotId);
            }
            else
            {
                return await _dataServiceProxy.RemoveSnapshotAsync(snapshotId);
            }
        }

        public async Task<LocationSnapshot> UpdateSnapshotAsync(LocationSnapshot snapshot)
        {
            if (await _dataSourceGovernor.GetCurrentDataSourceTypeAsync() == DataSourceType.Local)
            {
                return await _dataService.UpdateSnapshotAsync(snapshot);
            }
            else
            {
                return await _dataServiceProxy.UpdateSnapshotAsync(snapshot);
            }
        }
    }
}
