using System;
using System.Globalization;
using System.Windows.Data;
using BotLibrary.Utilities;

namespace AnchorsCreator.Converters
{
    public class OffsetsToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as Offsets)?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Offsets.FromText(value as string);
        }
    }
}
