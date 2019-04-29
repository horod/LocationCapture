using System;
using System.ComponentModel.DataAnnotations;

namespace LocationCapture.WebAPI.Models
{
    public class LocationSnapshotForCreationDto
    {
        [Required]
        public int LocationId { get; set; }

        [Range(double.MinValue, double.MaxValue)]
        public double Longitude { get; set; }

        [Range(double.MinValue, double.MaxValue)]
        public double Latitude { get; set; }

        [Range(double.MinValue, double.MaxValue)]
        public double Altitude { get; set; }

        [StringLength(256, MinimumLength = 5)]
        [Required]
        public string PictureFileName { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }
    }
}
