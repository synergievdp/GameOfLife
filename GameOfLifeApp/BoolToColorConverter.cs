using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace GameOfLifeApp {
    public class BoolToColorConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            bool alive = (bool)value;
            if (alive)
                return new SolidColorBrush(Colors.Red);
            else
                return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
