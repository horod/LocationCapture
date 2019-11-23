using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LocationCapture.Enums;
using LocationCapture.Models;

namespace LocationCapture.BL
{
    public class LocationDataImporter : ILocationDataImporter
    {
        private readonly ILocationDataService _locationServiceProxy;
        private readonly ILocationSnapshotDataService _snapshotServiceProxy;
        private readonly ILocationSnapshotDataService _snapshotService;
        private readonly IPictureService _pictureServiceProxy;
        private readonly IPictureService _pictureService;

        public LocationDataImporter(IDataServiceFactory dataServiceFactory)
        {
            _locationServiceProxy = dataServiceFactory.CreateLocationDataService(DataSourceType.Remote);
            _snapshotServiceProxy = dataServiceFactory.CreateLocationSnapshotDataService(DataSourceType.Remote);
            _snapshotService = dataServiceFactory.CreateLocationSnapshotDataService(DataSourceType.Local);
            _pictureServiceProxy = dataServiceFactory.CreatePictureService(DataSourceType.Remote);
            _pictureService = dataServiceFactory.CreatePictureService(DataSourceType.Local);
        }

        public async Task ImportAsync(IEnumerable<Location> locationsToImport, CancellationToken cancellationToken)
        {
            var remoteLocations = await _locationServiceProxy.GetAllLocationsAsync();

            foreach(var locationToImport in locationsToImport)
            {
                if (cancellationToken.IsCancellationRequested) return;

                Location importedLocation = null;

                if(remoteLocations.All(_ => _.Name != locationToImport.Name))
                {
                    importedLocation = await _locationServiceProxy.AddLocationAsync(locationToImport);
                }
                else
                {
                    importedLocation = remoteLocations.First(_ => _.Name == locationToImport.Name);
                }

                await ImportSnapshotsAsync(locationToImport, importedLocation, cancellationToken);
            }
        }

        public async Task ImportSnapshotsAsync(Location locationToImport, 
            Location importedLocation,
            CancellationToken cancellationToken)
        {
            var snapshotsToImport = await _snapshotService.GetSnapshotsByLocationIdAsync(locationToImport.Id);
            var remoteSnapshots = await _snapshotServiceProxy.GetSnapshotsByLocationIdAsync(importedLocation.Id);

            foreach (var snapshotToImport in snapshotsToImport)
            {
                if (cancellationToken.IsCancellationRequested) return;

                if (remoteSnapshots.Any(_ => _.PictureFileName == snapshotToImport.PictureFileName)) continue;

                var pictureToImport = await _pictureService.GetSnapshotContentAsync(snapshotToImport);
                if (pictureToImport.Length == 0) continue;
                snapshotToImport.LocationId = importedLocation.Id;
                await _snapshotServiceProxy.AddSnapshotAsync(snapshotToImport);
                await _pictureServiceProxy.SaveSnapshotContentAsync(snapshotToImport, pictureToImport);
            }
        }
    }
}
