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


        // Действия над Дисциплинами
        


        // Действия над Ролями
        


        // Действия над Оцнеками
        


        // Действия над Жалобами
        



        // Действия над связкой Преподаватель-Дисциплина
        public void AddTeacherDiscipline (TeacherDiscipline newTeacherDiscipline)
        {
            TeacherDiscipline teacherDiscipline = new TeacherDiscipline();
        }


        // Действия над пользователями
        

        public User? AddUser(User newUser)
        {
            User user = new User();
            GetUsers();
            return GetUser(user.Id);
        }

        
    }
}
