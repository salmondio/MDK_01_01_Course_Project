using Course_project;
using Course_project_wpf.Controllers;
using Course_project_wpf.Elements;
using Course_project_wpf.Models.FullModels;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Course_project_wpf.Pages.Owner
{
    public partial class UserProfile : Page
    {
        private Models.FullModels.User _user;
        private bool _isEditing = false;
        private bool _canEdit = false;
        private int _currentUserRoleId;
        private int _targetUserRoleId;

        public UserProfile(Models.FullModels.User user)
        {
            InitializeComponent();
            _user = user;

            // Получаем роль текущего пользователя из токена
            _currentUserRoleId = GetCurrentUserRoleId();
            _targetUserRoleId = user.Id_role;
            Converters.RoleToColorConverter converter = new Converters.RoleToColorConverter();
            Header.Background = converter.ConvertRoleToColor(_targetUserRoleId) as Brush;
            // Проверяем права на редактирование
            CheckEditPermissions();

            InitializeVariables(user);
            SetEditMode(false);
        }

        private int GetCurrentUserRoleId()
        {
            // Пытаемся получить из App.CurrentUser
            if (App.CurrentUser != null)
            {
                return App.CurrentUser.Id_role;
            }

            // Если по какой-то причине нет, пробуем получить из токена через GetController
            try
            {
                // Можно добавить метод в GetController для получения текущего пользователя
                var users = GetController.Instance.GetUsers(false).GetAwaiter().GetResult();
                if (users != null)
                {
                    // Ищем пользователя по email или другим данным
                    // Пока возвращаем 4 (Student) как дефолт
                    return 4;
                }
            }
            catch { }

            return 4; // По умолчанию Student
        }

        private void CheckEditPermissions()
        {
            // Если пользователь Owner - может редактировать всех
            if (_currentUserRoleId == 1) // Owner
            {
                _canEdit = true;
                return;
            }

            // Если пользователь Admin - может редактировать только тех, у кого роль ниже
            if (_currentUserRoleId == 2) // Admin
            {
                // Admin может редактировать: Moderator (3), Student (4), Teacher (5)
                // НЕ может редактировать: Owner (1), Admin (2)
                _canEdit = _targetUserRoleId >= 3;
                return;
            }

            // Остальные роли не могут редактировать
            _canEdit = false;
        }

        private void InitializeVariables(Models.FullModels.User user)
        {
            tbId.Text = user.Id.ToString();
            tbLastname.Text = user.Lastname;
            tbName.Text = user.Name;
            tbSurname.Text = user.Surname ?? "";
            tbEmail.Text = user.Email;
            tbPhone.Text = user.Phone_number ?? "Не указан";
            tbRole.Text = GetRoleName(user.Id_role);

            SetActiveStatus(user.Is_active);

            // Применяем цвет роли к заголовку
            ApplyRoleColor(user.Id_role);
        }

        private void SetActiveStatus(bool isActive)
        {
            if (isActive)
            {
                StatusIndicator.Background = new SolidColorBrush(Color.FromRgb(76, 175, 80));
                tbStatus.Text = "Активен";
                tbStatus.Foreground = new SolidColorBrush(Color.FromRgb(76, 175, 80));
            }
            else
            {
                StatusIndicator.Background = new SolidColorBrush(Color.FromRgb(244, 67, 54));
                tbStatus.Text = "Заблокирован";
                tbStatus.Foreground = new SolidColorBrush(Color.FromRgb(244, 67, 54));
            }
        }

        private string GetRoleName(int roleId)
        {
            var role = GetController.Instance.GetRole(roleId);
            return role?.Name ?? "Неизвестно";
        }

        private void ApplyRoleColor(int roleId)
        {
            SolidColorBrush color;

            switch (roleId)
            {
                case 1: color = (SolidColorBrush)FindResource("OwnerColor"); break;
                case 2: color = (SolidColorBrush)FindResource("AdminColor"); break;
                case 3: color = (SolidColorBrush)FindResource("ModerColor"); break;
                case 4: color = (SolidColorBrush)FindResource("StudentColor"); break;
                case 5: color = (SolidColorBrush)FindResource("TeacherColor"); break;
                default: color = new SolidColorBrush(Colors.Gray); break;
            }

            // Меняем цвет рамки или фона
            var borderBrush = new SolidColorBrush(color.Color);
            tbId.Foreground = color;
            tbRole.Foreground = color;
        }

        private void SetEditMode(bool isEditing)
        {
            _isEditing = isEditing;

            // Включаем/отключаем поля ввода
            tbLastname.IsEnabled = isEditing && _canEdit;
            tbName.IsEnabled = isEditing && _canEdit;
            tbSurname.IsEnabled = isEditing && _canEdit;
            tbEmail.IsEnabled = isEditing && _canEdit;
            tbPhone.IsEnabled = isEditing && _canEdit;

            // Меняем стиль полей ввода
            var borderBrush = isEditing ? Brushes.Black : (Brush)FindResource("Hint");
            tbLastname.BorderBrush = borderBrush;
            tbName.BorderBrush = borderBrush;
            tbSurname.BorderBrush = borderBrush;
            tbEmail.BorderBrush = borderBrush;
            tbPhone.BorderBrush = borderBrush;

            // Показываем/скрываем кнопки
            EditButton.Visibility = isEditing ? Visibility.Collapsed : Visibility.Visible;
            SaveButton.Visibility = isEditing ? Visibility.Visible : Visibility.Collapsed;
            CancelButton.Visibility = isEditing ? Visibility.Visible : Visibility.Collapsed;

            // Если пользователь не может редактировать - скрываем кнопку редактирования
            if (!_canEdit && !isEditing)
            {
                EditButton.Visibility = Visibility.Collapsed;
                tbError.Text = "У вас недостаточно прав для редактирования этого пользователя";
                tbError.Visibility = Visibility.Visible;
            }
            else
            {
                tbError.Visibility = Visibility.Collapsed;
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (_canEdit)
            {
                SetEditMode(true);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Восстанавливаем исходные данные
            InitializeVariables(_user);
            SetEditMode(false);
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Валидация
                if (string.IsNullOrWhiteSpace(tbName.Text) ||
                    string.IsNullOrWhiteSpace(tbLastname.Text) ||
                    string.IsNullOrWhiteSpace(tbEmail.Text))
                {
                    MessageBox.Show("Имя, фамилия и email обязательны для заполнения",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Создаем обновленного пользователя
                var updatedUser = new Models.FullModels.User
                {
                    Id = _user.Id,
                    Name = tbName.Text.Trim(),
                    Lastname = tbLastname.Text.Trim(),
                    Surname = string.IsNullOrWhiteSpace(tbSurname.Text) ? null : tbSurname.Text.Trim(),
                    Email = tbEmail.Text.Trim(),
                    Phone_number = string.IsNullOrWhiteSpace(tbPhone.Text) ? null : tbPhone.Text.Trim(),
                    Id_role = _user.Id_role, // Роль не меняем
                    Is_active = _user.Is_active,
                    Password = _user.Password
                };

                var result = await PutController.Instance.AdminUpdateUser(updatedUser);

                if (result != null)
                {
                    _user = result;
                    InitializeVariables(result);
                    SetEditMode(false);

                    // Обновляем пользователя в кэше
                    await GetController.Instance.GetUsers(true);

                    MessageBox.Show("Данные пользователя успешно обновлены!",
                        "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Не удалось обновить данные пользователя",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show($"Вы уверены, что хотите удалить пользователя {_user.FullName}?\n\nЭто действие необратимо!",
                "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var deletedUser = await DeleteController.Instance.DeleteUser(_user.Id);

                    if (deletedUser != null)
                    {
                        // Обновляем кэш
                        await GetController.Instance.GetUsers(true);

                        MessageBox.Show("Пользователь успешно удален",
                            "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Возвращаемся на предыдущую страницу
                        if (NavigationService != null && NavigationService.CanGoBack)
                        {
                            NavigationService.GoBack();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Не удалось удалить пользователя",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}