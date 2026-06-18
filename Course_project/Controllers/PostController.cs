using System;
using System.Text.Json;
using System.Threading.Tasks;
using Course_project_wpf.Helpers;
using Course_project_wpf.Models.FullModels;
using Course_project_wpf.Models.DTO;

namespace Course_project_wpf.Controllers
{
    public class PostController
    {
        private static PostController? _instance;
        private static readonly object _instanceLock = new object();
        public event Action? DataChanged;
        private bool _isLoading;

        public static PostController Instance
        {
            get
            {
                lock (_instanceLock)
                {
                    if (_instance == null)
                        _instance = new PostController();
                    return _instance;
                }
            }
        }

        /*
            Действия Owner - работают с полными моделями
        */

        public async Task<Evaluation?> AddEvaluation(Evaluation newEvaluation)
        {
            return await ExecutePostRequestAsync<Evaluation, Evaluation>("api/Evaluation/Owner/Add", newEvaluation, "оценку");
        }

        public async Task<Discipline?> AddDiscipline(Discipline discipline)
        {
            return await ExecutePostRequestAsync<Discipline, Discipline>("api/Discipline/Owner/Add", discipline, "дисциплину");
        }

        public async Task<Role?> AddRole(Role role)
        {
            return await ExecutePostRequestAsync<Role, Role>("api/Role/Owner/Add", role, "роль");
        }

        public async Task<Report?> AddReport(Report report)
        {
            return await ExecutePostRequestAsync<Report, Report>("api/Report/Owner/Add", report, "жалобу");
        }

        public async Task<Review?> AddReview(Review review)
        {
            return await ExecutePostRequestAsync<Review, Review>("api/Review/Owner/Add", review, "отзыв");
        }

        /*
            Действия Admin - работают с полными моделями
        */

        public async Task<TeacherDiscipline?> AddTeacherDiscipline(TeacherDiscipline teacherDiscipline)
        {
            return await ExecutePostRequestAsync<TeacherDiscipline, TeacherDiscipline>("api/TeacherDiscipline/Admin/Add", teacherDiscipline, "связь преподавателя и дисциплины");
        }

        public async Task<User?> AddUser(User user)
        {
            return await ExecutePostRequestAsync<User, User>("api/User/Admin/Add", user, "пользователя");
        }

        /*
            Действия Student - используют DTO
        */

        public async Task<EvaluationCreateDTO?> AddEvaluationByStudent(EvaluationCreateDTO evaluation)
        {
            return await ExecutePostRequestAsync<EvaluationCreateDTO, EvaluationCreateDTO>("api/Evaluation/Add", evaluation, "оценку");
        }

        public async Task<ReportCreateDTO?> AddReportByStudent(ReportCreateDTO report)
        {
            return await ExecutePostRequestAsync<ReportCreateDTO, ReportCreateDTO>("api/Report/Add", report, "жалобу");
        }

        public async Task<ReviewCreateDTO?> AddReviewByStudent(ReviewCreateDTO review)
        {
            return await ExecutePostRequestAsync<ReviewCreateDTO, ReviewCreateDTO>("api/Review/Add", review, "отзыв");
        }

        /*
            Вспомогательные методы
        */

        private async Task<TResponse?> ExecutePostRequestAsync<TRequest, TResponse>(string endpoint, TRequest postData, string dataName)
        {
            if (_isLoading)
                return default;

            _isLoading = true;

            try
            {
                var response = await ApiClient.PostAsync(endpoint, postData);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<TResponse>(responseBody, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (data != null)
                    {
                        DataChanged?.Invoke();
                        return data;
                    }
                }
                else
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    LogError($"Не удалось добавить {dataName}. Status: {response.StatusCode}, Error: {errorBody}", response);
                }
            }
            catch (Exception ex)
            {
                LogError($"Не удалось добавить {dataName}", ex);
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