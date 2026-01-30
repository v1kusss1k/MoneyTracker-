using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MoneyTracker.App.Converters
{
    public class BudgetStatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                switch (status.ToLower())
                {
                    case "превышен":
                        return new SolidColorBrush(Color.FromRgb(244, 67, 67));
                    case "близко к лимиту":
                        return new SolidColorBrush(Color.FromRgb(255, 152, 0));
                    case "в норме":
                        return new SolidColorBrush(Color.FromRgb(76, 175, 80));
                    default:
                        return new SolidColorBrush(Color.FromRgb(33, 150, 243));
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