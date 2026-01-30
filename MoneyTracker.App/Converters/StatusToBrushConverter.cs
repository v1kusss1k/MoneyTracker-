#nullable disable

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MoneyTracker.App.Converters
{
    public class StatusToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                switch (status)
                {
                    case "Превышен":
                        return new SolidColorBrush(Color.FromRgb(244, 67, 102));
                    case "Близко к лимиту":
                        return new SolidColorBrush(Color.FromRgb(255, 152, 0));
                    case "В норме":
                        return new SolidColorBrush(Color.FromRgb(76, 175, 80));
                    default:
                        return new SolidColorBrush(Colors.Gray);
                }
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}