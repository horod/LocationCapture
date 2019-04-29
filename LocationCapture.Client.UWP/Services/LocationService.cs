using System;
using System.Threading.Tasks;
using LocationCapture.BL;
using LocationCapture.Models;
using Windows.Devices.Geolocation;
using LocationCapture.Client.MVVM.Services;

namespace LocationCapture.Client.UWP.Services
{
    public class LocationService : ILocationService
    {
        private const int MaxRetryCount = 10;
        private const int RetryIntervalInMiliseconds = 6000;

        private readonly IDialogService _dialogService;
        private readonly ILoggingService _loggingService;

        public LocationService(IDialogService dialogService,
            ILoggingService loggingService)
        {
            _dialogService = dialogService;
            _loggingService = loggingService;
        }

        public async Task<LocationDescriptor> GetCurrentLocationAsync()
        {
            var retryCount = 0;
            var result = new LocationDescriptor();
            var accessStatus = GeolocationAccessStatus.Unspecified;

            while(accessStatus != GeolocationAccessStatus.Allowed)
            {
                accessStatus = await Geolocator.RequestAccessAsync();
                retryCount++;
                if (retryCount >= MaxRetryCount) break;
                await Task.Delay(RetryIntervalInMiliseconds);
            }

            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:
                    var geolocator = new Geolocator { DesiredAccuracy = PositionAccuracy.High };
                    var position = await geolocator.GetGeopositionAsync();
                    var basicGeoposition = position.Coordinate.Point.Position;
                    result = new LocationDescriptor
                    {
                        Longitude = basicGeoposition.Longitude,
                        Latitude = basicGeoposition.Latitude,
                        Altitude = basicGeoposition.Altitude
                    };
                    break;
                case GeolocationAccessStatus.Denied:
                    _loggingService.Warning("Access to location is denied.");
                    await _dialogService.ShowAsync("Access to location is denied.");
                    break;
                case GeolocationAccessStatus.Unspecified:
                    _loggingService.Warning("Unspecified error occurred while accessing location.");
                    await _dialogService.ShowAsync("Unspecified error occurred while accessing location.");
                    break;
            }

            return result;
        }
    }
}
