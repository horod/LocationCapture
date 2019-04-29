using LocationCapture.BL;
using LocationCapture.Client.MVVM.Enums;
using LocationCapture.Client.MVVM.Infrastructure;
using LocationCapture.Client.MVVM.Models;
using LocationCapture.Client.MVVM.Services;
using LocationCapture.Models;
using System;

namespace LocationCapture.Client.MVVM.ViewModels
{
    public class WeatherViewModel : NotificationBase, INavigationTarget
    {
        private readonly INavigationService _navigationService;
        private readonly IWeatherDataService _weatherDataService;
        private readonly IConnectivityService _connectivityService;
        private readonly IDialogService _dialogService;
        private readonly ILoggingService _loggingService;

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
            ILoggingService loggingService)
        {
            _navigationService = navigationService;
            _weatherDataService = weatherDataService;
            _connectivityService = connectivityService;
            _dialogService = dialogService;
            _loggingService = loggingService;
        }

        public async void OnLoaded()
        {
            if(!_connectivityService.IsInternetAvailable())
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
            catch(Exception ex)
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
    }
}
