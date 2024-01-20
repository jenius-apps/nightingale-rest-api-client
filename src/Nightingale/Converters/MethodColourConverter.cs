using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Nightingale.Converters
{
    public class MethodColourConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int i)
            {
                switch (i)
                {
                    case 0:
                        return new SolidColorBrush(Colors.Green);
                    case 1:
                        return new SolidColorBrush(Colors.DarkGoldenrod);
                    case 2:
                        return new SolidColorBrush(Colors.DodgerBlue);
                    case 3:
                        return new SolidColorBrush(Colors.Red);
                    default:
                        return new SolidColorBrush(Colors.SlateGray);
                }
            }
            else if (value is string method)
            {
                switch (method)
                {
                    case "GET":
                        return new SolidColorBrush(Colors.Green);
                    case "POST":
                        return new SolidColorBrush(Colors.DarkGoldenrod);
                    case "PUT":
                        return new SolidColorBrush(Colors.DodgerBlue);
                    case "DELETE":
                        return new SolidColorBrush(Colors.Red);
                    default:
                        return new SolidColorBrush(Colors.SlateGray);
                }
            }
            else
            {
                return new SolidColorBrush(Colors.SlateGray);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
