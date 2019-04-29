using LocationCapture.Models;
using System;
using Windows.UI.Xaml.Data;

namespace LocationCapture.Client.UWP.Converters
{
    public class CoordinateValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var snapshot = (LocationSnapshot)value;
            if (snapshot == null) return string.Empty;
            var coordinateType = (string)parameter;

            switch (coordinateType)
            {
                case nameof(snapshot.Longitude):
                    return GetCoordinateValue(() => snapshot.Longitude, snapshot);
                case nameof(snapshot.Latitude):
                    return GetCoordinateValue(() => snapshot.Latitude, snapshot);
                case nameof(snapshot.Altitude):
                    return GetCoordinateValue(() => snapshot.Altitude, snapshot);
            }

            return string.Empty;
        }

        private string GetCoordinateValue(Func<double> selector, LocationSnapshot snapshot)
        {
            var coordinate = selector();
            var result = coordinate == double.MinValue
                            ? (DateTime.Now - snapshot.DateCreated).Minutes > 1
                                ? "Geolocation data not available"
                                : "Loading geolocation data..."
                            : coordinate.ToString();
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
