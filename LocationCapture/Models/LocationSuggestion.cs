using LocationCapture.Enums;

namespace LocationCapture.Models
{
    public class LocationSuggestion
    {
        public string Content { get; set; }

        public LocationSuggestionType Type { get; set; }

        public bool IsGeolocatable { get; set; }

        public string Venue { get; set; }
    }
}
