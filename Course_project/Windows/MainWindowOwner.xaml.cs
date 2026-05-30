using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Course_project_wpf.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindowOwner.xaml
    /// </summary>
    public partial class MainWindowOwner : Window
    {
        public MainWindowOwner()
        {
            InitializeComponent();
            MenuButton.IsChecked = true;
        }

        private void MenuListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void MenuButton_Checked(object sender, RoutedEventArgs e)
        {
            var originalStoryboard = (Storyboard)FindResource("HideMenuAnimation");

            // Клонируем Storyboard, чтобы можно было изменять
            var cloneStoryboard = originalStoryboard.Clone();
            cloneStoryboard.Completed += (s, a) =>
            {
                MenuPopup.IsOpen = false;
                // Опционально: снимаем обработчик
                cloneStoryboard.Completed -= (s2, a2) => { };
            };
            cloneStoryboard.Begin(MenuBorder);
        }

        private void MenuButton_Unchecked(object sender, RoutedEventArgs e)
        {
            MenuPopup.IsOpen = true;
            var showStoryboard = (Storyboard)FindResource("ShowMenuAnimation");

            // Для показа анимации клонирование необязательно, если нет изменений
            var cloneShow = showStoryboard.Clone();
            cloneShow.Begin(MenuBorder);
        }

        private void MenuPopup_Closed(object sender, EventArgs e)
        {
            MenuButton.IsChecked = true;
        }
    }
}
