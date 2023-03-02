using LocationCapture.Models;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Globalization;
using System;

namespace LocationCapture.BL
{
    public class WeatherDataService : IWeatherDataService
    {
        private readonly IWebClient _webClient;
        private readonly IAppSettingsProvider _appSettingsProvider;

        public WeatherDataService(IWebClient webClient,
            IAppSettingsProvider appSettingsProvider)
        {
            _webClient = webClient;
            _appSettingsProvider = appSettingsProvider;
        }

        public async Task<WeatherData> GetWeatherDataForLocationAsync(LocationSnapshot locationSnapshot)
        {
            var appSettings = await _appSettingsProvider.GetAppSettingsAsync();
            var uri = GetWeatherUri(locationSnapshot, appSettings);
            var rawWeatherData = await _webClient.GetStringAsync(uri);

            return ParseWeatherData(rawWeatherData);
        }

        private string GetWeatherUri(LocationSnapshot locationSnapshot, AppSettings appSettings) =>
            string.Format(CultureInfo.InvariantCulture, "{0}?mode=xml&units=metric&type=accurate&lon={1}&lat={2}&APPID={3}", appSettings.WeatherApiUri, locationSnapshot.Longitude, locationSnapshot.Latitude, appSettings.WeatherApiKey);

        private WeatherData ParseWeatherData(string rawWeatherData)
        {
            var document = XDocument.Parse(rawWeatherData);
            var result = new WeatherData();
            var formatProvider = CultureInfo.InvariantCulture;
            var root = document.Root;

            var cityElement = root?.Element("city");
            result.CityName = cityElement?.Attribute("name")?.Value;
            var countryCode = cityElement?.Element("country")?.Value;
            result.CountryName = new RegionInfo(countryCode ?? "PL").EnglishName;
            var sunElement = cityElement?.Element("sun");
            result.SunRise = DateTime.Parse(sunElement?.Attribute("rise")?.Value ?? "1901-01-01").ToLocalTime();
            result.SunSet = DateTime.Parse(sunElement?.Attribute("set")?.Value ?? "1901-01-01").ToLocalTime();
            var temperatureElement = root?.Element("temperature");
            result.Temperature = double.Parse(temperatureElement?.Attribute("value")?.Value ?? int.MinValue.ToString(), formatProvider);
            result.TemperatureMin = double.Parse(temperatureElement?.Attribute("min")?.Value ?? int.MinValue.ToString(), formatProvider);
            result.TemperatureMax = double.Parse(temperatureElement?.Attribute("max")?.Value ?? int.MinValue.ToString(), formatProvider);
            result.Humidity = double.Parse(root?.Element("humidity")?.Attribute("value")?.Value ?? int.MinValue.ToString(), formatProvider);
            result.Pressure = double.Parse(root?.Element("pressure")?.Attribute("value")?.Value ?? int.MinValue.ToString(), formatProvider);
            var windSpeedElement = root?.Element("wind")?.Element("speed");
            var windSpeed = double.Parse(windSpeedElement?.Attribute("value")?.Value ?? int.MinValue.ToString(), formatProvider);
            result.WindSpeed = Math.Round(windSpeed * 0.001 * 3600, 2);
            result.WindSpeedName = windSpeedElement?.Attribute("name")?.Value;
            var windDirectionElement = root?.Element("wind")?.Element("direction");
            result.WindDirection = double.Parse(windDirectionElement?.Attribute("value")?.Value ?? int.MinValue.ToString(), formatProvider);
            result.WindDirectionName = windDirectionElement?.Attribute("name")?.Value;
            var cloudsElement = root?.Element("clouds");
            result.Cloudiness = double.Parse(cloudsElement?.Attribute("value")?.Value ?? int.MinValue.ToString(), formatProvider);
            result.CloudinessName = cloudsElement?.Attribute("name")?.Value;
            var precipitationElement = root?.Element("precipitation");
            var precipitation = double.Parse(precipitationElement?.Attribute("value")?.Value ?? int.MinValue.ToString(), formatProvider);            
            result.PrecipitationName = precipitationElement?.Attribute("mode")?.Value;
            result.Precipitation = (result.PrecipitationName == "no") ? 0 : precipitation;
            result.WeatherDescription = root?.Element("weather")?.Attribute("value")?.Value;
            result.LastUpdate = DateTime.Parse(root?.Element("lastupdate")?.Attribute("value")?.Value ?? "1901-01-01").ToLocalTime();

            return result;
        }
    }
}
