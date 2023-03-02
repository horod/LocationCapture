using LocationCapture.BL;
using LocationCapture.Client.MVVM.Enums;
using LocationCapture.Client.MVVM.Events;
using LocationCapture.Client.MVVM.Models;
using LocationCapture.Client.MVVM.Services;
using LocationCapture.Client.MVVM.ViewModels;
using LocationCapture.Models;
using NSubstitute;
using Prism.Events;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace LocationCapture.Client.MVVM.UnitTests
{
    public class CameraViewModelUnitTests
    {
        private INavigationService _navigationService;
        private ICameraService _cameraService;
        private IPictureService _pictureService;
        private ILocationSnapshotDataService _locationSnapshotDataService;
        private ILocationService _locationService;
        private IEventAggregator _eventAggregator;

        [Fact]
        public async void OnCaptureSnapshot_ShouldSucceed()
        {
            // Arrange
            SetUp();
            var navParam = new SnapshotsViewNavParams
            {
                SnapshotsIdsource = new Location { Id = 2 }
            };
            var locDescriptor = new LocationDescriptor
            {
                Longitude = 1,
                Latitude = 2,
                Altitude = 3
            };
            LocationSnapshot newSnapshot = null;
            LocationDescriptor receivedLocDesc = null;
            _locationSnapshotDataService.AddSnapshotAsync(Arg.Any<LocationSnapshot>())
                .Returns(_ =>
                {
                    var snapshot = _.Arg<LocationSnapshot>();
                    var tcs = new TaskCompletionSource<LocationSnapshot>();
                    tcs.SetResult(snapshot);
                    return tcs.Task;
                });
            _pictureService.SaveSnapshotContentAsync(Arg.Any<LocationSnapshot>(), Arg.Any<byte[]>())
                .Returns(_ =>
                {
                    var tcs = new TaskCompletionSource<int>();
                    tcs.SetResult(0);
                    return tcs.Task;
                });
            _locationService.GetCurrentLocationAsync()
                .Returns(_ =>
                {
                    var tcs = new TaskCompletionSource<LocationDescriptor>();
                    tcs.SetResult(locDescriptor);
                    return tcs.Task;
                });
            _locationSnapshotDataService.GetSnapshotsByIdsAsync(Arg.Any<IEnumerable<int>>())
                .Returns(_ =>
                {
                    var tcs = new TaskCompletionSource<IEnumerable<LocationSnapshot>>();
                    tcs.SetResult(new List<LocationSnapshot> { new LocationSnapshot() });
                    return tcs.Task;
                });
            _locationSnapshotDataService.AddSnapshotAsync(Arg.Any<LocationSnapshot>())
                .Returns(_ =>
                {
                    newSnapshot = _.Arg<LocationSnapshot>();
                    var tcs = new TaskCompletionSource<LocationSnapshot>();
                    tcs.SetResult(newSnapshot);
                    return tcs.Task;
                });
            _eventAggregator.GetEvent<GeolocationReadyEvent>().Subscribe(_ => receivedLocDesc = _);
            _cameraService.CapturePhotoWithOrientationAsync()
                .Returns(_ => Task.FromResult(new byte[10]));

            // Act
            var sit = CreateViewModel();
            sit.NavigationParam = navParam;
            await sit.OnCaptureSnapshot();

            // Assert
            Assert.Equal(((Location)navParam.SnapshotsIdsource).Id, newSnapshot.LocationId);
            Assert.True(newSnapshot.PictureFileName.StartsWith("LocationCapture_") 
                && newSnapshot.PictureFileName.EndsWith(".jpg"));
            Assert.Equal(locDescriptor.Longitude, newSnapshot.Longitude);
            Assert.Equal(locDescriptor.Latitude, newSnapshot.Latitude);
            Assert.Equal(locDescriptor.Altitude, newSnapshot.Altitude);
            Assert.Equal(newSnapshot.PictureFileName.Substring(16, 17), newSnapshot.DateCreated.ToString("yyyyMMdd_HH_mm_ss"));
            Assert.Equal(locDescriptor, receivedLocDesc);
        }

        [Fact]
        public void GoBack_ShouldNavigateToSnapshotsView()
        {
            // Arrange
            SetUp();

            // Act
            var sit = CreateViewModel();
            sit.NavigationParam = new object();
            sit.GoBack();

            // Assert
            _navigationService.Received().GoTo(AppViews.Snapshots, sit.NavigationParam);
        }

        private void SetUp()
        {
            _navigationService = Substitute.For<INavigationService>();
            _cameraService = Substitute.For<ICameraService>();
            _pictureService = Substitute.For<IPictureService>();
            _locationSnapshotDataService = Substitute.For<ILocationSnapshotDataService>();
            _locationService = Substitute.For<ILocationService>();
            _eventAggregator = new EventAggregator();
        }

        private CameraViewModel CreateViewModel()
        {
            return new CameraViewModel(_navigationService,
                _cameraService,
                _pictureService,
                _locationSnapshotDataService,
                _locationService,
                _eventAggregator);
        }
    }
}
