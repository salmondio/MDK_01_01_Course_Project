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
        }

        private void MenuListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void MenuButtonClick(object sender, RoutedEventArgs e)
        {
            if (!MenuPopup.IsOpen)
            {
                MenuPopup.IsOpen = true;
                var showStoryboard = (Storyboard)FindResource("ShowMenuAnimation");
                showStoryboard.Begin(MenuBorder);
            }
            else
            {
                var hideStoryboard = (Storyboard)FindResource("HideMenuAnimation");
                hideStoryboard.Completed += (s, a) => MenuPopup.IsOpen = false;
                hideStoryboard.Begin(MenuBorder);
            }
        }
    }
}
