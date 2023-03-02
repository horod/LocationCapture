using System.Globalization;
using SelectionMode = LocationCapture.Client.MVVM.Enums.SelectionMode;

namespace LocationCapture.Client.DotNetMaui.Converters
{
    public class SelectionModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is SelectionMode)) return Microsoft.Maui.Controls.SelectionMode.None;

            var selectionMode = (SelectionMode)value;

            switch (selectionMode)
            {
                case SelectionMode.None:
                    return Microsoft.Maui.Controls.SelectionMode.None;
                case SelectionMode.Single:
                    return Microsoft.Maui.Controls.SelectionMode.Single;
                case SelectionMode.Multiple:
                    return Microsoft.Maui.Controls.SelectionMode.Multiple;
                default:
                    return Microsoft.Maui.Controls.SelectionMode.None;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
