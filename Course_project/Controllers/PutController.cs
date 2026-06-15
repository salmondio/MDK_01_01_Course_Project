using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Course_project_wpf.Helpers;
using Course_project_wpf.Models.FullModels;
using System.Windows;
using Couse_project_RestAPI.Models;
using System.Net.Http;
using System.Threading.Tasks;
using Course_project_wpf.Models.DTO;

namespace Course_project_wpf.Controllers
{
    public class PutController
    {
        private static PutController? _instance;
        private static readonly object _instanceLock = new object();
        public event Action? DataChanged;
        private bool _isLoading;

        public static PutController Instance
        {
            get
            {
                lock (_instanceLock)
                {
                    if (_instance == null)
                        _instance = new PutController();
                    return _instance;
                }
            }
        }

        /*
            Действия Owner 
        */

        // Оценки
        public async Task<Evaluation?> UpdateEvaluation(Evaluation updatedEvaluation)
        {
            return await ExecutePutRequestAsync<Evaluation>("api/Evaluation/Owner/Update", updatedEvaluation, "обновить оценку");
        }

        // Дисциплины
        public async Task<Discipline?> UpdateDiscipline(Discipline discipline)
        {
            return await ExecutePutRequestAsync<Discipline>("api/Discipline/Owner/Update", discipline, "обновить дисциплину");
        }

        // Роли
        public async Task<Role?> UpdateRole(Role role)
        {
            return await ExecutePutRequestAsync<Role>("api/Role/Owner/Update", role, "обновить роль");
        }

        // Жалобы (Owner)
        public async Task<Report?> OwnerUpdateReport(Report report)
        {
            return await ExecutePutRequestAsync<Report>("api/Report/Owner/Update", report, "обновить жалобу");
        }

        // Отзывы (Owner)
        public async Task<Review?> OwnerUpdateReview(Review review)
        {
            return await ExecutePutRequestAsync<Review>("api/Review/Owner/Update", review, "обновить отзыв");
        }

        /*
            Действия Admin
        */

        // Пользователь (Admin)
        public async Task<User?> AdminUpdateUser(User user)
        {
            return await ExecutePutRequestAsync<User>("api/User/Admin/Update", user, "обновить пользователя");
        }

        // Преподаватель-Дисциплина (Admin)
        public async Task<TeacherDiscipline?> UpdateTeacherDiscipline(TeacherDiscipline teacherDiscipline)
        {
            return await ExecutePutRequestAsync<TeacherDiscipline>("api/TeacherDiscipline/Admin/Update", teacherDiscipline, "обновить связь преподавателя и дисциплины");
        }

        /*
            Действия пользователя
        */

        // Обновление информации о себе
        public async Task<UserDTO?> UpdateSelf(UserDTO user)
        {
            return await ExecutePutRequestAsync<UserDTO>("api/User/Update", user, "обновить свои данные");
        }

        // Обновление оценки студентом
        public async Task<EvaluationCreateDTO?> UpdateEvaluationByStudent(EvaluationCreateDTO evaluation)
        {
            return await ExecutePutRequestAsync<EvaluationCreateDTO>("api/Evaluation/Update", evaluation, "обновить оценку");
        }

        // Обновление жалобы студентом (ChangeActive - это PATCH, но он в DeleteController)
        // Оставлено в DeleteController

        /*
            Вспомогательные методы 
        */

        // Выполнение PUT-запроса, принимающего и возвращающего модель
        private async Task<T?> ExecutePutRequestAsync<T>(string endpoint, T putData, string dataName)
        {
            if (_isLoading)
                return default;

            _isLoading = true;

            try
            {
                var response = await ApiClient.PutAsync(endpoint, putData);

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
                    LogError($"Не удалось выполнить действие: {dataName}. Status: {response.StatusCode}, Error: {errorBody}", response);
                }
            }
            catch (Exception ex)
            {
                LogError($"Не удалось выполнить действие: {dataName}", ex);
            }
            finally
            {
                _isLoading = false;
            }

            return default;
        }

        // Логирование
        private void LogError(string message, object error)
        {
            System.Diagnostics.Debug.WriteLine($"{message}: {error}");
        }
    }
}