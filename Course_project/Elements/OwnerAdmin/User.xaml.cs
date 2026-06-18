using Course_project;
using Course_project_wpf.Controllers;
using Course_project_wpf.Models.FullModels;
using Course_project_wpf.Windows;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Course_project_wpf.Elements.OwnerAdmin
{
    public partial class User : UserControl
    {
        private Models.FullModels.User _user;
        private bool _isOwner;
        private bool _canToggleActive;

        // Храним кисти для применения к элементам
        private SolidColorBrush _colorBrush;
        private SolidColorBrush _darkColorBrush;

        public User()
        {
            CreateDefaultResources();
            InitializeComponent();
            _isOwner = App.CurrentUser?.Role == "Owner";
            ApplyColorsToElements();
        }

        public User(Models.FullModels.User user)
        {
            _user = user;
            _isOwner = App.CurrentUser?.Role == "Owner";

            // Проверяем, может ли текущий пользователь менять статус
            _canToggleActive = CheckTogglePermissions();

            CreateResourcesFromUser(user);
            InitializeComponent();
            ApplyColorsToElements();

            InitializeVariables(user);

            // Если пользователь не может менять статус - скрываем кнопку
            if (!_canToggleActive)
            {
                ToggleActiveButton.Visibility = Visibility.Collapsed;
                MainGrid.MouseEnter -= MainGrid_MouseEnter;
                MainGrid.MouseLeave -= MainGrid_MouseLeave;
            }
        }

        private bool CheckTogglePermissions()
        {
            // Если текущий пользователь Owner - может менять статус всем
            if (_isOwner)
                return true;

            // Если текущий пользователь Admin
            if (App.CurrentUser?.Id_role == 2) // Admin
            {
                // Admin может менять статус только у пользователей с ролью ниже (Moderator, Student, Teacher)
                return _user.Id_role >= 3;
            }

            return false;
        }

        private void CreateDefaultResources()
        {
            var defaultColor = Color.FromRgb(100, 100, 100);
            var defaultDarkColor = Color.FromRgb(70, 70, 70);

            _colorBrush = new SolidColorBrush(defaultColor);
            _darkColorBrush = new SolidColorBrush(defaultDarkColor);
        }

        private void CreateResourcesFromUser(Models.FullModels.User user)
        {
            Color mainColor;

            switch (user.Id_role)
            {
                case 1: mainColor = Color.FromRgb(106, 27, 154); break;   // Owner
                case 2: mainColor = Color.FromRgb(21, 101, 192); break;   // Admin
                case 3: mainColor = Color.FromRgb(46, 125, 50); break;    // Moderator
                case 4: mainColor = Color.FromRgb(0, 131, 143); break;    // Student
                case 5: mainColor = Color.FromRgb(230, 81, 0); break;     // Teacher
                default: mainColor = Color.FromRgb(100, 100, 100); break;
            }

            var darkColor = DarkenColor(mainColor, 0.7);

            _colorBrush = new SolidColorBrush(mainColor);
            _darkColorBrush = new SolidColorBrush(darkColor);
        }

        private Color DarkenColor(Color color, double factor)
        {
            return Color.FromRgb(
                (byte)(color.R * factor),
                (byte)(color.G * factor),
                (byte)(color.B * factor)
            );
        }

        private void ApplyColorsToElements()
        {
            bdId.Background = _colorBrush;
            bdName.Background = _colorBrush;
            bdEmail.Background = _colorBrush;
            bdPhone.Background = _colorBrush;
            bdRole.Background = _colorBrush;
            bdActions.Background = _colorBrush;
        }

        private void InitializeVariables(Models.FullModels.User user)
        {
            lbId.Content = user.Id.ToString();

            lbLastname.Text = user.Lastname;
            lbName.Text = $" {user.Name}";
            lbSurname.Text = user.Surname ?? "";

            lbEmail.Text = user.Email;
            lbPhone.Text = user.Phone_number ?? "Не указан";
            lbRole.Text = GetRoleName(user.Id_role);

            SetActiveStatus(user.Is_active);
        }

        private string GetRoleName(int roleId)
        {
            var role = GetController.Instance.GetRole(roleId);
            return role?.Name ?? "Неизвестно";
        }

        private void SetActiveStatus(bool isActive)
        {
            if (isActive)
            {
                ActiveIndicator.Background = new SolidColorBrush(Color.FromRgb(76, 175, 80));
                lbActive.Text = "Активен";
                ToggleIcon.Text = "◉";
                ToggleIcon.Foreground = new SolidColorBrush(Color.FromRgb(76, 175, 80));
            }
            else
            {
                ActiveIndicator.Background = new SolidColorBrush(Color.FromRgb(244, 67, 54));
                lbActive.Text = "Заблокирован";
                ToggleIcon.Text = "◯";
                ToggleIcon.Foreground = new SolidColorBrush(Color.FromRgb(244, 67, 54));
            }
        }

        #region Mouse Events

        private void MainGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            if (_canToggleActive)
            {
                ToggleActiveButton.Visibility = Visibility.Visible;
                ViewProfileButton.Visibility = Visibility.Visible;
            }
            else
            {
                ViewProfileButton.Visibility = Visibility.Visible;
            }
        }

        private void MainGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            ToggleActiveButton.Visibility = Visibility.Collapsed;
            ViewProfileButton.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region Actions

        private async void ToggleActive_Click(object sender, RoutedEventArgs e)
        {
            var action = _user.Is_active ? "деактивировать" : "активировать";
            var result = MessageBox.Show($"Вы уверены, что хотите {action} пользователя {_user.FullName}?",
                "Подтверждение действия", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    bool? isSuccess = await PutController.Instance.ChangeUserActive(_user.Id);

                    if (isSuccess != null && isSuccess == true)
                    {
                        _user.Is_active = !_user.Is_active;
                        SetActiveStatus(_user.Is_active);

                        // Обновляем кэш
                        await GetController.Instance.GetUsers(true);

                        MessageBox.Show($"Пользователь успешно {(_user.Is_active ? "активирован" : "деактивирован")}",
                            "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Ошибка: Не удалось сменить активность пользователя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ViewProfile_Click(object sender, RoutedEventArgs e)
        {
            // Открываем страницу профиля
            if (MainWindowOwner.OwnerWindow != null)
            {
                MainWindowOwner.OwnerWindow.PageParent.Navigate(new Pages.OwnerAdmin.UserProfile(_user));
            }
        }

        private void MainButton_Click(object sender, RoutedEventArgs e)
        {
            // При клике на карточку открываем профиль
            ViewProfile_Click(sender, e);
        }

        #endregion
    }
}