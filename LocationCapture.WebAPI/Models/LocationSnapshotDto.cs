using System;

namespace LocationCapture.WebAPI.Models
{
    public class LocationSnapshotDto
    {
        public int Id { get; set; }

        public int LocationId { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public double Altitude { get; set; }

        public string PictureFileName { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
