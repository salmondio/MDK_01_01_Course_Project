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

namespace Course_project_wpf.Controllers
{
    public class PutController
    {
        private static PutController? _instance;
        private static readonly object _instanceLock = new object();
        public event Action? DataChanged;
        private bool _isLoading;

        public static PutController? Instance
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
            Действия Овнера 
        */

        // Оценки
        public async Task<Evaluation?> UpdateEvaluation(Evaluation updatedEvaluation)
        {
            return await ExecuteRequestAsync<Evaluation>("api/Evaluation/Owner/Update", updatedEvaluation, "обновить оценку");
        }

        /*
            Действия Админа
        */

        // Пользователь
        public async Task<User?> ChangeActiveUser(int id)
        {
            return await ExecuteRequestAsync<User>(
                "api/User/Admin/ChangeActive",
                id,
                "обновить активность пользователя"
                );
        }

        public async Task<User?> UpdateUser(User user)
        {
            return await ExecuteRequestAsync<User>(
                "api/User/Admin/Update",
                user,
                "обновить пользователя"
                );
        }

        // Жалобы
        public async Task<Report?> ChangeStatusReport(int id)
        {
            return await ExecuteRequestAsync<Report>(
                "api/Report/ChangeStatus",
                id,
                "обновить статус жалобы"
                );
        }


        // Отзывы
        public async Task<Review?> ChangeStatusReview(int id)
        {
            return await ExecuteRequestAsync<Review>(
                "api/Review/ChangeStatus",
                id,
                "обновить статус отзыва"
                );
        }


        /*
            Вспомогательные методы 
        */

        // Выполненеие запроса, принимающего и возвращающего модель
        private async Task<T?> ExecuteRequestAsync<T>(string endpoint, T putData, string dataName)
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
                    LogError($"Не удалось выполнить дествие: {dataName}", response);
                }
            }
            catch (Exception ex)
            {
                LogError($"Не удалось выполнить дествие: {dataName}", ex);
            }
            finally
            {
                _isLoading = false;
            }

            return default;
        }

        // Выполненеие запроса, принимающего только id и возвращающего модель
        private async Task<T?> ExecuteRequestAsync<T>(string endpoint, int id, string dataName)
        {
            if (_isLoading)
                return default;

            _isLoading = true;

            try
            {
                var response = await ApiClient.PutAsync(endpoint, id);

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
                    LogError($"Не удалось выполнить дествие: {dataName}", response);
                }
            }
            catch (Exception ex)
            {
                LogError($"Не удалось выполнить дествие: {dataName}", ex);
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
