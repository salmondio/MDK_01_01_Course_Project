using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using Course_project_wpf.Helpers;
using Course_project_wpf.Models.FullModels;
using Couse_project_RestAPI.Models;

namespace Course_project_wpf.Controllers
{
    public class GetController
    {
        private static GetController? _instance;
        private static readonly object _lock = new object();

        // Событие для уведомления об изменении данных
        public event Action? DataChanged;

        // Приватный конструктор
        private GetController() { }

        // Синглтон
        public static GetController Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new GetController();
                    return _instance;
                }
            }
        }

        // Списки (теперь не статические)
        public List<Discipline>? Disciplines { get; private set; }
        public List<Role>? Roles { get; private set; }
        public List<Evaluation>? Evaluations { get; private set; }
        public List<Report>? Reports { get; private set; }
        public List<Review>? Reviews { get; private set; }
        public List<User>? Users { get; private set; }

        // Флаг загрузки
        public bool IsLoading { get; private set; }

        // Очистка данных (например, при выходе из аккаунта)
        public void ClearData()
        {
            Disciplines = null;
            Roles = null;
            Evaluations = null;
            Reports = null;
            Reviews = null;
            Users = null;
            DataChanged?.Invoke();
        }

        /*
            Действия админа
         */

        // Дисциплины
        public async Task<List<Discipline>?> GetDisciplines(bool forceRefresh = false)
        {
            if (Disciplines != null && !forceRefresh)
                return Disciplines;

            return await ExecuteRequestAsync<List<Discipline>>(
                "/api/Discipline/List",
                data => Disciplines = data,
                "дисциплин"
            );
        }

        public Discipline? GetDiscipline(int id)
        {
            return Disciplines?.FirstOrDefault(x => x.Id == id);
        }

        // Роли
        public async Task<List<Role>?> GetRoles(bool forceRefresh = false)
        {
            if (Roles != null && !forceRefresh)
                return Roles;

            return await ExecuteRequestAsync<List<Role>>(
                "/api/Roles/Admin/List",
                data => Roles = data,
                "ролей"
            );
        }

        public Role? GetRole(int id)
        {
            return Roles?.FirstOrDefault(x => x.Id == id);
        }

        // Оценки
        public async Task<List<Evaluation>?> GetEvaluations(bool forceRefresh = false)
        {
            if (Evaluations != null && !forceRefresh)
                return Evaluations;

            return await ExecuteRequestAsync<List<Evaluation>>(
                "/api/Evaluation/Admin/List",
                data => Evaluations = data,
                "оценок"
            );
        }

        public Evaluation? GetEvaluation(int idStudent, int idTeacher)
        {
            return Evaluations?.FirstOrDefault(x => x.IdStudent == idStudent && x.IdTeacher == idTeacher);
        }

        // Жалобы
        public async Task<List<Report>?> GetReports(bool forceRefresh = false)
        {
            if (Reports != null && !forceRefresh)
                return Reports;

            return await ExecuteRequestAsync<List<Report>>(
                "/api/Report/Admin/List",
                data => Reports = data,
                "жалоб"
            );
        }

        public Report? GetReport(int id)
        {
            return Reports?.FirstOrDefault(x => x.Id == id);
        }

        // Отзывы
        public async Task<List<Review>?> GetReviews(bool forceRefresh = false)
        {
            if (Reviews != null && !forceRefresh)
                return Reviews;

            return await ExecuteRequestAsync<List<Review>>(
                "/api/Review/Admin/List",
                data => Reviews = data,
                "отзывов"
            );
        }

        public Review? GetReview(int id)
        {
            return Reviews?.FirstOrDefault(x => x.Id == id);
        }

        // Пользователи
        public async Task<List<User>?> GetUsers(bool forceRefresh = false)
        {
            if (Users != null && !forceRefresh)
                return Users;

            return await ExecuteRequestAsync<List<User>>(
                "/api/User/Admin/List",
                data => Users = data,
                "пользователей"
            );
        }

        public User? GetUser(int id)
        {
            return Users?.FirstOrDefault(x => x.Id == id);
        }

        /*
            Общие действия
         */

        // Общий метод для выполнения запросов (DRY принцип)
        private async Task<T?> ExecuteRequestAsync<T>(string endpoint, Action<T> updateData, string dataName)
        {
            if (IsLoading)
                return default;

            IsLoading = true;

            try
            {
                var response = await ApiClient.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<T>(responseBody);

                    if (data != null)
                    {
                        updateData(data);
                        DataChanged?.Invoke();
                        return data;
                    }
                }
                else
                {
                    // Логирование ошибки без MessageBox
                    LogError($"Не удалось получить список {dataName}", response);
                }
            }
            catch (Exception ex)
            {
                LogError($"Не удалось получить список {dataName}", ex);
            }
            finally
            {
                IsLoading = false;
            }

            return default;
        }

        private void LogError(string message, object error)
        {
            // Используйте логгер вместо MessageBox
            System.Diagnostics.Debug.WriteLine($"{message}: {error}");
        }
    }
}