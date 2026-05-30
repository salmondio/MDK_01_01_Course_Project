using Course_project;
using Course_project_wpf.Common;
using Course_project_wpf.Models;
using Microsoft.AspNetCore.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Course_project_wpf.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindowAuthorization.xaml
    /// </summary>
    public partial class MainWindowAuthorization : Window
    {
        public MainWindowAuthorization()
        {
            InitializeComponent();
        }


        private void LogIn(object sender, RoutedEventArgs e)
        {
            LoginRequest loginRequest = new LoginRequest()
            {
                Email = tbEmail.Text,
                Password = pbPassword.Password
            };

            TryLogIn(loginRequest);
        }


        private void WindowKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                LoginRequest loginRequest = new LoginRequest()
                {
                    Email = tbEmail.Text,
                    Password = pbPassword.Password
                };

                TryLogIn(loginRequest);
            }
        }



        private async void TryLogIn(LoginRequest loginRequest)
        {
            btnLogIn.IsEnabled = false;

            try
            {
                var response = await ApiClient.PostAsync("api/User/Login", loginRequest);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseBody,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    App.JwtToken = loginResponse.Token;
                    App.CurrentUser = loginResponse.User;

                    // Устанавливаем токен для всех следующих запросов
                    ApiClient.SetAuthToken(loginResponse.Token);

                    OpenMainWindowByRole(loginResponse.User.Role);
                    this.Close();
                }
                else
                {
                    var error = (int)response.StatusCode == 500 ? "Ошибка сервера" : "Ошибка" + await response.Content.ReadAsStringAsync();
                    MessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btnLogIn.IsEnabled = true;
            }
        }


        private void OpenMainWindowByRole(string role)
        {
            Window mainWindow = role switch
            {
                "Student" => new MainWindowStudent(),
                "Teacher" => new MainWindowTeacher(),
                "Moderator" => new MainWindowModerator(),
                "Admin" => new MainWindowAdmin(),
                "Owner" => new MainWindowOwner(),
                _ => null
            };

            if (mainWindow != null)
            {
                mainWindow.Show();
            }
            else
            {
                MessageBox.Show($"Неизвестная роль: {role}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
