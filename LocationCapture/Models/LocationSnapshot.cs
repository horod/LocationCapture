using System;

namespace LocationCapture.Models
{
    public class LocationSnapshot : NotificationBase
    {
        public int Id { get; set; }
        
        public int LocationId { get; set; }

        private double _Longitude;
        public double Longitude
        {
            get { return _Longitude; }
            set { SetProperty(ref _Longitude, value); }
        }

        private double _Latitude;
        public double Latitude
        {
            get { return _Latitude; }
            set { SetProperty(ref _Latitude, value); }
        }

        private double _Altitude;
        public double Altitude
        {
            get { return _Altitude; }
            set { SetProperty(ref _Altitude, value); }
        }

        private string _PictureFileName;
        public string PictureFileName
        {
            get { return _PictureFileName; }
            set { SetProperty(ref _PictureFileName, value); }
        }

        private DateTime _DateCreated;
        public DateTime DateCreated
        {
            get { return _DateCreated; }
            set { SetProperty(ref _DateCreated, value); }
        }

        private string _Thumbnail;
        public string Thumbnail
        {
            get { return _Thumbnail; }
            set { SetProperty(ref _Thumbnail, value); }
        }
    }
}
