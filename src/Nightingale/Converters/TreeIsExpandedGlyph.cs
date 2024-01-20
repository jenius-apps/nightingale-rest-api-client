using System;
using Windows.UI.Xaml.Data;

namespace Nightingale.Converters
{
    public class TreeIsExpandedGlyph : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool isExpanded)
            {
                return isExpanded ? "\uED43" : "\uED41";
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
