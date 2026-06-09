using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Course_project_wpf.Helpers;
using Course_project_wpf.Models.FullModels;
using System.Windows;

namespace Course_project_wpf.Controllers
{
    public class PutController
    {
        /*
            Действия Админа
        */
        public User? ChangeActiveUser(int id)
        {
            return GetUser(id);
        }

        public async Task<User?> UpdateUser(User user)
        {
            User updatedUser = new User();

            var response = await ApiClient.PutAsync("api/User/AdminUpdate", user);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                updatedUser = JsonSerializer.Deserialize<User>(responseBody);
            }
            else
                MessageBox.Show("Ошибка: Не удалось обновить пользователя: " + response.RequestMessage + " код ошибки: " + response.StatusCode, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

            return updatedUser;
        }
    }
}
