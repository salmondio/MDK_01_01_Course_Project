using Course_project;
using Course_project_wpf.Models;
using Course_project_wpf.Windows;
using Microsoft.AspNetCore.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace Course_project_wpf.Helpers
{
    public static class AuthorizationHelper
    {
        public static async Task<bool> TryLogIn(LoginRequest loginRequest)
        {
            try
            {
                // Отправляем запрос
                var response = await ApiClient.PostAsync("api/User/Login", loginRequest);

                // Возвращен успешный статус
                if (response.IsSuccessStatusCode)
                {
                    // Читаем ответ
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseBody,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    // Устанавливаем текущий токен и пользователя
                    App.JwtToken = loginResponse.Token;
                    App.CurrentUser = loginResponse.User;

                    // Устанавливаем токен для всех следующих запросов
                    ApiClient.SetAuthToken(loginResponse.Token);

                    // Открываем окно, соответсвующее роли
                    OpenMainWindowByRole(loginResponse.User.Role);
                    return true;
                }

                // Обрабатываем и выводим ошибку
                var error = (int)response.StatusCode == 500 ? "Ошибка сервера" : "Ошибка" + await response.Content.ReadAsStringAsync();
                MessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private static void OpenMainWindowByRole(string role)
        {
            Window? mainWindow = role switch
            {
                "Student" => new MainWindowStudent(),
                "Teacher" => new MainWindowTeacher(),
                "Moderator" => new MainWindowModerator(),
                "Admin" => new MainWindowAdmin(),
                "Owner" => new MainWindowOwner(),
                _ => null
            };

            if (mainWindow != null)
                mainWindow.Show();
            else
                MessageBox.Show($"Неизвестная роль: {role}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
