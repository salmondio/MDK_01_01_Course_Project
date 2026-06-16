using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Course_project_wpf.Elements
{
    public partial class SortableHeader : UserControl
    {
        public event EventHandler<SortEventArgs>? SortRequested;

        private string? _currentSortColumn;
        private bool _isAscending = true;
        private readonly Dictionary<string, ColumnInfo> _columns = new Dictionary<string, ColumnInfo>();

        public SortableHeader()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Добавляет колонку с фиксированной шириной в пикселях
        /// </summary>
        public void AddColumnFixed(string columnName, string displayName, double pixelWidth, HorizontalAlignment alignment = HorizontalAlignment.Left)
        {
            HeaderGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(pixelWidth) });

            var panel = CreateHeaderPanel(columnName, displayName, alignment);

            HeaderGrid.Children.Add(panel);
            var columnIndex = HeaderGrid.Children.Count - 1;
            Grid.SetColumn(panel, columnIndex);
        }

        /// <summary>
        /// Добавляет колонку с пропорциональной шириной (звездочка)
        /// </summary>
        public void AddColumnStar(string columnName, string displayName, double starWidth, HorizontalAlignment alignment = HorizontalAlignment.Left)
        {
            HeaderGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(starWidth, GridUnitType.Star) });

            var panel = CreateHeaderPanel(columnName, displayName, alignment);

            HeaderGrid.Children.Add(panel);
            var columnIndex = HeaderGrid.Children.Count - 1;
            Grid.SetColumn(panel, columnIndex);
        }

        /// <summary>
        /// Добавляет колонку с автоматической шириной
        /// </summary>
        public void AddColumnAuto(string columnName, string displayName, HorizontalAlignment alignment = HorizontalAlignment.Left)
        {
            HeaderGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var panel = CreateHeaderPanel(columnName, displayName, alignment);

            HeaderGrid.Children.Add(panel);
            var columnIndex = HeaderGrid.Children.Count - 1;
            Grid.SetColumn(panel, columnIndex);
        }

        /// <summary>
        /// Создает панель заголовка
        /// </summary>
        private StackPanel CreateHeaderPanel(string columnName, string displayName, HorizontalAlignment alignment)
        {
            var panel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Cursor = Cursors.Hand,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = alignment,
                Margin = new Thickness(5, 0, 5, 0)
            };

            var textBlock = new TextBlock
            {
                Text = displayName,
                FontWeight = FontWeights.SemiBold,
                FontSize = 14,
                Foreground = GetActiveBrush(),
                VerticalAlignment = VerticalAlignment.Center
            };

            var arrow = new TextBlock
            {
                Text = "⇅",
                FontSize = 12,
                Visibility = Visibility.Collapsed,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = GetActiveBrush(),
                Margin = new Thickness(3, 0, 0, 0)
            };

            panel.Children.Add(textBlock);
            panel.Children.Add(arrow);

            // Сохраняем информацию о колонке
            _columns[columnName] = new ColumnInfo
            {
                Panel = panel,
                TextBlock = textBlock,
                Arrow = arrow,
                DisplayName = displayName
            };

            // Обработчики событий
            panel.MouseLeftButtonUp += (s, e) =>
            {
                OnColumnClick(columnName);
            };

            panel.MouseEnter += (s, e) =>
            {
                if (_currentSortColumn != columnName)
                {
                    textBlock.Foreground = GetHoverBrush();
                }
            };

            panel.MouseLeave += (s, e) =>
            {
                if (_currentSortColumn != columnName)
                {
                    textBlock.Foreground = GetActiveBrush();
                }
            };

            return panel;
        }

        private void OnColumnClick(string columnName)
        {
            if (_currentSortColumn == columnName)
            {
                _isAscending = !_isAscending;
            }
            else
            {
                _currentSortColumn = columnName;
                _isAscending = true;
            }

            UpdateSortArrows();
            SortRequested?.Invoke(this, new SortEventArgs(columnName, _isAscending));
        }

        private void UpdateSortArrows()
        {
            var activeBrush = GetActiveBrush();
            var hintBrush = GetHintBrush();

            foreach (var kvp in _columns)
            {
                var columnInfo = kvp.Value;
                var isCurrent = kvp.Key == _currentSortColumn;

                if (isCurrent)
                {
                    columnInfo.Arrow.Text = _isAscending ? "↑" : "↓";
                    columnInfo.Arrow.Visibility = Visibility.Visible;
                    columnInfo.TextBlock.Foreground = activeBrush;
                    columnInfo.Arrow.Foreground = activeBrush;
                }
                else
                {
                    columnInfo.Arrow.Text = "⇅";
                    columnInfo.Arrow.Visibility = Visibility.Collapsed;
                    columnInfo.TextBlock.Foreground = activeBrush;
                    columnInfo.Arrow.Foreground = hintBrush;
                }
            }
        }

        private SolidColorBrush GetHintBrush()
        {
            try
            {
                var brush = FindResource("Hint") as SolidColorBrush;
                if (brush != null)
                    return brush;
            }
            catch { }
            return new SolidColorBrush(Color.FromRgb(128, 128, 128));
        }

        private SolidColorBrush GetActiveBrush()
        {
            try
            {
                var brush = FindResource("ActiveColor") as SolidColorBrush;
                if (brush != null)
                    return brush;
            }
            catch { }
            return new SolidColorBrush(Color.FromRgb(50, 50, 50));
        }

        private SolidColorBrush GetHoverBrush()
        {
            try
            {
                var brush = FindResource("BaseColor") as SolidColorBrush;
                if (brush != null)
                    return brush;
            }
            catch { }
            return new SolidColorBrush(Color.FromRgb(0, 0, 0));
        }

        public void ClearSort()
        {
            _currentSortColumn = null;
            _isAscending = true;
            UpdateSortArrows();
        }

        public void SetSort(string columnName, bool ascending = true)
        {
            if (_columns.ContainsKey(columnName))
            {
                _currentSortColumn = columnName;
                _isAscending = ascending;
                UpdateSortArrows();
            }
        }

        public void ClearColumns()
        {
            HeaderGrid.ColumnDefinitions.Clear();
            HeaderGrid.Children.Clear();
            _columns.Clear();
            _currentSortColumn = null;
        }

        private class ColumnInfo
        {
            public StackPanel Panel { get; set; } = new StackPanel();
            public TextBlock TextBlock { get; set; } = new TextBlock();
            public TextBlock Arrow { get; set; } = new TextBlock();
            public string DisplayName { get; set; } = string.Empty;
        }
    }

    public class SortEventArgs : EventArgs
    {
        public string ColumnName { get; }
        public bool IsAscending { get; }

        public SortEventArgs(string columnName, bool isAscending)
        {
            ColumnName = columnName;
            IsAscending = isAscending;
        }
    }
}