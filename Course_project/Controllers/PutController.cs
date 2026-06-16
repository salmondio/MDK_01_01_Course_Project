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

        /*
            PATCH-запросы (ChangeActive, ChangeStatus и т.д.)
            Они используют PATCH, не DELETE
        */

        // Активация/деактивация пользователя (Admin)
        public async Task<bool?> ChangeUserActive(int id)
        {
            return await ExecutePatchRequestAsync($"api/User/Admin/ChangeActive/{id}", "изменить активность пользователя");
        }

        // Смена пароля пользователя
        public async Task<bool?> ChangePassword(string newPassword)
        {
            return await ExecutePatchRequestAsync($"api/User/ChangePassword", newPassword, "сменить пароль");
        }

        // Изменение статуса жалобы (Admin/Moderator)
        public async Task<Report?> ChangeReportStatus(int id, MessageStatus status)
        {
            return await ExecutePatchRequestWithBodyAsync<Report, MessageStatus>($"api/Report/UpdateStatus/{id}", status, "изменить статус жалобы");
        }

        // Изменение статуса отзыва (Admin/Moderator)
        public async Task<Review?> ChangeReviewStatus(int id, MessageStatus status)
        {
            return await ExecutePatchRequestWithBodyAsync<Review, MessageStatus>($"api/Review/UpdateStatus/{id}", status, "изменить статус отзыва");
        }

        // Активация/деактивация жалобы студентом
        public async Task<ReportDTO?> ChangeReportActive(int id)
        {
            return await ExecutePatchRequestAsync<ReportDTO>($"api/Report/ChangeActive/{id}", "изменить активность жалобы");
        }

        // Активация/деактивация отзыва студентом
        public async Task<ReviewDTO?> ChangeReviewActive(int id)
        {
            return await ExecutePatchRequestAsync<ReviewDTO>($"api/Review/ChangeActive/{id}", "изменить активность отзыва");
        }

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

        // PATCH-запрос без тела (только URL)
        private async Task<bool?> ExecutePatchRequestAsync(string endpoint, string actionName)
        {
            if (_isLoading)
                return null;

            _isLoading = true;

            try
            {
                var response = await ApiClient.PatchAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    DataChanged?.Invoke();
                    return true;
                }
                else
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    LogError($"Не удалось выполнить действие: {actionName}. Status: {response.StatusCode}, Error: {errorBody}", response);
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogError($"Не удалось выполнить действие: {actionName}", ex);
                return null;
            }
            finally
            {
                _isLoading = false;
            }
        }

        // PATCH-запрос с телом, возвращающий объект
        private async Task<TResponse?> ExecutePatchRequestAsync<TResponse>(string endpoint, string actionName)
        {
            if (_isLoading)
                return default;

            _isLoading = true;

            try
            {
                var response = await ApiClient.PatchAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<TResponse>(responseBody);

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

        // PATCH-запрос с телом, возвращающий объект
        private async Task<TResponse?> ExecutePatchRequestWithBodyAsync<TResponse, TBody>(string endpoint, TBody body, string actionName)
        {
            if (_isLoading)
                return default;

            _isLoading = true;

            try
            {
                var response = await ApiClient.PatchAsync(endpoint, body);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<TResponse>(responseBody);

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

        // PATCH-запрос с телом, возвращающий bool
        private async Task<bool?> ExecutePatchRequestAsync<TBody>(string endpoint, TBody body, string actionName)
        {
            if (_isLoading)
                return null;

            _isLoading = true;

            try
            {
                var response = await ApiClient.PatchAsync(endpoint, body);

                if (response.IsSuccessStatusCode)
                {
                    DataChanged?.Invoke();
                    return true;
                }
                else
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    LogError($"Не удалось выполнить действие: {actionName}. Status: {response.StatusCode}, Error: {errorBody}", response);
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogError($"Не удалось выполнить действие: {actionName}", ex);
                return null;
            }
            finally
            {
                _isLoading = false;
            }
        }

        // Логирование
        private void LogError(string message, object error)
        {
            System.Diagnostics.Debug.WriteLine($"{message}: {error}");
        }
    }
}