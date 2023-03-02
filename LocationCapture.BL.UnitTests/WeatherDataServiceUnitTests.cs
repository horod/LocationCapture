using LocationCapture.Models;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

namespace LocationCapture.BL.UnitTests
{
    public class WeatherDataServiceUnitTests
    {
        [Fact]
        public async void GetWeatherDataForLocationAsync_ShouldGetCompleteWeatherData()
        {
            // Arrange
            var xml = "<?xml version='1.0' encoding='UTF-8'?><current><city id='3095140' name='Koscielisko'><coord lon='19.89' lat='49.29'></coord><country>PL</country><sun rise='2018-01-01T06:35:44' set='2018-01-01T14:53:07'></sun></city><temperature value='3' min='3' max='3' unit='metric'></temperature><humidity value='86' unit='%'></humidity><pressure value='1007' unit='hPa'></pressure><wind><speed value='4.1' name='Gentle Breeze'></speed><gusts></gusts><direction value='250' code='WSW' name='West-southwest'></direction></wind><clouds value='90' name='overcast clouds'></clouds><visibility value='10000'></visibility><precipitation mode='rain' value='10'></precipitation><weather number='804' value='overcast clouds' icon='04n'></weather><lastupdate value='2018-01-01T18:00:00'></lastupdate></current>";
            var webClient = Substitute.For<IWebClient>();
            webClient.GetStringAsync(Arg.Any<string>())
                .Returns(_ =>
                {
                    var tcs = new TaskCompletionSource<string>();
                    tcs.SetResult(xml);
                    return tcs.Task;
                });
            var appSettingsProvider = Substitute.For<IAppSettingsProvider>();
            appSettingsProvider.GetAppSettingsAsync().
                Returns(_ =>
                {
                    var tcs = new TaskCompletionSource<AppSettings>();
                    tcs.SetResult(new AppSettings());
                    return tcs.Task;
                });

            // Act
            var sit = new WeatherDataService(webClient, appSettingsProvider);
            var weatherData = await sit.GetWeatherDataForLocationAsync(new LocationSnapshot());

            // Assert
            Assert.Equal("Koscielisko", weatherData.CityName);
            Assert.Equal("Poland", weatherData.CountryName);
            Assert.Equal(3d, weatherData.Temperature);
            Assert.Equal(3d, weatherData.TemperatureMin);
            Assert.Equal(3d, weatherData.TemperatureMax);
            Assert.Equal(DateTime.Parse("2018-01-01T06:35:44").ToLocalTime(), weatherData.SunRise);
            Assert.Equal(DateTime.Parse("2018-01-01T14:53:07").ToLocalTime(), weatherData.SunSet);
            Assert.Equal(86d, weatherData.Humidity);
            Assert.Equal(1007d, weatherData.Pressure);
            Assert.Equal(Math.Round(4.1 * 0.001 * 3600, 2), weatherData.WindSpeed);
            Assert.Equal("Gentle Breeze", weatherData.WindSpeedName);
            Assert.Equal(250d, weatherData.WindDirection);
            Assert.Equal("West-southwest", weatherData.WindDirectionName);
            Assert.Equal(90d, weatherData.Cloudiness);
            Assert.Equal("overcast clouds", weatherData.CloudinessName);
            Assert.Equal(10d, weatherData.Precipitation);
            Assert.Equal("rain", weatherData.PrecipitationName);
            Assert.Equal(DateTime.Parse("2018-01-01T18:00:00").ToLocalTime(), weatherData.LastUpdate);
        }

        [Fact]
        public async void GetWeatherDataForLocationAsync_ShouldBuildCorrectWeatherApiUrl()
        {
            // Arrange
            var xml = "<?xml version='1.0' encoding='UTF-8'?><current></current>";
            var actualUrl = string.Empty;
            const string expectedUrl = "http://api.openweathermap.org/data/2.5/weather?mode=xml&units=metric&type=accurate&lon=19.89&lat=49.29&APPID=64b8f091998633d5aa257ee3141b0b4c";
            var webClient = Substitute.For<IWebClient>();
            webClient.GetStringAsync(Arg.Any<string>())
                .Returns(_ =>
                {
                    actualUrl = _.Arg<string>();
                    var tcs = new TaskCompletionSource<string>();
                    tcs.SetResult(xml);
                    return tcs.Task;
                });
            var appSettingsProvider = Substitute.For<IAppSettingsProvider>();
            appSettingsProvider.GetAppSettingsAsync().
                Returns(_ =>
                {
                    var tcs = new TaskCompletionSource<AppSettings>();
                    tcs.SetResult(new AppSettings { WeatherApiUri = "http://api.openweathermap.org/data/2.5/weather", WeatherApiKey = "64b8f091998633d5aa257ee3141b0b4c" });
                    return tcs.Task;
                });

            // Act
            var sit = new WeatherDataService(webClient, appSettingsProvider);
            var weatherData = await sit.GetWeatherDataForLocationAsync(new LocationSnapshot { Longitude = 19.89 , Latitude = 49.29 });

            // Assert
            Assert.Equal(expectedUrl, actualUrl);
        }
    }
}
