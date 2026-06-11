using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Course_project_wpf.Helpers;
using Course_project_wpf.Models.FullModels;
using Couse_project_RestAPI.Models;

namespace Course_project_wpf.Controllers
{
    public class PostController
    {
        private static PostController? _instance;
        private static readonly object _instanceLock = new object();
        public event Action? DataChanged;
        private bool _isLoading;

        public static PostController? Instance
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
            Действия Овнера 
        */

        // Оценки
        public async Task<Evaluation?> AddEvaluation(Evaluation newEvaluation)
        {
            return await ExecuteRequestAsync("api/Evaluation/Owner/Add", newEvaluation, "ценка");
        }


        /*
            Действия Админа
        */

        // Преподаватель-Дисциплина
        public async Task<TeacherDiscipline?> AddTeacherDiscipline(TeacherDiscipline newTeacherDiscipline)
        {
            return await ExecuteRequestAsync<TeacherDiscipline>("api/TeacherDiscipline/Admin/Add", newTeacherDiscipline, "дисциплина препода");
        }


        // Пользователь
        public async Task<User?> AddUser(User newUser)
        {
            return await ExecuteRequestAsync<User>("api/User/Admin/Add", newUser, "пользователь");
        }


        /*
            Вспомогательные методы
        */
        private async Task<T?> ExecuteRequestAsync<T>(string endpoint, T postData, string dataName)
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
                    var data = JsonSerializer.Deserialize<T>(responseBody);

                    if (data != null)
                    {
                        //postData(data);
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
                _isLoading = false;
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
