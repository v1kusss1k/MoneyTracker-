using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MoneyTracker.App.Converters
{
    public class TransactionTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Brushes.Black;

            string type = value.ToString();

            if (type == "Income")
                return new SolidColorBrush(Color.FromRgb(76, 175, 80));   // Зелёный
            else if (type == "Expense")
                return new SolidColorBrush(Color.FromRgb(244, 67, 54));   // Красный

            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}