using LocationCapture.BL;
using LocationCapture.Client.MVVM.Enums;
using LocationCapture.Client.MVVM.Events;
using LocationCapture.Client.MVVM.Models;
using LocationCapture.Client.MVVM.Services;
using LocationCapture.Client.MVVM.ViewModels;
using LocationCapture.Models;
using NSubstitute;
using Prism.Events;
using System.Threading.Tasks;
using Xunit;

namespace LocationCapture.Client.MVVM.UnitTests
{
    public class SnapshotDetailsViewModelUnitTests
    {
        private INavigationService _navigationService;
        private IBitmapConverter _bitmapConverter;
        private IPictureService _pictureService;
        private IEventAggregator _eventAggregator;

        [Fact]
        public async void OnLoaded_ShouldSucceed()
        {
            // Arrange
            SetUp();
            var navParam = new SnapshotDetailsViewNavParams
            {
                LocationSnapshot = new LocationSnapshot(),
                SnapshotsViewState = new object()
            };

            // Act
            var sit = CreateViewModel();
            sit.NavigationParam = navParam;
            await sit.OnLoaded();

            // Assert
            Assert.NotNull(sit.SnapshotContent);
            Assert.Equal(navParam.LocationSnapshot, sit.SnapshotDetails);
        }

        [Fact]
        public async void OnGeolocationReady_ShouldSucceed()
        {
            // Arrange
            SetUp();
            var locDescriptor = new LocationDescriptor
            {
                Longitude = 1,
                Latitude = 2,
                Altitude = 3
            };

            // Act
            var sit = CreateViewModel();
            sit.NavigationParam = new SnapshotDetailsViewNavParams { LocationSnapshot = new LocationSnapshot() };
            await sit.OnLoaded();
            _eventAggregator.GetEvent<GeolocationReadyEvent>().Publish(locDescriptor);

            // Assert
            Assert.Equal(locDescriptor.Longitude, sit.SnapshotDetails.Longitude);
            Assert.Equal(locDescriptor.Latitude, sit.SnapshotDetails.Latitude);
            Assert.Equal(locDescriptor.Altitude, sit.SnapshotDetails.Altitude);
            Assert.True(sit.IsGeolocationDataAvailable);
        }

        [Fact]
        public void ShowLocation_ShouldNavigateToGeolocationView()
        {
            // Arrange
            SetUp();

            // Act
            var sit = CreateViewModel();
            sit.ShowLocation();

            // Assert
            _navigationService.Received().GoTo(AppViews.Geolocation, sit.NavigationParam);
        }

        [Fact]
        public void ShowWeather_ShouldNavigateToWeatherView()
        {
            // Arrange
            SetUp();

            // Act
            var sit = CreateViewModel();
            sit.ShowWeather();

            // Assert
            _navigationService.Received().GoTo(AppViews.Weather, sit.NavigationParam);
        }

        [Fact]
        public void GoBack_ShouldNavigateToSnapshotsView()
        {
            // Arrange
            SetUp();

            // Act
            var sit = CreateViewModel();
            var viewState = new object();
            sit.NavigationParam = new SnapshotDetailsViewNavParams { SnapshotsViewState = viewState };
            sit.GoBack();

            // Assert
            _navigationService.Received().GoTo(AppViews.Snapshots, viewState);
        }

        private void SetUp()
        {
            _navigationService = Substitute.For<INavigationService>();
            _bitmapConverter = Substitute.For<IBitmapConverter>();
            _pictureService = Substitute.For<IPictureService>();
            _pictureService.GetSnapshotContentAsync(Arg.Any<LocationSnapshot>())
                .Returns(_ =>
                {
                    var tcs = new TaskCompletionSource<byte[]>();
                    tcs.SetResult(new byte[0]);
                    return tcs.Task;
                });
            _bitmapConverter.GetBitmapAsync(Arg.Any<byte[]>())
                .Returns(_ =>
                {
                    var tcs = new TaskCompletionSource<object>();
                    tcs.SetResult(new object());
                    return tcs.Task;
                });
            _eventAggregator = new EventAggregator();
        }

        private SnapshotDetailsViewModel CreateViewModel()
        {
            return new SnapshotDetailsViewModel(_navigationService,
                _bitmapConverter,
                _pictureService,
                _eventAggregator);
        }
    }
}
