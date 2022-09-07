using System.ComponentModel.DataAnnotations;

namespace LocationCapture.WebAPI.Models
{
    public class LocationSnapshotForUpdateDto
    {
        [Required]
        public int Id { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public double Altitude { get; set; }

        public string Thumbnail { get; set; }
    }
}
