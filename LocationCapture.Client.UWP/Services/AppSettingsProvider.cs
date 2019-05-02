using LocationCapture.BL;
using System.Threading.Tasks;
using LocationCapture.Models;
using Windows.Storage;
using System;
using System.Xml.Serialization;
using System.IO;

namespace LocationCapture.Client.UWP.Services
{
    public class AppSettingsProvider : IAppSettingsProvider
    {
        private const string AppSettingsFileName = "AppSettings.xml";

        private AppSettings _appSettings;

        public async Task<AppSettings> GetAppSettingsAsync()
        {
            if (_appSettings != null) return _appSettings.Clone();
            return await ReadAppSettings();
        }

        private async Task<AppSettings> ReadAppSettings()
        {
            AppSettings appSettings = null;
            var localFolder = ApplicationData.Current.LocalFolder;
            var appSettingsFile = (StorageFile)(await localFolder.TryGetItemAsync(AppSettingsFileName));

            if (appSettingsFile == null)
            {
                appSettings = GenerateDefaultAppSettings();
                _appSettings = appSettings;
                await WriteAppSettings(appSettings);
                return appSettings;
            }

            var serializer = new XmlSerializer(typeof(AppSettings));
            using (var stream = (await appSettingsFile.OpenReadAsync()).AsStream())
            {
                appSettings = (AppSettings)serializer.Deserialize(stream);
                _appSettings = appSettings;
                return appSettings;
            }
        }

        private async Task WriteAppSettings(AppSettings appSettings)
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            var appSettingsFile = await localFolder.CreateFileAsync(AppSettingsFileName, CreationCollisionOption.ReplaceExisting);
            var serializer = new XmlSerializer(typeof(AppSettings));
            using (var stream = await appSettingsFile.OpenStreamForWriteAsync())
            {
                serializer.Serialize(stream, appSettings);
            }
        }

        private AppSettings GenerateDefaultAppSettings()
        {
            return new AppSettings
            {
                DbConnectionString = "Data Source=locationCapture.db",
                MapsApiKey = "Generate your Bing Maps API key by following the instructions: https://docs.microsoft.com/en-us/bingmaps/getting-started/bing-maps-dev-center-help/getting-a-bing-maps-key",
                WeatherApiUri = "http://api.openweathermap.org/data/2.5/weather",
                WeatherApiKey = "Generate your Openweather API key by following the instructions: https://openweathermap.org/appid",
                LocationCaptureApiUri = "http://localhost:81/api"
            };
        }

        public async Task SaveAppSettingsAsync(AppSettings appSettings)
        {
            await WriteAppSettings(appSettings);
            _appSettings = appSettings;
        }
    }
}
