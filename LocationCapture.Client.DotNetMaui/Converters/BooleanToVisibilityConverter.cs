using System.Globalization;

namespace LocationCapture.Client.DotNetMaui.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var direction = parameter as string ?? string.Empty;
            var boolVal = (bool)value;
            if (string.IsNullOrEmpty(direction))
            {
                return boolVal ? true : false;
            }
            else
            {
                return boolVal ? false : true;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
