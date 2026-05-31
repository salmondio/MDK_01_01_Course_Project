using Course_project;
using Course_project_wpf.Windows;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Course_project_wpf.Elements
{
    /// <summary>
    /// Логика взаимодействия для Header.xaml
    /// </summary>
    public partial class Header : UserControl
    {
        private readonly string _role;
        public Header(string role, IEnumerable<string> tabItems, string defaultItemSelected, IEnumerable<(string imgSourse, string name)> arrayItems)
        {
            InitializeComponent();
            MenuButton.IsChecked = true;
            tcPages.SelectedItem = null;


            // Определяем роль и устанавливаем цветовую тему
            _role = role;
            SetTheme();
            // Заполнение навигационного меню
            foreach (string tabItem in tabItems)
            {
                tcPages.Items.Add(new TabItem() { Header = tabItem });
                // Выбираем элемент по умолчанию
                if(tabItem == defaultItemSelected)
                    tcPages.SelectedIndex = tcPages.Items.Count - 1;
            }
            // Заполнение навигационного выпадающего списка
            foreach (var arrayItem in arrayItems)
                MenuListBox.Items.Add(CreateListBoxItem(arrayItem.imgSourse, arrayItem.name));
            MenuListBox.Items.Add(new Separator()
            {
                Margin = new Thickness(0.5),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DADCE0")),
                Height = 1
            });
            MenuListBox.Items.Add(CreateListBoxItem(@"/Resources/Icons/Exit.png", "Выйти"));
        }

        private void SetTheme()
        {
            if (!String.IsNullOrEmpty(_role))
            {
                RoleName.Content = _role;
                switch (_role)
                {
                    case "Owner":
                        MainGrid.Background = (SolidColorBrush)FindResource("OwnerColor");
                        break;
                    case "Admin":
                        MainGrid.Background = (SolidColorBrush)FindResource("AdminColor");
                        break;
                    case "Moderator":
                        MainGrid.Background = (SolidColorBrush)FindResource("ModeratorColor");
                        break;
                    case "Student":
                        MainGrid.Background = (SolidColorBrush)FindResource("StudentColor");
                        break;
                    case "Teacher":
                        MainGrid.Background = (SolidColorBrush)FindResource("TeacherColor");
                        break;
                    default:
                        RoleName.Content = "Неавторизован";
                        MainGrid.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DADCE0"));
                        break;
                }
            }
        }

        private ListBoxItem CreateListBoxItem(string imgPath, string name)
        {
            // Создаем компоненты
            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };

            var image = new Image
            {
                Source = new BitmapImage(new Uri(imgPath, UriKind.Relative)),
                Width = 16,
                Height = 16,
                Margin = new Thickness(0, 0, 10, 0)
            };

            var textBlock = new TextBlock
            {
                Text = name,
                VerticalAlignment = VerticalAlignment.Center
            };

            // Суем в стак панель
            stackPanel.Children.Add(image);
            stackPanel.Children.Add(textBlock);

            // Возвращаем готовый элемент
            return new ListBoxItem
            {
                Name = name,
                Content = stackPanel
            };
        }

        private void NavigateToPage(string pageName)
        {
            switch (_role)
            {
                case "Owner":
                    MainWindowOwner.OwnerWindow.MoveToPage(pageName);
                    break;
                //case "Admin":
                //    MainWindowAdmin.AdminWindow.MoveToPage(pageName);
                //    break;
                //case "Moderator":
                //    MainWindowModerator.ModeratorWindow.MoveToPage(pageName);
                //    break;
                //case "Student":
                //    MainWindowStudent.StudentWindow.MoveToPage(pageName);
                //    break;
                //case "Teacher":
                //    MainWindowTeacher.TeacherWindow.MoveToPage(pageName);
                //    break;
                default:
                    MessageBox.Show("Ошибка: неизвестная роль пользователя");
                    break;
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabItem? selectedItem = (sender as TabControl)?.SelectedItem as TabItem;

            if(selectedItem != null)
            {
                string? selectedItemName = selectedItem.Header.ToString();
                if (selectedItemName != null)
                {
                    NavigateToPage (selectedItemName);
                }
                else
                    MessageBox.Show("Ошибка: Выбранный элемент меню не имеет заголовка");
            }
        }

        private void MenuListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem? selectedItem = (sender as ListBox)?.SelectedItem as ListBoxItem;

            if (selectedItem != null)
            {
                string? selectedItemName = selectedItem.Name;
                if (selectedItemName != null)
                {
                    NavigateToPage(selectedItemName);
                }
                else
                    MessageBox.Show("Ошибка: Выбранный элемент меню не имеет заголовка");
            }
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
