using LocationCapture.BL;
using LocationCapture.Client.MVVM.Enums;
using LocationCapture.Client.MVVM.Events;
using LocationCapture.Client.MVVM.Infrastructure;
using LocationCapture.Client.MVVM.Models;
using LocationCapture.Client.MVVM.Services;
using LocationCapture.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocationCapture.Client.MVVM.ViewModels
{
    public class SnapshotDetailsViewModel : NotificationBase, INavigationTarget
    {
        private readonly INavigationService _navigationService;
        private readonly IBitmapConverter _bitmapConverter;
        private readonly IPictureService _pictureService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IAppStateProvider _appStateProvider;
        private readonly ILocationSnapshotDataService _locationSnapshotDataService;
        private readonly IDialogService _dialogService;
        private readonly ILoggingService _loggingService;
        private SubscriptionToken _geolocationReadyToken;
        private LocationSnapshot _previous;
        private LocationSnapshot _next;

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

        private SnapshotDetailsViewNavParams NavParam => (SnapshotDetailsViewNavParams)NavigationParam;

        public SnapshotDetailsViewModel(INavigationService navigationService,
            IBitmapConverter bitmapConverter,
            IPictureService pictureService,
            IEventAggregator eventAggregator,
            IAppStateProvider appStateProvider,
            ILocationSnapshotDataService locationSnapshotDataService,
            ILoggingService loggingService,
            IDialogService dialogService)
        {
            _navigationService = navigationService;
            _pictureService = pictureService;
            _bitmapConverter = bitmapConverter;
            _eventAggregator = eventAggregator;
            _appStateProvider = appStateProvider;
            _locationSnapshotDataService = locationSnapshotDataService;
            _loggingService = loggingService;
            _dialogService = dialogService;

            AreDetailsVisible = false;
            IsCommandBarVisible = false;
        }

        public async Task OnLoaded()
        {
            IsBusy = true;

            var navParam = (SnapshotDetailsViewNavParams)NavigationParam;
            _geolocationReadyToken = _eventAggregator.GetEvent<GeolocationReadyEvent>().Subscribe(OnGeolocationReady);

            try
            {
                var data = await _pictureService.GetSnapshotContentAsync(navParam.LocationSnapshot);
                SnapshotContent = await _bitmapConverter.GetBitmapAsync(data);
                SnapshotDetails = navParam.LocationSnapshot;
                RaisePropertyChanged(nameof(IsGeolocationDataAvailable));

                var navigationEntry = NavParam.SnapshoNavigationMap?.FirstOrDefault(x => x.SnapshotId == NavParam.LocationSnapshot.Id);

                if (navigationEntry != null)
                {
                    _previous = (await _locationSnapshotDataService.GetSnapshotsByIdsAsync(new[] { navigationEntry.PreviousSnapshotId }))
                        .FirstOrDefault();
                    _next = (await _locationSnapshotDataService.GetSnapshotsByIdsAsync(new[] { navigationEntry.NextSnapshotId }))
                            .FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                var snapshotContentLoadingError = "Could not load snapshot content.";
                _loggingService.Warning(snapshotContentLoadingError + " Details: {Ex}", ex);
                await _dialogService.ShowAsync(snapshotContentLoadingError);
                GoBack();
            }

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

        public void ShowSuggestions()
        {
            var navParam = new SuggestionsViewNavParams
            {
                SelectedSuggestionType = LocationCapture.Enums.LocationSuggestionType.Description,
                SnapshotDetailsViewState = (SnapshotDetailsViewNavParams)NavigationParam
            };
            _navigationService.GoTo(AppViews.Suggestions, navParam);
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

        public async Task ImageSwipedLeft()
        {
            if (_next != null)
            {
                var navParam = new SnapshotDetailsViewNavParams
                {
                    LocationSnapshot = _next,
                    SnapshoNavigationMap = NavParam.SnapshoNavigationMap,
                    SnapshotsViewState = NavParam.SnapshotsViewState
                };

                NavigationParam = navParam;

                await OnNavigatedTo();
            }
        }

        public async Task ImageSwipedRight()
        {
            if (_previous != null)
            {
                var navParam = new SnapshotDetailsViewNavParams
                {
                    LocationSnapshot = _previous,
                    SnapshoNavigationMap = NavParam.SnapshoNavigationMap,
                    SnapshotsViewState = NavParam.SnapshotsViewState
                };

                NavigationParam = navParam;

                await OnNavigatedTo();
            }
        }

        public async Task OnNavigatedTo()
        {
            await OnLoaded();
        }

        public async Task SaveState()
        {
            var appState = new AppState
            {
                CurrentView = AppViews.SnapshotDetails,
                NavigationParam = new SnapshotDetailsViewNavParams
                {
                    LocationSnapshot = SnapshotDetails,
                    SnapshoNavigationMap = NavParam.SnapshoNavigationMap,
                    SnapshotsViewState = NavParam.SnapshotsViewState
                }
            };

            await _appStateProvider.SaveAppStateAsync(appState);
        }
    }
}