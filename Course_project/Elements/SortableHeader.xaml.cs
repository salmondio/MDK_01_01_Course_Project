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

namespace Course_project_wpf.Elements
{
    /// <summary>
    /// Логика взаимодействия для SortableHeader.xaml
    /// </summary>
    public partial class SortableHeader : UserControl
    {
        // Объявляем событие, которое будет вызываться при клике на колонку
        public event EventHandler<SortEventArgs> SortRequested;

        public SortableHeader()
        {
            InitializeComponent();
        }

        // Метод для добавления колонок
        public void AddColumn(string columnName, string displayName)
        {
            var button = new Button
            {
                Content = displayName,
                Tag = columnName, // Сохраняем имя колонки
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0, 0, 1, 0),
                BorderBrush = (Brush)FindResource("BorderBrush"),
                Padding = new Thickness(10, 5, 10, 5),
                Cursor = Cursors.Hand,
                FontWeight = FontWeights.SemiBold,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            // Добавляем стрелку сортировки
            var stackPanel = new StackPanel { Orientation = Orientation.Horizontal };
            var textBlock = new TextBlock { Text = displayName, Margin = new Thickness(0, 0, 5, 0) };
            var arrow = new TextBlock
            {
                Text = "↕️",
                FontSize = 12,
                Visibility = Visibility.Collapsed
            };

            stackPanel.Children.Add(textBlock);
            stackPanel.Children.Add(arrow);
            button.Content = stackPanel;

            // Сохраняем стрелку в Tag кнопки для обновления
            button.Tag = new { ColumnName = columnName, Arrow = arrow };

            button.Click += (s, e) =>
            {
                // Вызываем событие при клике
                SortRequested?.Invoke(this, new SortEventArgs(columnName));

                // Обновляем стрелки сортировки
                UpdateSortArrows(columnName);
            };

            HeadersPanel.Children.Add(button);
        }

        private void UpdateSortArrows(string currentColumn)
        {
            foreach (UIElement element in HeadersPanel.Children)
            {
                if (element is Button button && button.Tag != null)
                {
                    var tagInfo = button.Tag;
                    var columnName = tagInfo.GetType().GetProperty("ColumnName")?.GetValue(tagInfo);
                    var arrow = tagInfo.GetType().GetProperty("Arrow")?.GetValue(tagInfo) as TextBlock;

                    if (arrow != null)
                    {
                        if (columnName?.ToString() == currentColumn)
                        {
                            arrow.Text = "↑"; // Или ↓ для обратной сортировки
                            arrow.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            arrow.Text = "↕️";
                            arrow.Visibility = Visibility.Collapsed;
                        }
                    }
                }
            }
        }
    }

    // Класс для аргументов события
    public class SortEventArgs : EventArgs
    {
        public string ColumnName { get; set; }
        public bool IsAscending { get; set; } = true;

        public SortEventArgs(string columnName)
        {
            ColumnName = columnName;
        }
    }
}
