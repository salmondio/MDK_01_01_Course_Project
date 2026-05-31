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
        public Evaluation(Models.FullModels.Evaluation evaluation)
        {
            InitializeComponent();
            DeleteButton.Visibility = Visibility.Hidden;
            EditButton.Visibility = Visibility.Hidden;

            InitializeVariables();
        }


        private void InitializeVariables(Models.FullModels.Evaluation evaluation)
        {
            Teacher.Content = evaluation.;
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
