using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Course_project_wpf.Converters
{
    public class RoleToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int roleId)
            {
                return roleId switch
                {
                    1 => (SolidColorBrush)Application.Current.FindResource("OwnerColor"),
                    2 => (SolidColorBrush)Application.Current.FindResource("AdminColor"),
                    3 => (SolidColorBrush)Application.Current.FindResource("ModerColor"),
                    4 => (SolidColorBrush)Application.Current.FindResource("StudentColor"),
                    5 => (SolidColorBrush)Application.Current.FindResource("TeacherColor"),
                    _ => new SolidColorBrush(Colors.Gray)
                };
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}