using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Course_project_wpf.Helpers;
using System.Windows;
using Course_project_wpf.Models.FullModels;
using Couse_project_RestAPI.Models;

namespace Course_project_wpf.Controllers
{
    public class GetController
    {
        // Списки
        public static List<Discipline>? Disciplines { get; private set; }
        public static List<Role>? Roles { get; private set; }
        public static List<Evaluation>? Evaluations { get; private set; }
        public static List<Report>? Reports { get; private set; }
        public static List<Review>? Reviews { get; private set; }
        public static List<User>? Users { get; private set; }


        /*
            Запросы Админа
        */
        // Дисциплины
        public async Task<List<Discipline>> GetDisciplines()
        {
            try
            {
                var response = await ApiClient.GetAsync("/api/Discipline/List");

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var disciplineBody = JsonSerializer.Deserialize<List<Discipline>>(responseBody);

                    Disciplines = disciplineBody?.ToList();
                }
                else
                    MessageBox.Show("Ошибка: Не удалось получить список дисциплин: " + response.RequestMessage + " код ошибки: " + response.StatusCode, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: Не удалось получить список дисциплин. " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return Disciplines;
        }

        public Discipline? GetDiscipline(int id)
        {
            return Disciplines.FirstOrDefault(x => x.Id == id);
        }

        // Роли
        public async Task<List<Role>> GetRoles()
        {
            try
            {
                var response = await ApiClient.GetAsync("/api/Roles/Admin/List");

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var rolesBosy = JsonSerializer.Deserialize<List<Role>>(responseBody);

                    Roles = rolesBosy?.ToList();
                }
                else
                    MessageBox.Show("Ошибка: Не удалось получить список ролей: " + response.RequestMessage + " код ошибки: " + response.StatusCode, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: Не удалось получить список ролей. " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }


            return Roles;
        }

        public Role? GetRole(int id)
        {
            return Roles.FirstOrDefault(x => x.Id == id);
        }

        // Оценки
        public async Task<List<Evaluation>?> GetEvaluations()
        {
            try
            {
                var response = await ApiClient.GetAsync("/api/Evaluation/Admin/List");

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var evaluationsBody = JsonSerializer.Deserialize<List<Evaluation>>(responseBody);

                    Evaluations = evaluationsBody?.ToList();
                }
                else
                    MessageBox.Show("Ошибка: Не удалось получить список оценок: " + response.RequestMessage + " код ошибки: " + response.StatusCode, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: Не удалось получить список оценок: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return Evaluations;
        }

        public Evaluation? GetEvaluation(int idStudent, int idTeacher)
        {
            return Evaluations?.FirstOrDefault(x => x.IdStudent == idStudent && x.IdTeacher == idTeacher);
        }

        // Жалобы
        public async Task<List<Report>> GetReports()
        {
            try
            {
                var response = await ApiClient.GetAsync("/api/Report/Admin/List");

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var reportsBody = JsonSerializer.Deserialize<List<Report>>(responseBody);

                    Reports = reportsBody?.ToList();
                }
                else
                    MessageBox.Show("Ошибка: Не удалось получить список жалоб: " + response.RequestMessage + " код ошибки: " + response.StatusCode, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: Не удалось получить список жалоб: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return Reports;
        }

        public Report? GetReport(int id)
        {
            return Reports.FirstOrDefault(x => x.Id == id);
        }

        // Отзывы
        public async Task<List<Review>> GetReviews()
        {
            try
            {
                var response = await ApiClient.GetAsync("/api/Review/Admin/List");

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var reviewBody = JsonSerializer.Deserialize<List<Review>>(responseBody);

                    Reviews = reviewBody?.ToList();
                }
                else
                    MessageBox.Show("Ошибка: Не удалось получить список отзывов: " + response.RequestMessage + " код ошибки: " + response.StatusCode, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: Не удалось получить список отзывов: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return Reviews;
        }

        public Review? GetReview(int id)
        {
            return Reviews.FirstOrDefault(x => x.Id == id);
        }

        // Пользователи
        public async Task<List<User>?> GetUsers()
        {

            var response = await ApiClient.GetAsync("api/User/Admin/List");

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var usersResponse = JsonSerializer.Deserialize<List<User>>(responseBody);

                Users = usersResponse?.ToList();
            }
            else
                MessageBox.Show("Ошибка: Не удалось получить список пользователей: " + response.RequestMessage + " код ошибки: " + response.StatusCode, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);


            return Users;
        }

        public User? GetUser(int id)
        {
            return Users?.FirstOrDefault(x => x.Id == id);
        }
    }
}
