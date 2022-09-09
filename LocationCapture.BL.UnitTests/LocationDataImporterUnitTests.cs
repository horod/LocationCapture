using LocationCapture.Enums;
using LocationCapture.Models;
using NSubstitute;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LocationCapture.BL.UnitTests
{
    public class LocationDataImporterUnitTests
    {
        private IDataServiceFactory _dataServiceFactory;
        private ILocationDataService _locationServiceProxy;
        private ILocationSnapshotDataService _snapshotServiceProxy;
        private ILocationSnapshotDataService _snapshotService;
        private IPictureService _pictureServiceProxy;
        private IPictureService _pictureService;
        private Location _locationToImport;
        private List<LocationSnapshot> _snapshotsToImport;
        private List<LocationSnapshot> _importedSnapshots;
        private byte[] _pictureData;

        [Fact]
        public async void ImportAsync_ShouldImportNewLocation()
        {
            // Arrange
            SetUp();
            _locationServiceProxy.GetAllLocationsAsync()
                .Returns(_ =>
                {
                    var tcs = new TaskCompletionSource<IEnumerable<Location>>();
                    tcs.SetResult(new List<Location>());
                    return tcs.Task;
                });
            _locationServiceProxy.AddLocationAsync(Arg.Any<Location>())
                .Returns(_ =>
                {
                    var tcs = new TaskCompletionSource<Location>();
                    tcs.SetResult(_locationToImport);
                    return tcs.Task;
                });
            _snapshotServiceProxy.GetSnapshotsByLocationIdAsync(Arg.Any<int>())
                .Returns(_ =>
                {
                    var tcs = new TaskCompletionSource<IEnumerable<LocationSnapshot>>();
                    tcs.SetResult(new List<LocationSnapshot>());
                    return tcs.Task;
                });

            // Act
            var sit = new LocationDataImporter(_dataServiceFactory);
            await sit.ImportAsync(new List<Location> { _locationToImport }, new CancellationTokenSource().Token);

            // Assert
            await _locationServiceProxy.Received().AddLocationAsync(_locationToImport);
            await _snapshotServiceProxy.Received().AddSnapshotAsync(_snapshotsToImport[0]);
            await _snapshotServiceProxy.Received().AddSnapshotAsync(_snapshotsToImport[1]);
            await _pictureServiceProxy.Received().SaveSnapshotContentAsync(_importedSnapshots[0], _pictureData);
            await _pictureServiceProxy.Received().SaveSnapshotContentAsync(_importedSnapshots[1], _pictureData);
        }

        [Fact]
        public async void ImportAsync_ShouldComplementExistingLocation()
        {
            // Arrange
            SetUp();
            _locationServiceProxy.GetAllLocationsAsync()
                .Returns(_ =>
                {
                    var tcs = new TaskCompletionSource<IEnumerable<Location>>();
                    tcs.SetResult(new List<Location> { _locationToImport });
                    return tcs.Task;
                });
            _snapshotServiceProxy.GetSnapshotsByLocationIdAsync(Arg.Any<int>())
                .Returns(_ =>
                {
                    var tcs = new TaskCompletionSource<IEnumerable<LocationSnapshot>>();
                    tcs.SetResult(new List<LocationSnapshot> {_snapshotsToImport[0]});
                    return tcs.Task;
                });

            // Act
            var sit = new LocationDataImporter(_dataServiceFactory);
            await sit.ImportAsync(new List<Location> { _locationToImport }, new CancellationTokenSource().Token);

            // Assert
            await _locationServiceProxy.DidNotReceive().AddLocationAsync(_locationToImport);
            await _snapshotServiceProxy.DidNotReceive().AddSnapshotAsync(_snapshotsToImport[0]);
            await _snapshotServiceProxy.Received().AddSnapshotAsync(_snapshotsToImport[1]);
            await _pictureServiceProxy.DidNotReceive().SaveSnapshotContentAsync(_importedSnapshots[0], _pictureData);
            await _pictureServiceProxy.Received().SaveSnapshotContentAsync(_importedSnapshots[1], _pictureData);
        }

        private void SetUp()
        {
            _dataServiceFactory = Substitute.For<IDataServiceFactory>();
            _locationServiceProxy = Substitute.For<ILocationDataService>();
            _snapshotServiceProxy = Substitute.For<ILocationSnapshotDataService>();
            _snapshotService = Substitute.For<ILocationSnapshotDataService>();
            _pictureServiceProxy = Substitute.For<IPictureService>();
            _pictureService = Substitute.For<IPictureService>();

            _dataServiceFactory.CreateLocationDataService(DataSourceType.Remote).Returns(_locationServiceProxy);
            _dataServiceFactory.CreateLocationSnapshotDataService(DataSourceType.Remote).Returns(_snapshotServiceProxy);
            _dataServiceFactory.CreateLocationSnapshotDataService(DataSourceType.Local).Returns(_snapshotService);
            _dataServiceFactory.CreatePictureService(DataSourceType.Remote).Returns(_pictureServiceProxy);
            _dataServiceFactory.CreatePictureService(DataSourceType.Local).Returns(_pictureService);

            _locationToImport = new Location
            {
                Id = 1,
                Name = "Barcelona"
            };
            _snapshotsToImport = new List<LocationSnapshot>
            {
                new LocationSnapshot
                {
                    Id = 1,
                    LocationId = 1,
                    PictureFileName = "Barcelona1.jpg"
                },
                new LocationSnapshot
                {
                    Id = 2,
                    LocationId = 1,
                    PictureFileName = "Barcelona2.jpg"
                },
            };

            _importedSnapshots = new List<LocationSnapshot>
            {
                new LocationSnapshot
                {
                    Id = 3,
                    LocationId = _snapshotsToImport[0].LocationId,
                    PictureFileName = _snapshotsToImport[0].PictureFileName
                },
                new LocationSnapshot
                {
                    Id = 4,
                    LocationId = _snapshotsToImport[1].LocationId,
                    PictureFileName = _snapshotsToImport[1].PictureFileName
                },
            };

            _snapshotService.GetSnapshotsByLocationIdAsync(Arg.Any<int>())
                .Returns(_ =>
                {
                    var tcs = new TaskCompletionSource<IEnumerable<LocationSnapshot>>();
                    tcs.SetResult(_snapshotsToImport);
                    return tcs.Task;
                });

            _pictureData = new byte[] { 1, 2, 3, 4 };
            _pictureService.GetSnapshotContentAsync(Arg.Any<LocationSnapshot>())
                .Returns(_ =>
                {
                    var tcs = new TaskCompletionSource<byte[]>();
                    tcs.SetResult(_pictureData);
                    return tcs.Task;
                });

            _snapshotServiceProxy.AddSnapshotAsync(Arg.Any<LocationSnapshot>())
                .Returns(_ =>
                {
                    var snapshotToImport = _.Arg<LocationSnapshot>();
                    LocationSnapshot importedSnapshot;

                    if (snapshotToImport == _snapshotsToImport[0])
                    {
                        importedSnapshot = _importedSnapshots[0];
                    }
                    else
                    {
                        importedSnapshot = _importedSnapshots[1];
                    }

                    var tcs = new TaskCompletionSource<LocationSnapshot>();
                    tcs.SetResult(importedSnapshot);
                    return tcs.Task;
                });
        }
    }
}
