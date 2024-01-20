using Nightingale.Core.Workspaces.Models;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Nightingale.Converters
{
    public class RequestTypeVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value is ItemType type && type == ItemType.Request
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
