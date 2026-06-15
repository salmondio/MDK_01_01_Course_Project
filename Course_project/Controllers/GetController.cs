using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using Course_project_wpf.Helpers;
using Course_project_wpf.Models.FullModels;
using Course_project_wpf.Models.DTO;
using Couse_project_RestAPI.Models;

namespace Course_project_wpf.Controllers
{
    public class GetController
    {
        private static GetController? _instance;
        private static readonly object _lock = new object();

        public event Action? DataChanged;
        public bool IsLoading { get; private set; }

        // Списки для хранения полных моделей
        public List<Discipline>? Disciplines { get; private set; }
        public List<Role>? Roles { get; private set; }
        public List<Evaluation>? Evaluations { get; private set; }
        public List<Report>? Reports { get; private set; }
        public List<Review>? Reviews { get; private set; }
        public List<User>? Users { get; private set; }

        // Списки для DTO (если нужно кэшировать)
        public List<UserDTO>? Teachers { get; private set; }

        private GetController() { }

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

        /*
            Действия Admin
        */

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

        public async Task<List<Role>?> GetRoles(bool forceRefresh = false)
        {
            if (Roles != null && !forceRefresh)
                return Roles;

            return await ExecuteRequestAsync<List<Role>>(
                "/api/Role/Admin/List",
                data => Roles = data,
                "ролей"
            );
        }

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

        /*
            Действия Student (возвращают DTO)
        */

        public async Task<List<UserDTO>?> GetTeachers(bool forceRefresh = false)
        {
            if (Teachers != null && !forceRefresh)
                return Teachers;

            return await ExecuteRequestAsync<List<UserDTO>>(
                "/api/User/ListTeacher",
                data => Teachers = data,
                "преподавателей"
            );
        }

        public async Task<UserDTO?> GetTeacher(int id)
        {
            return await ExecuteSingleRequestAsync<UserDTO>($"/api/User/ListTeacher/{id}", "преподавателя");
        }

        public async Task<List<EvaluationDTO>?> GetEvaluationsForCurrentUser(bool forceRefresh = false)
        {
            return await ExecuteRequestAsync<List<EvaluationDTO>>(
                "/api/Evaluation/List",
                null,
                "оценок для текущего пользователя",
                false
            );
        }

        public async Task<EvaluationDTO?> GetEvaluationForCurrentUser(int idStudent, int idTeacher)
        {
            return await ExecuteSingleRequestAsync<EvaluationDTO>($"/api/Evaluation/{idStudent}/{idTeacher}", "оценки");
        }

        public async Task<List<ReportDTO>?> GetReportsForCurrentStudent(bool forceRefresh = false)
        {
            return await ExecuteRequestAsync<List<ReportDTO>>(
                "/api/Report/List",
                null,
                "жалоб текущего студента",
                false
            );
        }

        public async Task<ReportDTO?> GetReportForCurrentStudent(int id)
        {
            return await ExecuteSingleRequestAsync<ReportDTO>($"/api/Report/{id}", "жалобы");
        }

        public async Task<List<ReviewDTO>?> GetReviewsForCurrentStudent(bool forceRefresh = false)
        {
            return await ExecuteRequestAsync<List<ReviewDTO>>(
                "/api/Review/List",
                null,
                "отзывов текущего студента",
                false
            );
        }

        public async Task<ReviewDTO?> GetReviewForCurrentStudent(int id)
        {
            return await ExecuteSingleRequestAsync<ReviewDTO>($"/api/Review/{id}", "отзыва");
        }

        /*
            Вспомогательные методы
        */

        private async Task<T?> ExecuteRequestAsync<T>(string endpoint, Action<T>? updateData, string dataName, bool useCache = true)
        {
            if (IsLoading)
                return default;

            IsLoading = true;

            try
            {
                var response = await ApiClient.GetAsync(endpoint);
                if (response == null) return default;

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<T>(responseBody, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (data != null)
                    {
                        updateData?.Invoke(data);
                        DataChanged?.Invoke();
                        return data;
                    }
                }
                else
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    LogError($"Не удалось получить список {dataName}. Status: {response.StatusCode}, Error: {errorBody}", response);
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

        private async Task<T?> ExecuteSingleRequestAsync<T>(string endpoint, string dataName)
        {
            try
            {
                var response = await ApiClient.GetAsync(endpoint);
                if (response == null) return default;

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<T>(responseBody, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return data;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return default;
                }
                else
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    LogError($"Не удалось получить {dataName}. Status: {response.StatusCode}, Error: errorBody", response);
                }
            }
            catch (Exception ex)
            {
                LogError($"Не удалось получить {dataName}", ex);
            }

            return default;
        }

        private void LogError(string message, object error)
        {
            System.Diagnostics.Debug.WriteLine($"{message}: {error}");
        }

        public void ClearData()
        {
            Disciplines = null;
            Roles = null;
            Evaluations = null;
            Reports = null;
            Reviews = null;
            Users = null;
            Teachers = null;
            DataChanged?.Invoke();
        }

        // Методы для получения одиночных объектов из кэша
        public Discipline? GetDiscipline(int id) => Disciplines?.FirstOrDefault(x => x.Id == id);
        public Role? GetRole(int id) => Roles?.FirstOrDefault(x => x.Id == id);
        public Evaluation? GetEvaluation(int idStudent, int idTeacher) => Evaluations?.FirstOrDefault(x => x.IdStudent == idStudent && x.IdTeacher == idTeacher);
        public Report? GetReport(int id) => Reports?.FirstOrDefault(x => x.Id == id);
        public Review? GetReview(int id) => Reviews?.FirstOrDefault(x => x.Id == id);
        public User? GetUser(int id) => Users?.FirstOrDefault(x => x.Id == id);
        public UserDTO? GetTeacherFromCache(int id) => Teachers?.FirstOrDefault(x => x.Id == id);
    }
}