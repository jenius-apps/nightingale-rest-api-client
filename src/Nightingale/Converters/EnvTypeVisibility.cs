﻿using Nightingale.Core.Models;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Nightingale.Converters
{
    public class EnvTypeVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value is ParamType type && type == ParamType.EnvVariable
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
