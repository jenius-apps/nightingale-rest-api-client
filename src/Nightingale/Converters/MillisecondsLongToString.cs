using System;
using Windows.UI.Xaml.Data;

namespace Nightingale.Converters
{
    public class MillisecondsLongToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is long l)
            {
                return $"{l} ms";
            }
            else
            {
                return "--";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
