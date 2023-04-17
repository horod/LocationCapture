using LocationCapture.BL;
using LocationCapture.Client.MVVM.Enums;
using LocationCapture.Client.MVVM.Infrastructure;
using LocationCapture.Client.MVVM.Models;
using LocationCapture.Client.MVVM.Services;
using LocationCapture.Models;
using System;
using System.Threading.Tasks;

namespace LocationCapture.Client.MVVM.ViewModels
{
    public class WeatherViewModel : NotificationBase, INavigationTarget
    {
        private readonly INavigationService _navigationService;
        private readonly IWeatherDataService _weatherDataService;
        private readonly IConnectivityService _connectivityService;
        private readonly IDialogService _dialogService;
        private readonly ILoggingService _loggingService;
        private readonly IAppStateProvider _appStateProvider;

        public object NavigationParam { get; set; }

        private bool _IsBusy;
        public bool IsBusy
        {
            get { return _IsBusy; }
            set { SetProperty(ref _IsBusy, value); }
        }

        private WeatherData _WeatherForecast;
        public WeatherData WeatherForecast
        {
            get { return _WeatherForecast; }
            set { SetProperty(ref _WeatherForecast, value); }
        }

        public WeatherViewModel(INavigationService navigationService,
            IWeatherDataService weatherDataService,
            IConnectivityService connectivityService,
            IDialogService dialogService,
            ILoggingService loggingService,
            IAppStateProvider appStateProvider)
        {
            _navigationService = navigationService;
            _weatherDataService = weatherDataService;
            _connectivityService = connectivityService;
            _dialogService = dialogService;
            _loggingService = loggingService;
            _appStateProvider = appStateProvider;
        }

        public async Task OnLoaded()
        {
            if (!_connectivityService.IsInternetAvailable())
            {
                _loggingService.Warning("Could not load weather data. No Internet connection available.");
                await _dialogService.ShowAsync("Could not load weather data. No Internet connection available.");
                _navigationService.GoTo(AppViews.SnapshotDetails, NavigationParam);
                return;
            }

            IsBusy = true;
            var navParam = (SnapshotDetailsViewNavParams)NavigationParam;
            var snapshot = navParam.LocationSnapshot;
            try
            {
                WeatherForecast = await _weatherDataService.GetWeatherDataForLocationAsync(snapshot);
            }
            catch (Exception ex)
            {
                _loggingService.Warning("Weather API error. This might indicate an incorrect API URI or key. Details: {Ex}", ex);
                await _dialogService.ShowAsync("Weather API error. This might indicate an incorrect API URI or key.");
                _navigationService.GoTo(AppViews.SnapshotDetails, NavigationParam);
                return;
            }
            IsBusy = false;
        }

        public void GoBack()
        {
            _navigationService.GoTo(AppViews.SnapshotDetails, NavigationParam);
        }

        public async Task OnNavigatedTo()
        {
            await OnLoaded();
        }

        public async Task SaveState()
        {
            var appState = new AppState
            {
                CurrentView = AppViews.Weather,
                NavigationParam = NavigationParam
            };

            await _appStateProvider.SaveAppStateAsync(appState);
        }
    }
}