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
        private List<string> tabItems = new List<string>();
        private List<(string imgPath, string name)> listBoxItems = new List<(string, string)>();
        public MainWindowOwner()
        {
            InitializeComponent();
            OwnerWindow = this;
            InitializeNavigationItems();

            
            MainGrid.Children.Add(new Elements.Header("Owner", tabItems, "Оценки", listBoxItems));
        }

        private void InitializeNavigationItems()
        {
            tabItems.Add("Жалобы");
            tabItems.Add("Отзывы");
            tabItems.Add("Оценки");
            tabItems.Add("Пользователи");
        }

        public void MoveToPage(string nameOfPage)
        {
            switch (nameOfPage)
            {
                case "Оценки":
                    PageParent.Navigate(new Pages.Owner.Evaluations());
                    break;
                default:
                    MessageBox.Show("Ошибка: неизвестное имя страницы");
                    break;
            }
        }
    }
}
