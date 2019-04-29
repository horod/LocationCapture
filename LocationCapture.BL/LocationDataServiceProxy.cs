using LocationCapture.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocationCapture.BL
{
    public class LocationDataServiceProxy : ILocationDataService
    {
        private static string LocationsRoute = "locations";

        private readonly IAppSettingsProvider _appSettingsProvider;
        private readonly IWebClient _webClient;

        public LocationDataServiceProxy(IAppSettingsProvider appSettingsProvider,
            IWebClient webClient)
        {
            _appSettingsProvider = appSettingsProvider;
            _webClient = webClient;
        }

        private async Task<string> GetLocationsUrl()
        {
            var appSettings = await _appSettingsProvider.GetAppSettingsAsync();
            return string.Join("/", appSettings.LocationCaptureApiUri, LocationsRoute);
        }

        public async Task<Location> AddLocationAsync(Location locationToAdd)
        {
            var locationsUrl = await GetLocationsUrl();
            var result = await _webClient.PostAsync<Location, Location>(locationsUrl, locationToAdd);

            return result;
        }

        public async Task<IEnumerable<Location>> GetAllLocationsAsync()
        {
            var locationsUrl = await GetLocationsUrl();
            var result = await _webClient.GetAsync<IEnumerable<Location>>(locationsUrl);

            return result;
        }

        public async Task<Location> RemoveLocationAsync(int locationId)
        {
            var locationsUrl = await GetLocationsUrl();
            locationsUrl = $"{locationsUrl}/{locationId}";
            var result = await _webClient.DeleteAsync<Location>(locationsUrl);

            return result;
        }

        public async Task<Location> RenameLocationAsync(int locationId, string newName)
        {
            var locationsUrl = await GetLocationsUrl();
            locationsUrl = $"{locationsUrl}/{locationId}";
            var payload = new Location
            {
                Id = locationId,
                Name = newName
            };
            var result = await _webClient.PutAsync<Location, Location>(locationsUrl, payload);

            return result;
        }
    }
}
