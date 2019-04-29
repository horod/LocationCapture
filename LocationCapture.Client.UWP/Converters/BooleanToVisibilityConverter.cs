using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace LocationCapture.Client.UWP.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var direction = parameter as string ?? string.Empty;
            var boolVal = (bool)value;
            if(string.IsNullOrEmpty(direction))
            {
                return boolVal ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                return boolVal ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

    }
}
