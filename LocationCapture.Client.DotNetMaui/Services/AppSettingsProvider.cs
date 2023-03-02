using LocationCapture.BL;
using LocationCapture.Models;
using System.Text.Json;

namespace LocationCapture.Client.DotNetMaui.Services
{
    public class AppSettingsProvider : IAppSettingsProvider
    {
        private const string AppSettingsFileName = "appSettings.json";

        private AppSettings _appSettings;

        public async Task<AppSettings> GetAppSettingsAsync()
        {
            if (_appSettings != null) return _appSettings.Clone();
            return await ReadAppSettings();
        }

        public async Task SaveAppSettingsAsync(AppSettings appSettings)
        {
            await WriteAppSettings(appSettings);
            _appSettings = appSettings;
        }

        private async Task<AppSettings> ReadAppSettings()
        {
            AppSettings appSettings;

            var appSettingsPath = Path.Combine(FileSystem.Current.AppDataDirectory, AppSettingsFileName);

            if (!File.Exists(appSettingsPath))
            {
                appSettings = GenerateDefaultAppSettings();
                _appSettings = appSettings;
                await WriteAppSettings(appSettings);
                return appSettings;
            }

            var appSettingsAsJson = await File.ReadAllTextAsync(appSettingsPath);
            appSettings = JsonSerializer.Deserialize<AppSettings>(appSettingsAsJson);
            _appSettings = appSettings;

            return appSettings;
        }

        private async Task WriteAppSettings(AppSettings appSettings)
        {
            appSettings.DbConnectionString = $"Filename={Path.Combine(FileSystem.AppDataDirectory, appSettings.DbFileName)}";

            var appSettingsPath = Path.Combine(FileSystem.Current.AppDataDirectory, AppSettingsFileName);

            await File.WriteAllTextAsync(appSettingsPath, JsonSerializer.Serialize(appSettings));
        }

        private AppSettings GenerateDefaultAppSettings()
        {
            return new AppSettings
            {
                DbConnectionString = $"Filename={Path.Combine(FileSystem.AppDataDirectory, "locationCapture.db")}",
                DbFileName = "locationCapture.db",
                WeatherApiUri = "http://api.openweathermap.org/data/2.5/weather",
                WeatherApiKey = "Generate your Openweather API key by following the instructions: https://openweathermap.org/appid",
                LocationCaptureApiUri = "http://localhost:81/api"
            };
        }
    }
}
