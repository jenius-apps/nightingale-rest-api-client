using Nightingale.Core.Models;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Nightingale.Converters
{
    /// <summary>
    /// Class for returning Visibility.Visible when
    /// value is ParamType.Parameter.
    /// </summary>
    public class ParamTypeVisibilty : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value is ParamType type && 
                (type == ParamType.Parameter 
                    || type == ParamType.Header
                    || type == ParamType.FormEncodedData)
                ? Visibility.Visible 
                : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
