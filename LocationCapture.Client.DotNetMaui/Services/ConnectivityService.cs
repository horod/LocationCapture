using LocationCapture.BL;
using LocationCapture.Client.MVVM.Services;

namespace LocationCapture.Client.DotNetMaui.Services
{
    public class ConnectivityService : IConnectivityService
    {
        private readonly IAppSettingsProvider _appSettingsProvider;
        private readonly IWebClient _webClient;
        private readonly ILoggingService _loggingService;

        public ConnectivityService(IAppSettingsProvider appSettingsProvider,
            IWebClient webClient,
            ILoggingService loggingService)
        {
            _appSettingsProvider = appSettingsProvider;
            _webClient = webClient;
            _loggingService = loggingService;
        }

        public bool IsInternetAvailable()
        {
            NetworkAccess accessType = Connectivity.Current.NetworkAccess;

            return accessType == NetworkAccess.Internet;
        }

        public async Task<bool> IsWebApiAvailableAsync()
        {
            const string locationsRoute = "locations";
            var appSettings = await _appSettingsProvider.GetAppSettingsAsync();
            var locationsUrl = string.Join("/", appSettings.LocationCaptureApiUri, locationsRoute);

            try
            {
                await _webClient.GetStringAsync(locationsUrl);
                return true;
            }
            catch (Exception ex)
            {
                _loggingService.Warning("Web API not available. Details: {@Ex}", ex);
                return false;
            }
        }
    }
}
