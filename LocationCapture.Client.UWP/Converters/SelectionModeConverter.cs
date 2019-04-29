using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using SelectionMode = LocationCapture.Client.MVVM.Enums.SelectionMode;

namespace LocationCapture.Client.UWP.Converters
{
    public class SelectionModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is SelectionMode)) return DependencyProperty.UnsetValue;

            var selectionMode = (SelectionMode)value;
            
            switch(selectionMode)
            {
                case SelectionMode.None:
                    return ListViewSelectionMode.None;
                case SelectionMode.Single:
                    return ListViewSelectionMode.Single;
                case SelectionMode.Multiple:
                    return ListViewSelectionMode.Multiple;
                case SelectionMode.Extended:
                    return ListViewSelectionMode.Extended;
                default:
                    return ListViewSelectionMode.None;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
