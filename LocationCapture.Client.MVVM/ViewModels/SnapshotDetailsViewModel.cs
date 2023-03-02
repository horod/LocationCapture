using LocationCapture.BL;
using LocationCapture.Client.MVVM.Enums;
using LocationCapture.Client.MVVM.Events;
using LocationCapture.Client.MVVM.Infrastructure;
using LocationCapture.Client.MVVM.Models;
using LocationCapture.Client.MVVM.Services;
using LocationCapture.Models;
using Prism.Events;
using System.Threading.Tasks;

namespace LocationCapture.Client.MVVM.ViewModels
{
    public class SnapshotDetailsViewModel : NotificationBase, INavigationTarget
    {
        private readonly INavigationService _navigationService;
        private readonly IBitmapConverter _bitmapConverter;
        private readonly IPictureService _pictureService;
        private readonly IEventAggregator _eventAggregator;
        private SubscriptionToken _geolocationReadyToken;

        public object NavigationParam { get; set; }

        private object _SnapshotContent;
        public object SnapshotContent
        {
            get { return _SnapshotContent; }
            set { SetProperty(ref _SnapshotContent, value); }
        }

        private LocationSnapshot _SnapshotDetails;
        public LocationSnapshot SnapshotDetails
        {
            get { return _SnapshotDetails; }
            set { SetProperty(ref _SnapshotDetails, value); }
        }

        private bool _AreDetailsVisible;
        public bool AreDetailsVisible
        {
            get { return _AreDetailsVisible; }
            set { SetProperty(ref _AreDetailsVisible, value); }
        }

        private bool _IsCommandBarVisible;
        public bool IsCommandBarVisible
        {
            get { return _IsCommandBarVisible; }
            set { SetProperty(ref _IsCommandBarVisible, value); }
        }

        private bool _IsBusy;
        public bool IsBusy
        {
            get { return _IsBusy; }
            set { SetProperty(ref _IsBusy, value); }
        }

        public bool IsGeolocationDataAvailable => SnapshotDetails?.Longitude > double.MinValue
            && SnapshotDetails?.Latitude > double.MinValue
            && SnapshotDetails?.Altitude > double.MinValue;

        public SnapshotDetailsViewModel(INavigationService navigationService,
            IBitmapConverter bitmapConverter,
            IPictureService pictureService,
            IEventAggregator eventAggregator)
        {
            _navigationService = navigationService;
            _pictureService = pictureService;
            _bitmapConverter = bitmapConverter;
            _eventAggregator = eventAggregator;

            AreDetailsVisible = false;
            IsCommandBarVisible = false;
        }

        public async Task OnLoaded()
        {
            IsBusy = true;
            var navParam = (SnapshotDetailsViewNavParams)NavigationParam;
            var data = await _pictureService.GetSnapshotContentAsync(navParam.LocationSnapshot);
            SnapshotContent = await _bitmapConverter.GetBitmapAsync(data);
            SnapshotDetails = navParam.LocationSnapshot;
            _geolocationReadyToken = _eventAggregator.GetEvent<GeolocationReadyEvent>().Subscribe(OnGeolocationReady);
            RaisePropertyChanged(nameof(IsGeolocationDataAvailable));
            IsBusy = false;
        }

        private void OnGeolocationReady(LocationDescriptor locationDescriptor)
        {
            SnapshotDetails.Longitude = locationDescriptor.Longitude;
            SnapshotDetails.Latitude = locationDescriptor.Latitude;
            SnapshotDetails.Altitude = locationDescriptor.Altitude;

            RaisePropertyChanged(nameof(IsGeolocationDataAvailable));
            RaisePropertyChanged(nameof(SnapshotDetails));
        }

        public void ShowLocation()
        {
            _navigationService.GoTo(AppViews.Geolocation, NavigationParam);
        }

        public void ShowWeather()
        {
            _navigationService.GoTo(AppViews.Weather, NavigationParam);
        }

        public void GoBack()
        {
            _geolocationReadyToken?.Dispose();
            var navParam = (SnapshotDetailsViewNavParams)NavigationParam;
            _navigationService.GoTo(AppViews.Snapshots, navParam.SnapshotsViewState);
        }

        public void ImageTapped()
        {
            IsCommandBarVisible = !IsCommandBarVisible;
        }

        public async Task OnNavigatedTo()
        {
            await OnLoaded();
        }
    }
}