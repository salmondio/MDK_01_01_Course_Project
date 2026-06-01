using Course_project_wpf.Controllers;
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
using System.Xml.Linq;

namespace Course_project_wpf.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindowOwner.xaml
    /// </summary>
    public partial class MainWindowOwner : Window
    {
        public static MainWindowOwner OwnerWindow { get; private set; }
        public static OwnerController OwnerController { get; private set; }
        public static AdminController AdminController { get; private set; }
        private List<string> _tabItems = new List<string>();
        private List<(string imgPath, string name)> _listBoxItems = new List<(string, string)>();
        private Elements.Header _header;
        public MainWindowOwner()
        {
            InitializeComponent();
            OwnerWindow = this;
            OwnerController = new OwnerController();
            AdminController = new AdminController();
            InitializeNavigationItems();

            _header = new Elements.Header("Owner", _tabItems, "Оценки", _listBoxItems);
            MainGrid.Children.Add(_header);
        }

        private void InitializeNavigationItems()
        {
            _tabItems.Add("Жалобы");
            _tabItems.Add("Отзывы");
            _tabItems.Add("Оценки");
            _tabItems.Add("Пользователи");
        }

        public void MoveToPage(string nameOfPage)
        {
            switch (nameOfPage)
            {
                case "Оценки":
                    PageParent.Navigate(new Pages.Owner.Evaluations(AdminController));
                    break;
                default:
                    MessageBox.Show("Ошибка: неизвестное имя страницы");
                    break;
            }
        }
    }
}
