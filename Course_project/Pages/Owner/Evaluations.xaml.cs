using Course_project_wpf.Controllers;
using Course_project_wpf.Elements;
using Course_project_wpf.Elements.OwnerAdmin;
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

namespace Course_project_wpf.Pages.Owner
{
    /// <summary>
    /// Логика взаимодействия для Evaluations.xaml
    /// </summary>
    public partial class Evaluations : Page
    {
        private List<Models.FullModels.Evaluation> _evaluations;
        private AdminController _adminController;
        public Evaluations(AdminController adminController)
        {
            InitializeComponent();
            _adminController = adminController;

            Design();
        }

        private void Design()
        {
            // Линия сортировки
            Elements.SortableHeader sortableHeader = new Elements.SortableHeader();
            sortableHeader.SortRequested += SortHeader_SortRequested;
            Search.Children.Add(sortableHeader);

            // Заполнение оценками
            GetEvaluations();
        }

        private void GetEvaluations()
        {
            _evaluations = MainWindowAdmin.AdminController.GetEvaluations();
            foreach (var evaluation in _evaluations)
            {
                Parent.Children.Add(new Elements.OwnerAdmin.Evaluation(evaluation, _adminController));
            }
        }

        private void SortHeader_SortRequested(object sender, SortEventArgs e)
        {

        }
    }
}
