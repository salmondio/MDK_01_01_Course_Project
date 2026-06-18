using Course_project_wpf.Controllers;
using Course_project_wpf.Elements;
using Course_project_wpf.Elements.OwnerAdmin;
using Course_project_wpf.Models.FullModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Course_project_wpf.Pages.OwnerAdmin
{
    public partial class Evaluations : Page
    {
        private List<Models.FullModels.Evaluation>? _evaluations;
        private SortableHeader? _sortableHeader;

        public Evaluations()
        {
            InitializeComponent();
            Design();
        }

        private void Design()
        {
            _sortableHeader = new SortableHeader();
            _sortableHeader.SortRequested += SortHeader_SortRequested;

            // Добавляем колонки для оценок
            _sortableHeader.AddColumnFixed("IdStudent", "ID", 60, HorizontalAlignment.Center);
            _sortableHeader.AddColumnStar("FullName", "Студент → Преподаватель", 1, HorizontalAlignment.Left);
            _sortableHeader.AddColumnFixed("Evaluation", "Оценки", 200, HorizontalAlignment.Center);
            _sortableHeader.AddColumnFixed("DateTime", "Дата/Время", 100, HorizontalAlignment.Center);
            _sortableHeader.AddColumnFixed("Actions", "Действия", 150, HorizontalAlignment.Center);

            Search.Children.Add(_sortableHeader);

            GetEvaluations();
        }

        private async void GetEvaluations()
        {
            Parent.Children.Clear();
            await GetController.Instance.GetUsers();
            _evaluations = await GetController.Instance.GetEvaluations();

            if (_evaluations != null && _evaluations.Count != 0)
            {
                foreach (var evaluation in _evaluations)
                    Parent.Children.Add(new Elements.OwnerAdmin.Evaluation(evaluation));
            }
            else
            {
                Parent.Children.Add(new Label()
                {
                    Content = "Оценок нет",
                    FontSize = 20,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Foreground = (SolidColorBrush)FindResource("Hint")
                });
            }
        }

        private void SortHeader_SortRequested(object sender, SortEventArgs e)
        {
            if (_evaluations == null || _evaluations.Count == 0)
                return;

            IEnumerable<Models.FullModels.Evaluation> sortedEvaluations;

            switch (e.ColumnName)
            {
                case "IdStudent":
                    sortedEvaluations = e.IsAscending
                        ? _evaluations.OrderBy(ev => ev.IdStudent)
                        : _evaluations.OrderByDescending(ev => ev.IdStudent);
                    break;
                case "DateTime":
                    sortedEvaluations = e.IsAscending
                        ? _evaluations.OrderBy(ev => ev.DateTime)
                        : _evaluations.OrderByDescending(ev => ev.DateTime);
                    break;
                case "Evaluation":
                    sortedEvaluations = e.IsAscending
                        ? _evaluations.OrderBy(ev => ev.Average)
                        : _evaluations.OrderByDescending(ev => ev.Average);
                    break;
                default:
                    sortedEvaluations = _evaluations;
                    break;
            }

            _evaluations = sortedEvaluations.ToList(); // <-- Обновляем список

            Parent.Children.Clear();
            foreach (var evaluation in _evaluations)
            {
                Parent.Children.Add(new Elements.OwnerAdmin.Evaluation(evaluation));
            }
        }
    }
}