using Nightingale.Core.Models;
using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Nightingale.Converters
{
    class TestResultColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is TestResult tr)
            {
                if (tr == TestResult.Unstarted)
                {
                    return null;
                }
                else if (tr == TestResult.Pass)
                {
                    return new SolidColorBrush(Colors.LimeGreen);
                }
                else if (tr == TestResult.Fail)
                {
                    return new SolidColorBrush(Colors.Red);
                }
                else if (tr == TestResult.Error)
                {
                    return new SolidColorBrush(Colors.DarkGoldenrod);
                }
                else
                {
                    return null;
                }
            }
            else if (value is bool b)
            {
                return b ? new SolidColorBrush(Colors.LimeGreen) : new SolidColorBrush(Colors.Red);
            }
            else if (value is string statusCode)
            {
                return StatusCodeToColour(statusCode);
            }
            else if (value is int code)
            {
                return StatusCodeToColour(code.ToString());
            }
            else
            {
                return new SolidColorBrush(Colors.Gray);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        private SolidColorBrush StatusCodeToColour(string statusCode)
        {
            if (statusCode == null)
            {
                return new SolidColorBrush(Colors.Gray);
            }

            if (statusCode.StartsWith("4") || statusCode.StartsWith("5"))
            {
                return new SolidColorBrush(Colors.Red);
            }
            else if (statusCode.StartsWith("2"))
            {
                return new SolidColorBrush(Colors.LimeGreen);
            }
            else if (statusCode.StartsWith("1") || statusCode.StartsWith("3"))
            {
                return new SolidColorBrush(Colors.Orange);
            }

            return new SolidColorBrush(Colors.Gray);
        }
    }
}
