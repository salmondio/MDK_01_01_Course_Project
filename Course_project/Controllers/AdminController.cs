using Course_project_wpf.Helpers;
using Course_project_wpf.Models.FullModels;
using Couse_project_RestAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace Course_project_wpf.Controllers
{
    public class AdminController
    {
        public List<Discipline>? Disciplines { get; private set; }
        public List<Role>? Roles { get; private set; }
        public List<Evaluation>? Evaluations { get; private set; }
        public List<Report>? Reports { get; private set; }
        public List<Review>? Reviews { get; private set; }
        public List<User>? Users { get; private set; }


        // Действия над Дисциплинами
        public List<Discipline> GetDisciplines()
        {

            return Disciplines;
        }

        public Discipline? GetDiscipline(int id)
        {
            return Disciplines.FirstOrDefault(x => x.Id == id);
        }


        // Действия над Ролями
        public List<Role> GetRoles()
        {
            return Roles;
        }

        public Role? GetRole(int id)
        {
            return Roles.FirstOrDefault(x => x.Id == id);
        }


        // Действия над Оцнеками
        public async Task<List<Evaluation>?> GetEvaluations()
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

            return Evaluations;
        }

        public Evaluation? GetEvaluation(int idStudent, int idTeacher)
        {
            return Evaluations?.FirstOrDefault(x => x.IdStudent == idStudent && x.IdTeacher == idTeacher);
        }


        // Действия над Жалобами
        public List<Report> GetReports()
        {
            return Reports;
        }

        public Report? GetReport(int id)
        {
            return Reports.FirstOrDefault(x => x.Id == id);
        }

        public Report? ChangeStatusReport(int id)
        {
            GetReports();
            return GetReport(id);
        }


        // Действия над отзывами
        public List<Review> GetReviews()
        {
            return Reviews;
        }

        public Review? GetReview(int id)
        {
            return Reviews.FirstOrDefault(x => x.Id == id);
        }

        public Review? ChangeStatusReview(int id)
        {
            GetReviews();
            return GetReview(id);
        }


        // Действия над связкой Преподаватель-Дисциплина
        public void AddTeacherDiscipline (TeacherDiscipline newTeacherDiscipline)
        {
            TeacherDiscipline teacherDiscipline = new TeacherDiscipline();
        }


        // Действия над пользователями
        public async Task<List<User>?> GetUsers()
        {
            var response = await ApiClient.GetAsync("api/User/List");

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

        public User? AddUser(User newUser)
        {
            User user = new User();
            GetUsers();
            return GetUser(user.Id);
        }

        public User? ChangeActiveUser(int id)
        {
            return GetUser(id);
        }

        public User? UpdateUser(User user)
        {
            User updatedUser = new User();
            GetUsers();
            return GetUser(user.Id);
        }
    }
}
