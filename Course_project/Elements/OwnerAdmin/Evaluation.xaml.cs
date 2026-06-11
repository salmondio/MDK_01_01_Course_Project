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
        private Models.FullModels.Evaluation _evaluation;
        private bool _isAdd;
        public Evaluation()
        {
            InitializeComponent();
            _isAdd = true;
            Update(this, new RoutedEventArgs());
        }
        public Evaluation(Models.FullModels.Evaluation evaluation)
        {
            InitializeResources(evaluation);
            InitializeComponent();
            _evaluation = evaluation;

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
            User? student = GetController.Instance.GetUser(evaluation.IdStudent);
            User? teacher = GetController.Instance.GetUser(evaluation.IdTeacher);
            if ( student != null)
            {
                Student.Content = $"{student.Lastname} {student.Name} {student.Surname}";
                lbIdStudent.Content = student.Id;
            }
            if( teacher != null)
            {
                Teacher.Content = $"{teacher.Lastname} {teacher.Name} {teacher.Surname}";
                lbIdTeacher.Content = teacher.Id;
            }
            lbIdStudent.Content = evaluation.IdStudent;
            lbIdTeacher.Content= evaluation.IdTeacher;
            Presentation.Content = evaluation.Presentation.ToString();
            Responsiveness.Content = evaluation.Responsiveness.ToString();
            Attitude.Content = evaluation.Attitude.ToString();
            lbDate.Content = evaluation.DateTime.Date.ToString("dd.MM.yyyy");
            lbTime.Content = evaluation.DateTime.ToString("HH:mm");


            // Если пользователь не Owner, 
            if (App.CurrentUser?.Role == null || App.CurrentUser.Role != "Owner")
            {
                MainGrid.MouseEnter -= MainGrid_MouseEnter;
                MainGrid.MouseLeave -= MainGrid_MouseLeave;
                gdActions.Visibility = Visibility.Collapsed;
            }
        }
        // Раскрашиваю элемент
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

        
        // Действия

        // Изменить запись
        private void Update(object sender, RoutedEventArgs e)
        {
            // Показываю текстбоксы вместо лейблов
            GetVisionTextBox();
            // Меняю кнопки
            EditButton.Visibility = Visibility.Collapsed;
            DeleteButton.Visibility = Visibility.Collapsed;
            SaveButton.Visibility = Visibility.Visible;
            CancelButton.Visibility = Visibility.Visible;
        }
        // Скрывает лейблы, показывает текстбоксы
        private void GetVisionTextBox()
        {
            // Устанавливаю соответствующий текст в текстбоксы
            tbIdStudent.Text = lbIdStudent.Content.ToString();
            tbIdTeacher.Text = lbIdTeacher.Content.ToString();
            tbPresentation.Text = Presentation.Content.ToString();
            tbAttitude.Text = Attitude.Content.ToString();
            tbResponsiveness.Text = Responsiveness.Content.ToString();
            tbDate.Text = lbDate.Content.ToString();
            tbTime.Text = lbTime.Content.ToString();
            // Скрываю лейблы
            lbIdStudent.Visibility = Visibility.Collapsed;
            lbIdTeacher.Visibility = Visibility.Collapsed;
            Presentation.Visibility = Visibility.Collapsed;
            Attitude.Visibility = Visibility.Collapsed;
            Responsiveness.Visibility = Visibility.Collapsed;
            lbDate.Visibility = Visibility.Collapsed;
            lbTime.Visibility = Visibility.Collapsed;
            // Раскрываю текстбоксы
            tbIdStudent.Visibility = Visibility.Visible;
            tbIdTeacher.Visibility = Visibility.Visible;
            tbPresentation.Visibility = Visibility.Visible;
            tbAttitude.Visibility = Visibility.Visible;
            tbResponsiveness.Visibility = Visibility.Visible;
            tbDate.Visibility = Visibility.Visible;
            tbTime.Visibility = Visibility.Visible;
            // Меняю кнопки
            EditButton.Visibility = Visibility.Collapsed;
            DeleteButton.Visibility = Visibility.Collapsed;
            SaveButton.Visibility = Visibility.Visible;
            CancelButton.Visibility = Visibility.Visible;
        }
        // Скрываю текстбоксы, показываю лейблы
        private void GetVisionLabel()
        {
            // Скрываю текстбоксы
            tbIdStudent.Visibility = Visibility.Collapsed;
            tbIdTeacher.Visibility = Visibility.Collapsed;
            tbPresentation.Visibility = Visibility.Collapsed;
            tbAttitude.Visibility = Visibility.Collapsed;
            tbResponsiveness.Visibility = Visibility.Collapsed;
            tbDate.Visibility = Visibility.Collapsed;
            tbTime.Visibility = Visibility.Collapsed;
            // Раскрываю лейблы
            lbIdStudent.Visibility = Visibility.Visible;
            lbIdTeacher.Visibility = Visibility.Visible;
            Presentation.Visibility = Visibility.Visible;
            Attitude.Visibility = Visibility.Visible;
            Responsiveness.Visibility = Visibility.Visible;
            lbDate.Visibility = Visibility.Visible;
            lbTime.Visibility = Visibility.Visible;
            // Меняю кнопки
            EditButton.Visibility = Visibility.Visible;
            DeleteButton.Visibility = Visibility.Visible;
            SaveButton.Visibility = Visibility.Collapsed;
            CancelButton.Visibility = Visibility.Collapsed;
        }
        // При изменении Id студента пытаюсь сразу вывести его ФИО
        private void tbIdStudent_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(int.TryParse(tbIdStudent.Text, out int id))
           {
                User? student = GetController.Instance.GetUser(id);
                if(student != null && student.Id_role == 4)
                {
                    Student.Content = student.FullName;
                    return;
                }
            }
            Student.Content = "Не удалось найти студента";
        }
        // При изменении Id препода пытаюсь сразу вывести его ФИО
        private void tbIdTeacher_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(tbIdTeacher.Text, out int id))
            {
                User? teacher = GetController.Instance.GetUser(id);
                if (teacher != null && teacher.Id_role == 5)
                {
                    Teacher.Content = teacher.FullName;
                    return;
                }
            }
            Teacher.Content = "Не удалось найти преподавателя";
        }
        // Отменить изменения
        private void Cancel(object sender, RoutedEventArgs e)
        {
            if (_isAdd)
            {
                if (this.Parent is Panel parentPanel)
                {
                    parentPanel.Children.Remove(this);
                }
            }
            GetVisionLabel();
        }
        // Сохранить изменения
        private async void Save(object sender, RoutedEventArgs e)
        {
            // Если введены корректные id
            if (int.TryParse(tbIdTeacher.Text, out int idTeacerh) &&
                int.TryParse(tbIdStudent.Text, out int idStudent))
            {
                if (int.TryParse(tbAttitude.Text, out int attitude) &&
                    int.TryParse(tbResponsiveness.Text, out int responsiveness) &&
                    int.TryParse(tbPresentation.Text, out int presentation) &&
                    0 < attitude && attitude < 10 && 0 < responsiveness && responsiveness < 10 && 0 < presentation && presentation < 10)
                    {
                    // Если студент и преподаватель с такими id существуют и нет оценки, выставленной этим этому
                    if (GetController.Instance.GetUser(idTeacerh)?.Id_role == 5 &&
                        GetController.Instance.GetUser(idStudent)?.Id_role == 4)
                    {
                        if (GetController.Instance.Evaluations?
                        .FirstOrDefault(e => e.IdTeacher == idTeacerh && e.IdStudent == idStudent) == null ||
                        idStudent == _evaluation.IdStudent && idTeacerh == _evaluation.IdTeacher)
                        {
                            // Создаю экземпляр оценки
                            Models.FullModels.Evaluation? updatedEvaluation = new Models.FullModels.Evaluation()
                            {
                                IdStudent = int.Parse(tbIdStudent.Text),
                                IdTeacher = int.Parse(tbIdTeacher.Text),
                                Presentation = byte.Parse(tbPresentation.Text),
                                Attitude = byte.Parse(tbAttitude.Text),
                                Responsiveness = byte.Parse(tbResponsiveness.Text),
                                DateTime = DateTime.Now
                            };
                            updatedEvaluation = await PutController.Instance.UpdateEvaluation(updatedEvaluation);

                            if (updatedEvaluation != null)
                            {

                                _evaluation = updatedEvaluation;
                                InitializeVariables(updatedEvaluation);
                                Cancel(new object(), new RoutedEventArgs());
                                MessageBox.Show("Оценка успешно изменена!");
                            }
                        }
                        else
                            MessageBox.Show("Оценка этого студента этому преподавателю уже выставлена");
                    }
                    else
                        MessageBox.Show("Ошибка: Не существует студента/преподавателя с указанным id");
                }
                else
                    MessageBox.Show("Введены некорректные значения оценок.\nКаждая оценка должна быть в промежутке от 1 до 9");
            }
            else
                MessageBox.Show("Введены некорректные значения Id");
        }

        private void Delete(object sender, RoutedEventArgs e)
        {

        }
    }
}
