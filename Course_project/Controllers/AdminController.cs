using Course_project_wpf.Models.FullModels;
using Couse_project_RestAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course_project_wpf.Controllers
{
    public class AdminController
    {
        public List<Discipline> Disciplines { get; private set; }
        public List<Role> Roles { get; private set; }
        public List<Evaluation> Evaluations { get; private set; } = new List<Evaluation>() {
        new Evaluation()
        {
            Id_student = 1,
            Id_teacher = 2,
            Presentation = 5,
            Attitude = 2,
            Responsiveness = 8,
            Date_time = DateTime.Now
        },
        new Evaluation()
        {
            Id_student = 1,
            Id_teacher = 2,
            Presentation = 8,
            Attitude = 9,
            Responsiveness = 7,
            Date_time = DateTime.Now
        },
        new Evaluation()
        {
            Id_student = 1,
            Id_teacher = 2,
            Presentation = 1,
            Attitude = 2,
            Responsiveness = 5,
            Date_time = DateTime.Now
        },
        new Evaluation()
        {
            Id_student = 1,
            Id_teacher = 2,
            Presentation = 7,
            Attitude = 7,
            Responsiveness = 8,
            Date_time = DateTime.Now
        },
        new Evaluation()
        {
            Id_student = 1,
            Id_teacher = 2,
            Presentation = 1,
            Attitude = 1,
            Responsiveness = 1,
            Date_time = DateTime.Now
        },
        new Evaluation()
        {
            Id_student = 1,
            Id_teacher = 2,
            Presentation = 5,
            Attitude = 5,
            Responsiveness = 5,
            Date_time = DateTime.Now
        },
        new Evaluation()
        {
            Id_student = 1,
            Id_teacher = 2,
            Presentation = 7,
            Attitude = 6,
            Responsiveness = 3,
            Date_time = DateTime.Now
        },
        new Evaluation()
        {
            Id_student = 1,
            Id_teacher = 2,
            Presentation = 7,
            Attitude = 9,
            Responsiveness = 8,
            Date_time = DateTime.Now
        },
        new Evaluation()
        {
            Id_student = 1,
            Id_teacher = 2,
            Presentation = 5,
            Attitude = 5,
            Responsiveness = 2,
            Date_time = DateTime.Now
        },
        new Evaluation()
        {
            Id_student = 1,
            Id_teacher = 2,
            Presentation = 9,
            Attitude = 2,
            Responsiveness = 1,
            Date_time = DateTime.Now
        },
        };
        public List<Report> Reports { get; private set; }
        public List<Review> Reviews { get; private set; }
        public List<User> Users { get; private set; }


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
        public List<Evaluation> GetEvaluations()
        {
            return Evaluations;
        }

        public Evaluation? GetEvaluation(int idStudent, int idTeacher)
        {
            return Evaluations.FirstOrDefault(x => x.Id_student == idStudent && x.Id_teacher == idTeacher);
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
        public List<User> GetUsers()
        {
            return Users;
        }

        public User? GetUser(int id)
        {
            return Users.FirstOrDefault(x => x.Id == id);
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
