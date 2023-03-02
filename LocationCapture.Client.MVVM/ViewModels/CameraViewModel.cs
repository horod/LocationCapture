using LocationCapture.BL;
using LocationCapture.Client.MVVM.Enums;
using LocationCapture.Client.MVVM.Events;
using LocationCapture.Client.MVVM.Infrastructure;
using LocationCapture.Client.MVVM.Models;
using LocationCapture.Client.MVVM.Services;
using LocationCapture.Models;
using Prism.Events;
using System;
using System.Linq;
using System.Threading.Tasks;
using Location = LocationCapture.Models.Location;

namespace LocationCapture.Client.MVVM.ViewModels
{
    public class CameraViewModel : NotificationBase, INavigationTarget
    {
        private readonly INavigationService _navigationService;
        private readonly ICameraService _cameraService;
        private readonly IPictureService _pictureService;
        private readonly ILocationSnapshotDataService _locationSnapshotDataService;
        private readonly ILocationService _locationService;
        private readonly IEventAggregator _eventAggregator;

        public object NavigationParam { get; set; }

        private bool _IsBusy;
        public bool IsBusy
        {
            get { return _IsBusy; }
            set { SetProperty(ref _IsBusy, value); }
        }

        public CameraViewModel(INavigationService navigationService,
            ICameraService cameraService,
            IPictureService pictureService,
            ILocationSnapshotDataService locationSnapshotDataService,
            ILocationService locationService,
            IEventAggregator eventAggregator)
        {
            _navigationService = navigationService;
            _cameraService = cameraService;
            _pictureService = pictureService;
            _locationSnapshotDataService = locationSnapshotDataService;
            _locationService = locationService;
            _eventAggregator = eventAggregator;
        }

        public void OnCaptureElementLoaded(object sender, object e)
        {
            _cameraService.SetCaptureElement(sender);
        }

        public async Task OnLoaded()
        {
            await _cameraService.InitializeCamera();
        }

        public async Task OnCaptureSnapshot()
        {
            IsBusy = true;
            var pictureData = await _cameraService.CapturePhotoWithOrientationAsync();
            if (pictureData.Length == 0)
            {
                IsBusy = false;
                _cameraService.EndCapturingPhoto();
                return;
            }

            var nowDt = DateTime.Now;
            var now = nowDt.ToString("yyyyMMdd_HH_mm_ss");
            var pictureFileName = $"LocationCapture_{now}.jpg";
            var locationSnapshot = new LocationSnapshot
            {
                LocationId = GetLocationId(),
                PictureFileName = pictureFileName,
                Longitude = double.MinValue,
                Latitude = double.MinValue,
                Altitude = double.MinValue,
                DateCreated = nowDt
            };

            var newSnapshot = await _locationSnapshotDataService.AddSnapshotAsync(locationSnapshot);
            await _pictureService.SaveSnapshotContentAsync(newSnapshot, pictureData);
            IsBusy = false;
            _cameraService.EndCapturingPhoto();

            var locationDescriptor = await _locationService.GetCurrentLocationAsync();
            if (!(await _locationSnapshotDataService.GetSnapshotsByIdsAsync(new[] { newSnapshot.Id })).Any()) return;
            newSnapshot.Longitude = locationDescriptor.Longitude;
            newSnapshot.Latitude = locationDescriptor.Latitude;
            newSnapshot.Altitude = locationDescriptor.Altitude;
            await _locationSnapshotDataService.UpdateSnapshotAsync(newSnapshot);
            _eventAggregator.GetEvent<GeolocationReadyEvent>().Publish(locationDescriptor);
        }

        private int GetLocationId()
        {
            var navParam = (SnapshotsViewNavParams)NavigationParam;
            var location = (Location)navParam.SnapshotsIdsource;
            return location.Id;
        }

        public void GoBack()
        {
            if (IsBusy) return;
            _navigationService.GoTo(AppViews.Snapshots, NavigationParam);
        }

        public async Task DisposeAsync()
        {
            await _cameraService.CleanupCameraAsync();
        }

        public async Task OnNavigatedTo()
        {
            await OnLoaded();
        }
    }
}