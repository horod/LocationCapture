namespace LocationCapture.Models
{
    public class LocationDescriptor : NotificationBase
    {
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
    }
}
