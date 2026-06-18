using Course_project_wpf.Controllers;
using Course_project_wpf.Elements;
using Course_project_wpf.Models.FullModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Course_project_wpf.Pages.OwnerAdmin
{
    public partial class Reports : Page
    {
        private List<Models.FullModels.Report>? _reports;
        private SortableHeader? _sortableHeader;
        private bool _isLoading;

        public Reports()
        {
            InitializeComponent();
            Design();
        }

        private void Design()
        {
            _sortableHeader = new SortableHeader();
            _sortableHeader.SortRequested += SortHeader_SortRequested;

            // Добавляем колонки для жалоб
            _sortableHeader.AddColumnFixed("Id", "ID", 60, HorizontalAlignment.Center);
            _sortableHeader.AddColumnStar("Users", "Отправитель → Получатель", 1, HorizontalAlignment.Left);
            _sortableHeader.AddColumnStar("Text", "Текст жалобы", 2, HorizontalAlignment.Left);
            _sortableHeader.AddColumnFixed("DateTime", "Дата/Время", 100, HorizontalAlignment.Center);
            _sortableHeader.AddColumnFixed("Actions", "Действия", 150, HorizontalAlignment.Center);

            Search.Children.Add(_sortableHeader);

            LoadReports();
        }

        private async void LoadReports()
        {
            if (_isLoading) return;
            _isLoading = true;

            Parent.Children.Clear();

            try
            {
                // Загружаем пользователей и статусы для отображения
                await GetController.Instance.GetUsers();
                await GetController.Instance.GetMessageStatuses();

                _reports = await GetController.Instance.GetReports();

                if (_reports != null && _reports.Count > 0)
                {
                    var sortedReports = _reports.OrderBy(r => r.Id).ToList();

                    foreach (var report in sortedReports)
                    {
                        Parent.Children.Add(new Elements.OwnerAdmin.Report(report));
                    }
                }
                else
                {
                    Parent.Children.Add(new Label()
                    {
                        Content = "Жалоб нет",
                        FontSize = 20,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Foreground = (SolidColorBrush)FindResource("Hint")
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке жалоб: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void SortHeader_SortRequested(object sender, SortEventArgs e)
        {
            if (_reports == null || _reports.Count == 0)
                return;

            IEnumerable<Models.FullModels.Report> sortedReports;

            switch (e.ColumnName)
            {
                case "Id":
                    sortedReports = e.IsAscending
                        ? _reports.OrderBy(r => r.Id)
                        : _reports.OrderByDescending(r => r.Id);
                    break;
                case "DateTime":
                    sortedReports = e.IsAscending
                        ? _reports.OrderBy(r => r.Date_time)
                        : _reports.OrderByDescending(r => r.Date_time);
                    break;
                case "Text":
                    sortedReports = e.IsAscending
                        ? _reports.OrderBy(r => r.Text)
                        : _reports.OrderByDescending(r => r.Text);
                    break;
                default:
                    sortedReports = _reports;
                    break;
            }

            _reports = sortedReports.ToList();

            Parent.Children.Clear();
            foreach (var report in _reports)
            {
                Parent.Children.Add(new Elements.OwnerAdmin.Report(report));
            }
        }

        public async void RefreshReports()
        {
            await GetController.Instance.GetReports(true);
            LoadReports();
        }
    }
}