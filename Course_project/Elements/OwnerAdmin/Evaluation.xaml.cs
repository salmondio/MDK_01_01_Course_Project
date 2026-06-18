using Course_project;
using Course_project_wpf.Controllers;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Course_project_wpf.Elements.OwnerAdmin
{
    public partial class Evaluation : UserControl
    {
        private Models.FullModels.Evaluation _evaluation;
        private bool _isAdd;
        private bool _isLoading = false;

        public Evaluation()
        {
            CreateDefaultResources();
            InitializeComponent();
            _isAdd = true;
            GetVisionTextBox();
        }

        public Evaluation(Models.FullModels.Evaluation evaluation)
        {
            _evaluation = evaluation;
            _isAdd = false;

            CreateResourcesFromEvaluation(evaluation);
            InitializeComponent();

            DeleteButton.Visibility = Visibility.Collapsed;
            EditButton.Visibility = Visibility.Collapsed;

            InitializeVariables(evaluation);
        }

        private void CreateDefaultResources()
        {
            var defaultColor = Color.FromRgb(111, 158, 123);
            var defaultDarkColor = Color.FromRgb(82, 139, 104);

            Resources["LocalColorTheme"] = new SolidColorBrush(defaultColor);
            Resources["LocalDarkColorTheme"] = new SolidColorBrush(defaultDarkColor);
        }

        private void CreateResourcesFromEvaluation(Models.FullModels.Evaluation evaluation)
        {
            var (themeColor, darkThemeColor) = GetEvaluationColors(evaluation);

            Resources["LocalColorTheme"] = new SolidColorBrush(themeColor);
            Resources["LocalDarkColorTheme"] = new SolidColorBrush(darkThemeColor);
        }

        private void InitializeVariables(Models.FullModels.Evaluation evaluation)
        {
            Models.FullModels.User? student = GetController.Instance.GetUser(evaluation.IdStudent);
            Models.FullModels.User? teacher = GetController.Instance.GetUser(evaluation.IdTeacher);

            if (student != null)
            {
                Student.Content = $"{student.Lastname} {student.Name} {student.Surname}";
                lbIdStudent.Content = student.Id;
            }
            else
            {
                Student.Content = $"Студент #{evaluation.IdStudent}";
                lbIdStudent.Content = evaluation.IdStudent;
            }

            if (teacher != null)
            {
                Teacher.Content = $"{teacher.Lastname} {teacher.Name} {teacher.Surname}";
                lbIdTeacher.Content = teacher.Id;
            }
            else
            {
                Teacher.Content = $"Преподаватель #{evaluation.IdTeacher}";
                lbIdTeacher.Content = evaluation.IdTeacher;
            }

            Presentation.Content = evaluation.Presentation.ToString();
            Responsiveness.Content = evaluation.Responsiveness.ToString();
            Attitude.Content = evaluation.Attitude.ToString();
            lbDate.Content = evaluation.DateTime.Date.ToString("dd.MM.yyyy");
            lbTime.Content = evaluation.DateTime.ToString("HH:mm");

            if (App.CurrentUser?.Role == null || App.CurrentUser.Role != "Owner")
            {
                MainGrid.MouseEnter -= MainGrid_MouseEnter;
                MainGrid.MouseLeave -= MainGrid_MouseLeave;
                gdActions.Visibility = Visibility.Collapsed;
            }
        }

        // Обновляет цвета (создает новые кисти)
        private void RefreshColors()
        {
            var (themeColor, darkThemeColor) = GetEvaluationColors(_evaluation);

            // СОЗДАЕМ НОВЫЕ кисти вместо изменения существующих
            var newColorBrush = new SolidColorBrush(themeColor);
            var newDarkColorBrush = new SolidColorBrush(darkThemeColor);

            // Обновляем ресурсы
            Resources["LocalColorTheme"] = newColorBrush;
            Resources["LocalDarkColorTheme"] = newDarkColorBrush;

            // Применяем ко всем элементам
            ApplyColorsToElements(newColorBrush, newDarkColorBrush);
        }

        // Применяет кисти ко всем элементам
        private void ApplyColorsToElements(SolidColorBrush colorBrush, SolidColorBrush darkColorBrush)
        {
            bdId.Background = colorBrush;
            gdPeople.Background = colorBrush;
            gdEvaluation.Background = colorBrush;
            bdDateTime.Background = colorBrush;

            Student.Background = colorBrush;
            Teacher.Background = colorBrush;
        }

        private (Color themeColor, Color darkThemeColor) GetEvaluationColors(Models.FullModels.Evaluation evaluation)
        {
            try
            {
                double average = evaluation.Average;

                if (average < 3)
                {
                    var themeBrush = (SolidColorBrush)FindResource("BadColor");
                    var darkBrush = (SolidColorBrush)FindResource("BadColorDark");
                    return (themeBrush.Color, darkBrush.Color);
                }
                else if (average < 6)
                {
                    var themeBrush = (SolidColorBrush)FindResource("NormalColor");
                    var darkBrush = (SolidColorBrush)FindResource("NormalColorDark");
                    return (themeBrush.Color, darkBrush.Color);
                }
                else
                {
                    var themeBrush = (SolidColorBrush)FindResource("GoodColor");
                    var darkBrush = (SolidColorBrush)FindResource("GoodColorDark");
                    return (themeBrush.Color, darkBrush.Color);
                }
            }
            catch
            {
                return (Color.FromRgb(111, 158, 123), Color.FromRgb(82, 139, 104));
            }
        }

        // Получает текущую основную кисть из ресурсов
        private SolidColorBrush GetCurrentColorBrush()
        {
            return (SolidColorBrush)Resources["LocalColorTheme"];
        }

        // Получает текущую темную кисть из ресурсов
        private SolidColorBrush GetCurrentDarkColorBrush()
        {
            return (SolidColorBrush)Resources["LocalDarkColorTheme"];
        }

        #region Navigation Events

        private void GoToStudent(object sender, RoutedEventArgs e)
        {
            var student = GetController.Instance.GetUser(_evaluation.IdStudent);
        }

        private void GoToTeacher(object sender, RoutedEventArgs e)
        {
            var teacher = GetController.Instance.GetUser(_evaluation.IdTeacher);
        }

        #endregion

        #region Mouse Events

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
            Student.Background = GetCurrentDarkColorBrush();
        }

        private void StudentButton_MouseLeave(object sender, MouseEventArgs e)
        {
            Student.Background = GetCurrentColorBrush();
        }

        private void TeacherButton_MouseEnter(object sender, MouseEventArgs e)
        {
            Teacher.Background = GetCurrentDarkColorBrush();
        }

        private void TeacherButton_MouseLeave(object sender, MouseEventArgs e)
        {
            Teacher.Background = GetCurrentColorBrush();
        }

        #endregion

        #region Actions

        private void Update(object sender, RoutedEventArgs e)
        {
            GetVisionTextBox();
            EditButton.Visibility = Visibility.Collapsed;
            DeleteButton.Visibility = Visibility.Collapsed;
            SaveButton.Visibility = Visibility.Visible;
            CancelButton.Visibility = Visibility.Visible;
        }

        private void GetVisionTextBox()
        {
            tbIdStudent.Text = lbIdStudent.Content.ToString();
            tbIdTeacher.Text = lbIdTeacher.Content.ToString();
            tbPresentation.Text = Presentation.Content.ToString();
            tbAttitude.Text = Attitude.Content.ToString();
            tbResponsiveness.Text = Responsiveness.Content.ToString();
            tbDate.Text = lbDate.Content.ToString();
            tbTime.Text = lbTime.Content.ToString();

            lbIdStudent.Visibility = Visibility.Collapsed;
            lbIdTeacher.Visibility = Visibility.Collapsed;
            Presentation.Visibility = Visibility.Collapsed;
            Attitude.Visibility = Visibility.Collapsed;
            Responsiveness.Visibility = Visibility.Collapsed;
            lbDate.Visibility = Visibility.Collapsed;
            lbTime.Visibility = Visibility.Collapsed;

            tbIdStudent.Visibility = Visibility.Visible;
            tbIdTeacher.Visibility = Visibility.Visible;
            tbPresentation.Visibility = Visibility.Visible;
            tbAttitude.Visibility = Visibility.Visible;
            tbResponsiveness.Visibility = Visibility.Visible;
            tbDate.Visibility = Visibility.Visible;
            tbTime.Visibility = Visibility.Visible;

            EditButton.Visibility = Visibility.Collapsed;
            DeleteButton.Visibility = Visibility.Collapsed;
            SaveButton.Visibility = Visibility.Visible;
            CancelButton.Visibility = Visibility.Visible;
        }

        private void GetVisionLabel()
        {
            tbIdStudent.Visibility = Visibility.Collapsed;
            tbIdTeacher.Visibility = Visibility.Collapsed;
            tbPresentation.Visibility = Visibility.Collapsed;
            tbAttitude.Visibility = Visibility.Collapsed;
            tbResponsiveness.Visibility = Visibility.Collapsed;
            tbDate.Visibility = Visibility.Collapsed;
            tbTime.Visibility = Visibility.Collapsed;

            lbIdStudent.Visibility = Visibility.Visible;
            lbIdTeacher.Visibility = Visibility.Visible;
            Presentation.Visibility = Visibility.Visible;
            Attitude.Visibility = Visibility.Visible;
            Responsiveness.Visibility = Visibility.Visible;
            lbDate.Visibility = Visibility.Visible;
            lbTime.Visibility = Visibility.Visible;

            EditButton.Visibility = Visibility.Visible;
            DeleteButton.Visibility = Visibility.Visible;
            SaveButton.Visibility = Visibility.Collapsed;
            CancelButton.Visibility = Visibility.Collapsed;
        }

        private void tbIdStudent_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(tbIdStudent.Text, out int id))
            {
                Models.FullModels.User? student = GetController.Instance.GetUser(id);
                if (student != null && student.Id_role == 4)
                {
                    Student.Content = student.FullName;
                    return;
                }
            }
            Student.Content = "Не удалось найти студента";
        }

        private void tbIdTeacher_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(tbIdTeacher.Text, out int id))
            {
                Models.FullModels.User? teacher = GetController.Instance.GetUser(id);
                if (teacher != null && teacher.Id_role == 5)
                {
                    Teacher.Content = teacher.FullName;
                    return;
                }
            }
            Teacher.Content = "Не удалось найти преподавателя";
        }

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

        private async void Save(object sender, RoutedEventArgs e)
        {
            if (_isLoading) return;

            if (!int.TryParse(tbIdTeacher.Text, out int idTeacher) ||
                !int.TryParse(tbIdStudent.Text, out int idStudent))
            {
                MessageBox.Show("Введены некорректные значения Id", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(tbAttitude.Text, out int attitude) ||
                !int.TryParse(tbResponsiveness.Text, out int responsiveness) ||
                !int.TryParse(tbPresentation.Text, out int presentation) ||
                attitude < 1 || attitude > 9 ||
                responsiveness < 1 || responsiveness > 9 ||
                presentation < 1 || presentation > 9)
            {
                MessageBox.Show("Введены некорректные значения оценок.\nКаждая оценка должна быть в промежутке от 1 до 9",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var teacher = GetController.Instance.GetUser(idTeacher);
            var student = GetController.Instance.GetUser(idStudent);

            if (teacher?.Id_role != 5)
            {
                MessageBox.Show("Ошибка: Не существует преподавателя с указанным id", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (student?.Id_role != 4)
            {
                MessageBox.Show("Ошибка: Не существует студента с указанным id", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Проверка на дубликат только при добавлении
            if (_isAdd)
            {
                var existing = GetController.Instance.Evaluations?
                    .FirstOrDefault(e => e.IdTeacher == idTeacher && e.IdStudent == idStudent);
                if (existing != null)
                {
                    MessageBox.Show("Оценка этого студента этому преподавателю уже выставлена",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            _isLoading = true;
            SaveButton.IsEnabled = false;

            try
            {
                var updatedEvaluation = new Models.FullModels.Evaluation()
                {
                    IdStudent = idStudent,
                    IdTeacher = idTeacher,
                    Presentation = (byte)presentation,
                    Attitude = (byte)attitude,
                    Responsiveness = (byte)responsiveness,
                    DateTime = DateTime.Now
                };

                Models.FullModels.Evaluation result;

                if (_isAdd)
                {
                    result = await PostController.Instance.AddEvaluation(updatedEvaluation);
                    if (result != null)
                    {
                        _evaluation = result;
                        _isAdd = false;
                        InitializeVariables(result);
                        RefreshColors();
                        GetVisionLabel();
                        MessageBox.Show("Оценка успешно добавлена!", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    result = await PutController.Instance.UpdateEvaluation(updatedEvaluation);
                    if (result != null)
                    {
                        _evaluation = result;
                        InitializeVariables(result);
                        RefreshColors();
                        GetVisionLabel();
                        MessageBox.Show("Оценка успешно изменена!", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _isLoading = false;
                SaveButton.IsEnabled = true;
            }
        }

        private async void Delete(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show($"Вы уверены, что хотите удалить оценку?\nСтудент: {Student.Content}\nПреподаватель: {Teacher.Content}",
                "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var deletedEvaluation = await DeleteController.Instance.DeleteEvaluation(_evaluation.IdStudent, _evaluation.IdTeacher);

                    if (deletedEvaluation != null)
                    {
                        if (Parent is Panel parentPanel)
                        {
                            parentPanel.Children.Remove(this);
                        }
                        MessageBox.Show("Оценка успешно удалена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Не удалось удалить оценку", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        #endregion
    }
}