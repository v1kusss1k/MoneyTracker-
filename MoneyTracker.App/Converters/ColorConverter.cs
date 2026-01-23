//using System;
//using System.Globalization;
//using System.Windows.Data;
//using System.Windows.Media;

//namespace MoneyTracker.App.Converters  // ← ЭТО ВАЖНО!
//{
//    public class ColorConverter : IValueConverter
//    {
//        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            if (value is string colorString)
//            {
//                if (colorString == "Green")
//                    return new SolidColorBrush(Color.FromRgb(76, 175, 80));
//                else if (colorString == "Red")
//                    return new SolidColorBrush(Color.FromRgb(244, 67, 54));
//            }
//            return Brushes.Black;
//        }

//        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}