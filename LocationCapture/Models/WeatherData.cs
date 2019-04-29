using System;

namespace LocationCapture.Models
{
    public class WeatherData
    {
        public string CityName { get; set; }
        public string CountryName { get; set; }
        public DateTime SunRise { get; set; }
        public DateTime SunSet { get; set; }
        public double Temperature { get; set; }
        public double TemperatureMin { get; set; }
        public double TemperatureMax { get; set; }
        public double Humidity { get; set; }
        public double Pressure { get; set; }
        public string WindSpeedName { get; set; }
        public double WindSpeed { get; set; }
        public string WindDirectionName { get; set; }
        public double WindDirection { get; set; }
        public string CloudinessName { get; set; }
        public double Cloudiness { get; set; }
        public string PrecipitationName { get; set; }
        public double Precipitation { get; set; }
        public string WeatherDescription { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
