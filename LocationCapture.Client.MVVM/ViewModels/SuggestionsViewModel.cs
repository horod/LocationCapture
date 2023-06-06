using LocationCapture.BL;
using LocationCapture.Client.MVVM.Enums;
using LocationCapture.Client.MVVM.Infrastructure;
using LocationCapture.Client.MVVM.Models;
using LocationCapture.Client.MVVM.Services;
using LocationCapture.Enums;
using LocationCapture.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace LocationCapture.Client.MVVM.ViewModels
{
    public class SuggestionsViewModel : NotificationBase, INavigationTarget
    {
        private readonly ISuggestionsService _suggestionsService;
        private readonly INavigationService _navigationService;
        private readonly IConnectivityService _connectivityService;
        private readonly IDialogService _dialogService;
        private readonly ILoggingService _loggingService;
        private readonly GeolocationViewModel _geolocationViewModel;
        private readonly IAppStateProvider _appStateProvider;
        private LocationSnapshot _snapshot;

        private readonly string ApiErrorMsg = "Suggestions API error. This might indicate an incorrect API URI or key.";

        private string _LocationName;
        public string LocationName
        {
            get { return _LocationName; }
            set { SetProperty(ref _LocationName, value); }
        }

        private LocationSuggestionType _SelectedSuggestionType;
        public LocationSuggestionType SelectedSuggestionType
        {
            get { return _SelectedSuggestionType; }
            set { SetProperty(ref _SelectedSuggestionType, value); }
        }

        private List<LocationSuggestionType> _SuggestionTypes;
        public List<LocationSuggestionType> SuggestionTypes
        {
            get { return _SuggestionTypes; }
            set { SetProperty(ref _SuggestionTypes, value); }
        }

        private ObservableCollection<LocationSuggestion> _LocationSuggestions;
        public ObservableCollection<LocationSuggestion> LocationSuggestions
        {
            get { return _LocationSuggestions; }
            set { SetProperty(ref _LocationSuggestions, value); }
        }

        private bool _IsBusy;
        public bool IsBusy
        {
            get { return _IsBusy; }
            set { SetProperty(ref _IsBusy, value); }
        }

        public object NavigationParam { get; set; }

        public SuggestionsViewModel(ISuggestionsService suggestionsService,
            INavigationService navigationService,
            IConnectivityService connectivityService,
            IDialogService dialogService,
            ILoggingService loggingService,
            GeolocationViewModel geolocationViewModel,
            IAppStateProvider appStateProvider)
        {
            _suggestionsService = suggestionsService;
            _navigationService = navigationService;
            _connectivityService = connectivityService;
            _dialogService = dialogService;
            _loggingService = loggingService;
            _geolocationViewModel = geolocationViewModel;
            _appStateProvider = appStateProvider;

            SuggestionTypes = new List<LocationSuggestionType>
            {
                LocationSuggestionType.Description,
                LocationSuggestionType.Top3ThingsToDo,
                LocationSuggestionType.Top3PlacesToVisit,
                LocationSuggestionType.Top3Hotels,
                LocationSuggestionType.Top3Restaurants
            };
            SelectedSuggestionType = (LocationSuggestionType)(- 1);
            LocationSuggestions = new ObservableCollection<LocationSuggestion>();
        }

        public void GoBack()
        {
            _navigationService.GoTo(AppViews.SnapshotDetails, ((SuggestionsViewNavParams)NavigationParam).SnapshotDetailsViewState);
        }

        public Task OnNavigatedTo()
        {
            var navParam = (SuggestionsViewNavParams)NavigationParam;
            var snapshotDetailsNavParam = navParam.SnapshotDetailsViewState;
            
            _snapshot = snapshotDetailsNavParam.LocationSnapshot;
            LocationName = navParam.LocationName;
            SelectedSuggestionType = navParam.SelectedSuggestionType;

            return Task.CompletedTask;
        }

        public async Task OnSuggestionTypeChanged()
        {
            var noConnectionMsg = "Could not load suggestions data. No Internet connection available.";

            if (!_connectivityService.IsInternetAvailable())
            {
                _loggingService.Warning(noConnectionMsg);
                await _dialogService.ShowAsync(noConnectionMsg);
                GoBack();
                return;
            }

            IsBusy = true;
            try
            {
                LocationSuggestions.Clear();

                if (SelectedSuggestionType == LocationSuggestionType.Description)
                {
                    var results = await _suggestionsService.GetLocationDescription(_snapshot);
                    LocationName = results.First().Content;
                    LocationSuggestions.Add(results.Last());
                }
                else
                {
                    var results = await _suggestionsService.GetLocationSuggestions(LocationName, SelectedSuggestionType);
                    results.ToList().ForEach(s => LocationSuggestions.Add(s));
                }
            }
            catch (Exception ex)
            {
                _loggingService.Warning(ApiErrorMsg + " Details: {Ex}", ex);
                await _dialogService.ShowAsync(ApiErrorMsg);
                GoBack();
                return;
            }
            IsBusy = false;
        }

        public async Task OnNavigatedToVenue(LocationSuggestion suggestion)
        {
            IsBusy = true;
            try
            {
                var results = await _suggestionsService.GetVenueGpsCoordinates(suggestion.Venue, LocationName, SelectedSuggestionType);

                if (results.Latitude == double.MinValue || results.Longitude == double.MinValue)
                {
                    await _dialogService.ShowAsync($"Could not find the place called '{suggestion.Venue}' on map.");
                    IsBusy = false;
                    return;
                }

                var navParam = new SnapshotDetailsViewNavParams { LocationSnapshot = new LocationSnapshot { Latitude = results.Latitude, Longitude = results.Longitude } };

                _geolocationViewModel.NavigationParam = navParam;
                await _geolocationViewModel.OnLoaded();
            }
            catch (Exception ex)
            {
                _loggingService.Warning(ApiErrorMsg + " Details: {Ex}", ex);
                await _dialogService.ShowAsync(ApiErrorMsg);
            }
            IsBusy = false;
        }

        public async Task SaveState()
        {
            var appState = new AppState
            {
                CurrentView = AppViews.Suggestions,
                NavigationParam = new SuggestionsViewNavParams
                {
                    SelectedSuggestionType = SelectedSuggestionType,
                    LocationName = LocationName,
                    SnapshotDetailsViewState = ((SuggestionsViewNavParams)NavigationParam).SnapshotDetailsViewState
                }
            };

            await _appStateProvider.SaveAppStateAsync(appState);
        }
    }
}
