using System;
using Windows.UI.Xaml.Data;

namespace LocationCapture.Client.UWP.Converters
{
    public class StringFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var stringFormat = (string)parameter;
            return string.Format(stringFormat, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
