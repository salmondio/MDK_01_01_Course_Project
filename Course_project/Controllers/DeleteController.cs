using Course_project_wpf.Helpers;
using Course_project_wpf.Models.DTO;
using Course_project_wpf.Models.FullModels;
using Couse_project_RestAPI.Models;
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

        private void LogError(string message, object error)
        {
            System.Diagnostics.Debug.WriteLine($"{message}: {error}");
        }
    }
}