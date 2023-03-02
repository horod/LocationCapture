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
    public class PictureServiceCaller : IPictureService
    {
        private readonly IDataSourceGovernor _dataSourceGovernor;
        private readonly PictureService _pictureService;
        private readonly PictureServiceProxy _pictureServiceProxy;

        public PictureServiceCaller(
            IDataSourceGovernor dataSourceGovernor,
            PictureService pictureService,
            PictureServiceProxy pictureServiceProxy)
        {
            _dataSourceGovernor = dataSourceGovernor;
            _pictureService = pictureService;
            _pictureServiceProxy = pictureServiceProxy;
        }

        public async Task<byte[]> GetSnapshotContentAsync(LocationSnapshot snapshot)
        {
            if (await _dataSourceGovernor.GetCurrentDataSourceTypeAsync() == DataSourceType.Local)
            {
                return await _pictureService.GetSnapshotContentAsync(snapshot);
            }
            else
            {
                return await _pictureServiceProxy.GetSnapshotContentAsync(snapshot);
            }
        }

        public async Task<SnapshotMiniature> GetSnapshotMiniatureAsync(LocationSnapshot snapshot)
        {
            if (await _dataSourceGovernor.GetCurrentDataSourceTypeAsync() == DataSourceType.Local)
            {
                return await _pictureService.GetSnapshotMiniatureAsync(snapshot);
            }
            else
            {
                return await _pictureServiceProxy.GetSnapshotMiniatureAsync(snapshot);
            }
        }

        public async Task<IEnumerable<SnapshotMiniature>> GetSnapshotMiniaturesAsync(IEnumerable<LocationSnapshot> snapshots)
        {
            if (await _dataSourceGovernor.GetCurrentDataSourceTypeAsync() == DataSourceType.Local)
            {
                return await _pictureService.GetSnapshotMiniaturesAsync(snapshots);
            }
            else
            {
                return await _pictureServiceProxy.GetSnapshotMiniaturesAsync(snapshots);
            }
        }

        public async Task RemoveSnapshotContentAsync(LocationSnapshot snapshot)
        {
            if (await _dataSourceGovernor.GetCurrentDataSourceTypeAsync() == DataSourceType.Local)
            {
                await _pictureService.RemoveSnapshotContentAsync(snapshot);
            }
            else
            {
                await _pictureServiceProxy.RemoveSnapshotContentAsync(snapshot);
            }
        }

        public async Task<int> SaveSnapshotContentAsync(LocationSnapshot snapshot, byte[] data)
        {
            if (await _dataSourceGovernor.GetCurrentDataSourceTypeAsync() == DataSourceType.Local)
            {
                return await _pictureService.SaveSnapshotContentAsync(snapshot, data);
            }
            else
            {
                return await _pictureServiceProxy.SaveSnapshotContentAsync(snapshot, data);
            }
        }
    }
}
