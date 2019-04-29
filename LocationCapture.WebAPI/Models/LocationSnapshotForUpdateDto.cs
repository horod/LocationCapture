using System.ComponentModel.DataAnnotations;

namespace LocationCapture.WebAPI.Models
{
    public class LocationSnapshotForUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Range(-180, 180)]
        public double Longitude { get; set; }

        [Range(-90, 90)]
        public double Latitude { get; set; }

        [Range(-1000, 9000)]
        public double Altitude { get; set; }
    }
}
