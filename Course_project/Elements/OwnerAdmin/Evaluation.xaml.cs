using Course_project;
using Course_project_wpf.Controllers;
using Course_project_wpf.Models.FullModels;
using Course_project_wpf.Pages.Owner;
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
            InitializeResources(evaluation);
            InitializeComponent();
            _evaluation = evaluation;
            _adminController = adminController;

            DeleteButton.Visibility = Visibility.Collapsed;
            EditButton.Visibility = Visibility.Collapsed;

            //var themeBrush = Resources["LocalColorTheme"] as SolidColorBrush;
            //if (themeBrush != null)
            //{
            //    Student.Background = themeBrush;
            //    Teacher.Background = themeBrush;
            //}
            InitializeVariables(evaluation);
        }


        private void InitializeVariables(Models.FullModels.Evaluation evaluation)
        {
            // Заполняем текстовые поля
            User? student = _adminController.Users?.FirstOrDefault(x => x.Id == evaluation.Id_student);
            User? teacher = _adminController.Users?.FirstOrDefault(x => x.Id == evaluation.Id_teacher);
            if( student != null)
            {
                Student.Content = $"{student.Lastname} {student.Name} {student.Surname}";
                lbIdStudent.Content = student.Id;
            }
            if( teacher != null)
            {
                Teacher.Content = $"{teacher.Lastname} {teacher.Name} {teacher.Surname}";
                lbIdTeacher.Content = teacher.Id;
            }
            Presentation.Content = evaluation.Presentation.ToString();
            Responsiveness.Content = evaluation.Responsiveness.ToString();
            Attitude.Content = evaluation.Attitude.ToString();
            lbDate.Content = evaluation.Date_time.Date.ToString("dd.MM.yyyy");
            lbTime.Content = evaluation.Date_time.ToString("HH:mm");


            // Если пользователь не Owner, 
            //if(App.CurrentUser.Role != "Owner")
            //{
            //MainGrid.MouseEnter -= MainGrid_MouseEnter;
            //MainGrid.MouseLeave -= MainGrid_MouseLeave;
            //gdActions.Visibility = Visibility.Collapsed;
            //}
        }

        private void InitializeResources(Models.FullModels.Evaluation evaluation)
        {
            try
            {
                SolidColorBrush themeBrush;
                SolidColorBrush darkThemeBrush;

                if (evaluation.Average < 3)
                {
                    themeBrush = (SolidColorBrush)FindResource("BadColor");
                    darkThemeBrush = (SolidColorBrush)FindResource("BadColorDark");
                }
                else if (evaluation.Average < 6)
                {
                    themeBrush = (SolidColorBrush)FindResource("NormalColor");
                    darkThemeBrush = (SolidColorBrush)FindResource("NormalColorDark");
                }
                else
                {
                    themeBrush = (SolidColorBrush)FindResource("GoodColor");
                    darkThemeBrush = (SolidColorBrush)FindResource("GoodColorDark");
                }

                // Создаем новые ресурсы (не переиспользуем существующие)
                Resources["LocalColorTheme"] = new SolidColorBrush(themeBrush.Color);
                Resources["LocalDarkColorTheme"] = new SolidColorBrush(darkThemeBrush.Color);
            }
            catch
            {
                MessageBox.Show("Ошибка: не удалось найти системные цвета", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                // Устанавливаем цвета по умолчанию
                Resources["LocalColorTheme"] = new SolidColorBrush(Color.FromRgb(111, 158, 123));
                Resources["LocalDarkColorTheme"] = new SolidColorBrush(Color.FromRgb(82, 139, 104));
            }
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

        private void MainGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            DeleteButton.Visibility = Visibility.Visible;
            EditButton.Visibility = Visibility.Visible;
        }

        private void MainGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            DeleteButton.Visibility = Visibility.Collapsed;
            EditButton.Visibility = Visibility.Collapsed;
        }

        private void StudentButton_MouseEnter(object sender, MouseEventArgs e)
        {
            Student.Background = (SolidColorBrush)Resources["LocalDarkColorTheme"];
        }

        private void StudentButton_MouseLeave(object sender, MouseEventArgs e)
        {
            Student.Background = (SolidColorBrush)Resources["LocalColorTheme"];
        }

        private void TeacherButton_MouseEnter(object sender, MouseEventArgs e)
        {
            Teacher.Background = (SolidColorBrush)Resources["LocalDarkColorTheme"];
        }

        private void TeacherButton_MouseLeave(object sender, MouseEventArgs e)
        {
            Teacher.Background = (SolidColorBrush)Resources["LocalColorTheme"];
        }
    }
}
