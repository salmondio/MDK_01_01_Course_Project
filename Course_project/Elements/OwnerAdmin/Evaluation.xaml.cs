using Course_project_wpf.Controllers;
using Course_project_wpf.Models.FullModels;
using Course_project_wpf.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Course_project_wpf.Elements.OwnerAdmin
{
    /// <summary>
    /// Логика взаимодействия для Evaluation.xaml
    /// </summary>
    public partial class Evaluation : UserControl
    {
        private AdminController _adminController;
        private Models.FullModels.Evaluation _evaluation;
        public Evaluation(Models.FullModels.Evaluation evaluation, AdminController adminController)
        {
            InitializeComponent();
            _evaluation = evaluation;
            _adminController = adminController;

            DeleteButton.Visibility = Visibility.Hidden;
            EditButton.Visibility = Visibility.Hidden;

            InitializeVariables(evaluation);
        }


        private void InitializeVariables(Models.FullModels.Evaluation evaluation)
        {
            User? student = _adminController.Users.FirstOrDefault(x => x.Id == evaluation.Id_student);
            User? teacher = _adminController.Users.FirstOrDefault(x => x.Id == evaluation.Id_teacher);
            Student.Content = $"{student?.Lastname} {student?.Name} {student?.Surname}";
            Teacher.Content = $"{teacher?.Lastname} {teacher?.Name} {teacher?.Surname}";
            Presentation.Content = evaluation.Presentation.ToString();
            Responsiveness.Content = evaluation.Responsiveness.ToString();
            Attitude.Content = evaluation.Attitude.ToString();
            lbDate.Content = evaluation.Date_time.Date.ToString();
            lbTime.Content = evaluation.Date_time.TimeOfDay.ToString();
        }

        private void GoToStudent(object sender, RoutedEventArgs e)
        {

        }

        private void GoToTeacher(object sender, RoutedEventArgs e)
        {

        }

        private void Update(object sender, RoutedEventArgs e)
        {

        }

        private void Delete(object sender, RoutedEventArgs e)
        {

        }
    }
}
