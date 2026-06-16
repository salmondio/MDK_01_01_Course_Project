using Course_project_wpf.Controllers;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Course_project_wpf.Windows
{
    public partial class MainWindowOwner : Window
    {
        public static MainWindowOwner OwnerWindow { get; private set; }
        private List<string> _tabItems = new List<string>();
        private List<(string imgPath, string name)> _listBoxItems = new List<(string, string)>();
        private Elements.Header _header;

        public MainWindowOwner()
        {
            InitializeComponent();
            OwnerWindow = this;
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
                    PageParent.Navigate(new Pages.Owner.Evaluations());
                    break;
                case "Пользователи":
                    PageParent.Navigate(new Pages.Owner.Users());
                    break;
                // Добавь другие страницы по мере необходимости
                case "Жалобы":
                    // PageParent.Navigate(new Pages.Owner.Reports());
                    break;
                case "Отзывы":
                    // PageParent.Navigate(new Pages.Owner.Reviews());
                    break;
                default:
                    MessageBox.Show("Ошибка: неизвестное имя страницы");
                    break;
            }
        }

        public void NavigateToUserProfile(Models.FullModels.User user)
        {
            PageParent.Navigate(new Pages.Owner.UserProfile(user));
        }
    }
}