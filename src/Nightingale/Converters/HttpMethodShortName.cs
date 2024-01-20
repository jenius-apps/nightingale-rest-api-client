using Nightingale.Core.Http;
using System;
using Windows.UI.Xaml.Data;

namespace Nightingale.Converters
{
    public class HttpMethodShortName : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int i)
            {
                return new RestSharpMethodConverter().IntToShortMethodName(i);
            }
            else if (value is string method)
            {
                return Method.GetShortName(method);
            }
            else
            {
                return "---";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException("Converting http shortname to int is not implemented");
        }
    }
}
