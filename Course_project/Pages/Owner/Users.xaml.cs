using Course_project_wpf.Controllers;
using Course_project_wpf.Elements;
using Course_project_wpf.Elements.OwnerAdmin;
using Course_project_wpf.Models.FullModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Course_project_wpf.Pages.Owner
{
    public partial class Users : Page
    {
        private List<Models.FullModels.User>? _users;
        private SortableHeader? _sortableHeader;

        public Users()
        {
            InitializeComponent();
            Design();
        }

        private void Design()
        {
            _sortableHeader = new SortableHeader();
            _sortableHeader.SortRequested += SortHeader_SortRequested;

            // Добавляем колонки с разными типами ширины
            _sortableHeader.AddColumnFixed("Id", "ID", 80, HorizontalAlignment.Center);
            _sortableHeader.AddColumnStar("FullName", "ФИО", 2, HorizontalAlignment.Left);
            _sortableHeader.AddColumnStar("Email", "Email", 1.5, HorizontalAlignment.Left);
            _sortableHeader.AddColumnStar("Phone", "Телефон", 1.5, HorizontalAlignment.Left);
            _sortableHeader.AddColumnFixed("Role", "Роль", 120, HorizontalAlignment.Center);
            _sortableHeader.AddColumnFixed("Actions", "Действия", 100, HorizontalAlignment.Center);

            Search.Children.Add(_sortableHeader);

            GetUsers();
        }

        private async void GetUsers()
        {
            Parent.Children.Clear();

            await GetController.Instance.GetRoles();
            _users = await GetController.Instance.GetUsers();

            if (_users != null && _users.Count != 0)
            {
                var sortedUsers = _users.OrderBy(u => u.Id).ToList();

                foreach (var user in sortedUsers)
                {
                    Parent.Children.Add(new Elements.OwnerAdmin.User(user));
                }
            }
            else
            {
                Parent.Children.Add(new Label()
                {
                    Content = "Пользователей нет",
                    FontSize = 20,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Foreground = (SolidColorBrush)FindResource("Hint")
                });
            }
        }

        private void SortHeader_SortRequested(object sender, SortEventArgs e)
        {
            if (_users == null || _users.Count == 0)
                return;

            IEnumerable<Models.FullModels.User> sortedUsers;

            switch (e.ColumnName)
            {
                case "Id":
                    sortedUsers = e.IsAscending
                        ? _users.OrderBy(u => u.Id)
                        : _users.OrderByDescending(u => u.Id);
                    break;
                case "FullName":
                    sortedUsers = e.IsAscending
                        ? _users.OrderBy(u => u.Lastname).ThenBy(u => u.Name).ThenBy(u => u.Surname)
                        : _users.OrderByDescending(u => u.Lastname).ThenByDescending(u => u.Name).ThenByDescending(u => u.Surname);
                    break;
                case "Email":
                    sortedUsers = e.IsAscending
                        ? _users.OrderBy(u => u.Email)
                        : _users.OrderByDescending(u => u.Email);
                    break;
                case "Phone":
                    sortedUsers = e.IsAscending
                        ? _users.OrderBy(u => u.Phone_number)
                        : _users.OrderByDescending(u => u.Phone_number);
                    break;
                case "Role":
                    sortedUsers = e.IsAscending
                        ? _users.OrderBy(u => u.Id_role)
                        : _users.OrderByDescending(u => u.Id_role);
                    break;
                default:
                    sortedUsers = _users;
                    break;
            }

            Parent.Children.Clear();
            foreach (var user in sortedUsers)
            {
                Parent.Children.Add(new Elements.OwnerAdmin.User(user));
            }
        }

        public async void RefreshUsers()
        {
            await GetController.Instance.GetUsers(true);
            GetUsers();
        }
    }
}