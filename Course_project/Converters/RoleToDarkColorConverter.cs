using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Course_project_wpf.Converters
{
    public class RoleToDarkColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int roleId)
            {
                return roleId switch
                {
                    1 => new SolidColorBrush(Color.FromRgb(74, 20, 110)),
                    2 => new SolidColorBrush(Color.FromRgb(21, 81, 162)),
                    3 => new SolidColorBrush(Color.FromRgb(40, 100, 45)),
                    4 => new SolidColorBrush(Color.FromRgb(0, 100, 110)),
                    5 => new SolidColorBrush(Color.FromRgb(200, 70, 0)),
                    _ => new SolidColorBrush(Colors.DarkGray)
                };
            }
            return new SolidColorBrush(Colors.DarkGray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}