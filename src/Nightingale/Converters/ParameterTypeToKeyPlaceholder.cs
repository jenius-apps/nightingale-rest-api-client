using Nightingale.Core.Models;
using System;
using Windows.UI.Xaml.Data;

namespace Nightingale.Converters
{
    public class ParameterTypeToKeyPlaceholder : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();

            if (value is ParamType parameterType)
            {
                switch (parameterType)
                {
                    case ParamType.ChainingRule:
                        return resourceLoader.GetString("VariableToUpdate");
                    default:
                        return resourceLoader.GetString("Key");
                }
            }
            else
            {
                return resourceLoader.GetString("Key");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
