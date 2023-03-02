using LocationCapture.Client.MVVM.Services;
using LocationCapture.Models;

namespace LocationCapture.Client.DotNetMaui.Services
{
    public class LocationService : ILocationService
    {
        public async Task<LocationDescriptor> GetCurrentLocationAsync()
        {
            var result = new LocationDescriptor();

            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

                var location = await Geolocation.Default.GetLocationAsync(request);

                if (location != null)
                {
                    result.Longitude = location.Longitude;
                    result.Latitude = location.Latitude;
                    result.Altitude = location.Altitude ?? 0;
                }
            }
            catch (Exception)
            {
                // Unable to get location
            }

            return result;
        }
    }
}
