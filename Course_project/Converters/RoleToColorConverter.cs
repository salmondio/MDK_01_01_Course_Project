using System;
using System.Globalization;
using System.Windows;
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
                // Используем Application.Current только если он не null
                if (Application.Current != null)
                {
                    try
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
                    catch
                    {
                        // Если ресурс не найден - используем запасной цвет
                        return GetFallbackColor(roleId);
                    }
                }
                else
                {
                    // Если Application.Current == null (например, в дизайнере)
                    return GetFallbackColor(roleId);
                }
            }
            return new SolidColorBrush(Colors.Gray);
        }
        public object ConvertRoleToColor(object idRole)
        {
            if (idRole is int roleId)
            {
                // Используем Application.Current только если он не null
                if (Application.Current != null)
                {
                    try
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
                    catch
                    {
                        // Если ресурс не найден - используем запасной цвет
                        return GetFallbackColor(roleId);
                    }
                }
                else
                {
                    // Если Application.Current == null (например, в дизайнере)
                    return GetFallbackColor(roleId);
                }
            }
            return new SolidColorBrush(Colors.Gray);
        }

        private SolidColorBrush GetFallbackColor(int roleId)
        {
            return roleId switch
            {
                1 => new SolidColorBrush(Color.FromRgb(106, 27, 154)),   // Owner
                2 => new SolidColorBrush(Color.FromRgb(21, 101, 192)),   // Admin
                3 => new SolidColorBrush(Color.FromRgb(46, 125, 50)),    // Moderator
                4 => new SolidColorBrush(Color.FromRgb(0, 131, 143)),    // Student
                5 => new SolidColorBrush(Color.FromRgb(230, 81, 0)),     // Teacher
                _ => new SolidColorBrush(Colors.Gray)
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}