using Course_project_wpf.Helpers;
using Course_project_wpf.Models.DTO;
using Course_project_wpf.Models.FullModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Course_project_wpf.Controllers
{
    public class DeleteController
    {
        private static DeleteController? _instance;
        private static readonly object _instanceLock = new object();
        public event Action? DataChanged;
        private bool _isLoading;

        public static DeleteController Instance
        {
            get
            {
                lock (_instanceLock)
                {
                    if (_instance == null)
                        _instance = new DeleteController();
                    return _instance;
                }
            }
        }

        /*
            Действия Owner
        */

        // Удаление пользователя
        public async Task<User?> DeleteUser(int id)
        {
            return await ExecuteDeleteRequestAsync<User>($"api/User/Delete/{id}", "удалить пользователя");
        }

        // Удаление дисциплины
        public async Task<Discipline?> DeleteDiscipline(int id)
        {
            return await ExecuteDeleteRequestAsync<Discipline>($"api/Discipline/Owner/Delete/{id}", "удалить дисциплину");
        }

        // Удаление роли
        public async Task<Role?> DeleteRole(int id)
        {
            return await ExecuteDeleteRequestAsync<Role>($"api/Role/Owner/Delete/{id}", "удалить роль");
        }

        // Удаление оценки
        public async Task<Evaluation?> DeleteEvaluation(int idStudent, int idTeacher)
        {
            return await ExecuteDeleteRequestAsync<Evaluation>($"api/Evaluation/Owner/Delete/{idStudent}/{idTeacher}", "удалить оценку");
        }

        // Удаление жалобы
        public async Task<Report?> DeleteReport(int id)
        {
            return await ExecuteDeleteRequestAsync<Report>($"api/Report/Owner/Delete/{id}", "удалить жалобу");
        }

        // Удаление отзыва
        public async Task<Review?> DeleteReview(int id)
        {
            return await ExecuteDeleteRequestAsync<Review>($"api/Review/Owner/Delete/{id}", "удалить отзыв");
        }

        /*
            Действия Admin
        */

        // Удаление связи преподаватель-дисциплина
        public async Task<TeacherDiscipline?> DeleteTeacherDiscipline(int id)
        {
            return await ExecuteDeleteRequestAsync<TeacherDiscipline>($"api/TeacherDiscipline/Admin/Delete/{id}", "удалить связь преподавателя и дисциплины");
        }

        

        /*
            Вспомогательные методы
        */

        // DELETE-запрос без тела, возвращающий удаленный объект
        private async Task<T?> ExecuteDeleteRequestAsync<T>(string endpoint, string actionName)
        {
            if (_isLoading)
                return default;

            _isLoading = true;

            try
            {
                var response = await ApiClient.DeleteAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<T>(responseBody);

                    if (data != null)
                    {
                        DataChanged?.Invoke();
                        return data;
                    }
                }
                else
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    LogError($"Не удалось выполнить действие: {actionName}. Status: {response.StatusCode}, Error: {errorBody}", response);
                }
            }
            catch (Exception ex)
            {
                LogError($"Не удалось выполнить действие: {actionName}", ex);
            }
            finally
            {
                _isLoading = false;
            }

            return default;
        }

        

        private void LogError(string message, object error)
        {
            System.Diagnostics.Debug.WriteLine($"{message}: {error}");
        }
    }
}