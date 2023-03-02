namespace LocationCapture.Models
{
    public class AppSettings
    {
        public string DbConnectionString { get; set; }
        public string DbFileName { get; set; }
        public string MapsApiKey { get; set; }
        public string WeatherApiKey { get; set; }
        public string WeatherApiUri { get; set; }
        public string LocationCaptureApiUri { get; set; }
        public bool UseWebApi { get; set; }

        public AppSettings Clone()
        {
            return (AppSettings)MemberwiseClone();
        }
    }
}
