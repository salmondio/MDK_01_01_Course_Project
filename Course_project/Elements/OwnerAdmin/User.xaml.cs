using Course_project;
using Course_project_wpf.Controllers;
using Course_project_wpf.Models.FullModels;
using Couse_project_RestAPI.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Course_project_wpf.Elements.OwnerAdmin
{
    /// <summary>
    /// Логика взаимодействия для User.xaml
    /// </summary>
    public partial class User : UserControl
    {
        private Models.FullModels.User _user;
        private bool _isAddMode;
        private bool _isOwner;

        public User()
        {
            InitializeComponent();
            _isAddMode = true;
            _isOwner = App.CurrentUser?.Role == "Owner";
            SetEditMode(false);
        }

        public User(Models.FullModels.User user)
        {
            _user = user;
            _isAddMode = false;
            _isOwner = App.CurrentUser?.Role == "Owner";

            InitializeResources(user);
            InitializeComponent();
            InitializeVariables(user);

            // Если не Owner, скрываем кнопки действий
            if (!_isOwner)
            {
                MainGrid.MouseEnter -= MainGrid_MouseEnter;
                MainGrid.MouseLeave -= MainGrid_MouseLeave;
                bdActions.Visibility = Visibility.Collapsed;
            }
        }

        private void InitializeVariables(Models.FullModels.User user)
        {
            // ID
            lbId.Content = user.Id.ToString();
            tbId.Text = user.Id.ToString();

            // ФИО
            lbLastname.Text = user.Lastname;
            lbName.Text = $" {user.Name}";
            lbSurname.Text = user.Surname ?? "";

            tbLastname.Text = user.Lastname;
            tbName.Text = user.Name;
            tbSurname.Text = user.Surname ?? "";

            // Email
            lbEmail.Text = user.Email;
            tbEmail.Text = user.Email;

            // Телефон
            lbPhone.Text = user.Phone_number ?? "Не указан";
            tbPhone.Text = user.Phone_number ?? "";

            // Роль
            lbRole.Text = GetRoleName(user.Id_role);

            // Статус активности
            SetActiveStatus(user.Is_active);
        }

        private void InitializeResources(Models.FullModels.User user)
        {
            try
            {
                SolidColorBrush themeBrush;
                SolidColorBrush darkThemeBrush;

                // Выбор цвета в зависимости от роли
                switch (user.Id_role)
                {
                    case 1: // Owner
                        themeBrush = (SolidColorBrush)FindResource("OwnerColor");
                        darkThemeBrush = new SolidColorBrush(Color.FromRgb(74, 20, 110));
                        break;
                    case 2: // Admin
                        themeBrush = (SolidColorBrush)FindResource("AdminColor");
                        darkThemeBrush = new SolidColorBrush(Color.FromRgb(21, 81, 162));
                        break;
                    case 3: // Moderator
                        themeBrush = (SolidColorBrush)FindResource("ModerColor");
                        darkThemeBrush = new SolidColorBrush(Color.FromRgb(40, 100, 45));
                        break;
                    case 4: // Student
                        themeBrush = (SolidColorBrush)FindResource("StudentColor");
                        darkThemeBrush = new SolidColorBrush(Color.FromRgb(0, 100, 110));
                        break;
                    case 5: // Teacher
                        themeBrush = (SolidColorBrush)FindResource("TeacherColor");
                        darkThemeBrush = new SolidColorBrush(Color.FromRgb(200, 70, 0));
                        break;
                    default:
                        themeBrush = (SolidColorBrush)FindResource("BaseColor");
                        darkThemeBrush = new SolidColorBrush(Color.FromRgb(200, 200, 200));
                        break;
                }

                Resources["LocalColorTheme"] = new SolidColorBrush(themeBrush.Color);
                Resources["LocalDarkColorTheme"] = new SolidColorBrush(darkThemeBrush.Color);
            }
            catch
            {
                // Цвета по умолчанию
                Resources["LocalColorTheme"] = new SolidColorBrush(Color.FromRgb(100, 100, 100));
                Resources["LocalDarkColorTheme"] = new SolidColorBrush(Color.FromRgb(70, 70, 70));
            }
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
                lbActive.Foreground = new SolidColorBrush(Color.FromRgb(200, 255, 200));
            }
            else
            {
                ActiveIndicator.Background = new SolidColorBrush(Color.FromRgb(244, 67, 54));
                lbActive.Text = "Заблокирован";
                lbActive.Foreground = new SolidColorBrush(Color.FromRgb(255, 200, 200));
            }
        }

        private void SetEditMode(bool isEditing)
        {
            if (isEditing)
            {
                // Скрываем лейблы, показываем поля ввода
                lbId.Visibility = Visibility.Collapsed;
                lbLastname.Visibility = Visibility.Collapsed;
                lbName.Visibility = Visibility.Collapsed;
                lbSurname.Visibility = Visibility.Collapsed;
                lbEmail.Visibility = Visibility.Collapsed;
                lbPhone.Visibility = Visibility.Collapsed;
                lbRole.Visibility = Visibility.Collapsed;

                tbId.Visibility = Visibility.Visible;
                tbLastname.Visibility = Visibility.Visible;
                tbName.Visibility = Visibility.Visible;
                tbSurname.Visibility = Visibility.Visible;
                tbEmail.Visibility = Visibility.Visible;
                tbPhone.Visibility = Visibility.Visible;
                cbRole.Visibility = Visibility.Visible;

                // Загружаем роли в комбобокс
                LoadRolesToComboBox();

                // Меняем кнопки
                EditButton.Visibility = Visibility.Collapsed;
                DeleteButton.Visibility = Visibility.Collapsed;
                SaveButton.Visibility = Visibility.Visible;
                CancelButton.Visibility = Visibility.Visible;
            }
            else
            {
                // Показываем лейблы, скрываем поля ввода
                lbId.Visibility = Visibility.Visible;
                lbLastname.Visibility = Visibility.Visible;
                lbName.Visibility = Visibility.Visible;
                lbSurname.Visibility = Visibility.Visible;
                lbEmail.Visibility = Visibility.Visible;
                lbPhone.Visibility = Visibility.Visible;
                lbRole.Visibility = Visibility.Visible;

                tbId.Visibility = Visibility.Collapsed;
                tbLastname.Visibility = Visibility.Collapsed;
                tbName.Visibility = Visibility.Collapsed;
                tbSurname.Visibility = Visibility.Collapsed;
                tbEmail.Visibility = Visibility.Collapsed;
                tbPhone.Visibility = Visibility.Collapsed;
                cbRole.Visibility = Visibility.Collapsed;

                // Меняем кнопки
                EditButton.Visibility = Visibility.Visible;
                DeleteButton.Visibility = Visibility.Visible;
                SaveButton.Visibility = Visibility.Collapsed;
                CancelButton.Visibility = Visibility.Collapsed;
            }
        }

        private async void LoadRolesToComboBox()
        {
            var roles = await GetController.Instance.GetRoles();
            if (roles != null)
            {
                cbRole.ItemsSource = roles;
                cbRole.DisplayMemberPath = "Name";
                cbRole.SelectedValuePath = "Id";

                // Выбираем текущую роль пользователя
                var currentRole = roles.FirstOrDefault(r => r.Id == _user.Id_role);
                if (currentRole != null)
                {
                    cbRole.SelectedItem = currentRole;
                }
            }
        }

        private void tbId_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Можно добавить валидацию ID
        }

        #region Mouse Events

        private void MainGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            if (_isOwner && !_isAddMode)
            {
                EditButton.Visibility = Visibility.Visible;
                DeleteButton.Visibility = Visibility.Visible;
            }
        }

        private void MainGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!SaveButton.IsVisible)
            {
                EditButton.Visibility = Visibility.Collapsed;
                DeleteButton.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        #region Actions

        private void Update(object sender, RoutedEventArgs e)
        {
            SetEditMode(true);
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            if (_isAddMode)
            {
                if (Parent is Panel parentPanel)
                {
                    parentPanel.Children.Remove(this);
                }
            }
            else
            {
                SetEditMode(false);
                // Восстанавливаем исходные данные
                InitializeVariables(_user);
            }
        }

        private async void Save(object sender, RoutedEventArgs e)
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
                    Id = int.Parse(tbId.Text),
                    Name = tbName.Text.Trim(),
                    Lastname = tbLastname.Text.Trim(),
                    Surname = string.IsNullOrWhiteSpace(tbSurname.Text) ? null : tbSurname.Text.Trim(),
                    Email = tbEmail.Text.Trim(),
                    Phone_number = string.IsNullOrWhiteSpace(tbPhone.Text) ? null : tbPhone.Text.Trim(),
                    Id_role = ((Role)cbRole.SelectedItem)?.Id ?? _user.Id_role,
                    Is_active = _user.Is_active,
                    Password = _user.Password
                };

                // Отправляем на сервер
                var result = await PutController.Instance.AdminUpdateUser(updatedUser);

                if (result != null)
                {
                    _user = result;
                    InitializeVariables(result);
                    InitializeResources(result);
                    SetEditMode(false);
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

        private async void Delete(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show($"Вы уверены, что хотите удалить пользователя {_user.FullName}?\nЭто действие необратимо.",
                "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var deletedUser = await DeleteController.Instance.DeleteUser(_user.Id);

                    if (deletedUser != null)
                    {
                        if (Parent is Panel parentPanel)
                        {
                            parentPanel.Children.Remove(this);
                        }
                        MessageBox.Show("Пользователь успешно удален",
                            "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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

        #endregion
    }
}