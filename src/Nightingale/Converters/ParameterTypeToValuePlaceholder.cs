using Nightingale.Core.Models;
using System;
using Windows.UI.Xaml.Data;

namespace Nightingale.Converters
{
    public class ParameterTypeToValuePlaceholder : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();

            if (value is ParamType parameterType)
            {
                switch (parameterType)
                {
                    case ParamType.ChainingRule:
                        return resourceLoader.GetString("ResponsePropertyExtact");
                    default:
                        return resourceLoader.GetString("Value");
                }
            }
            else
            {
                return resourceLoader.GetString("Value");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
