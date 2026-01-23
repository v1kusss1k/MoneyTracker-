using System;
using System.Globalization;
using System.Windows.Data;

namespace MoneyTracker.App.Converters
{
    public class ProgressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double progress)
            {
                return Math.Max(10, progress * 2); // Ширина в пикселях
            }
            return 100;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}