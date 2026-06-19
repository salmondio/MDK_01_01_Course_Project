using Course_project;
using Course_project_wpf.Controllers;
using Course_project_wpf.Models.FullModels;
using Course_project_wpf.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ReportModel = Course_project_wpf.Models.FullModels.Report;

namespace Course_project_wpf.Elements.OwnerAdmin
{
    public partial class Report : UserControl, IDisposable
    {
        #region Constants

        private const int MaxTextLength = 50;
        private const int DisplayTextCutoff = 47;
        private const double NormalHeight = 90;
        private const double ExpandedAreaMargin = 10;

        private static readonly Color DefaultColor = Color.FromRgb(111, 158, 123);
        private static readonly Color DefaultDarkColor = Color.FromRgb(82, 139, 104);
        private static readonly Color StatusNewColor = Color.FromRgb(255, 193, 7);
        private static readonly Color StatusInProgressColor = Color.FromRgb(33, 150, 243);
        private static readonly Color StatusResolvedColor = Color.FromRgb(76, 175, 80);
        private static readonly Color StatusRejectedColor = Color.FromRgb(244, 67, 54);
        private static readonly Color StatusUnknownColor = Color.FromRgb(158, 158, 158);
        private static readonly Color DarkNewColor = Color.FromRgb(255, 160, 0);
        private static readonly Color DarkInProgressColor = Color.FromRgb(25, 118, 210);
        private static readonly Color DarkResolvedColor = Color.FromRgb(56, 142, 60);
        private static readonly Color DarkRejectedColor = Color.FromRgb(211, 47, 47);
        private static readonly Color DarkUnknownColor = Color.FromRgb(117, 117, 117);

        #endregion

        #region Fields

        private ReportModel _report;
        private bool _isAdd;
        private bool _isLoading;
        private bool _canEdit;
        private bool _isEditing;
        private bool _isExpanded;
        private Models.FullModels.MessageStatus _currentStatus;
        private List<Models.FullModels.MessageStatus> _statuses;
        private ComboBox _statusComboBox;
        private bool _isDisposed;

        #endregion

        #region Constructors

        public Report()
        {
            _isAdd = true;
            _canEdit = CheckEditPermissions();

            CreateDefaultResources();
            InitializeComponent();

            InitializeControl();
            UpdateVisibility();
        }

        public Report(ReportModel report)
        {
            if (report == null)
                throw new ArgumentNullException(nameof(report));

            _report = report;
            _isAdd = false;
            _canEdit = CheckEditPermissions();

            CreateResourcesFromReport(report);
            InitializeComponent();
            InitializeControl();

            this.Unloaded += OnUnloaded;
            this.Loaded += OnLoaded;
            ExpandedArea.SizeChanged += ExpandedArea_SizeChanged;
        }

        #endregion

        #region Initialization

        private void InitializeControl()
        {
            this.Height = NormalHeight;
            this.VerticalAlignment = VerticalAlignment.Top;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            InitializeReportDataAsync();
        }

        private async void InitializeReportDataAsync()
        {
            try
            {
                await InitializeVariables(_report);
                await LoadStatuses();

                if (!_isExpanded)
                {
                    this.Height = NormalHeight;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing report data: {ex.Message}");
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            CleanupResources();
        }

        private void CleanupResources()
        {
            this.Loaded -= OnLoaded;
            this.Unloaded -= OnUnloaded;

            if (ExpandedArea != null)
            {
                ExpandedArea.SizeChanged -= ExpandedArea_SizeChanged;
            }
        }

        private void CreateDefaultResources()
        {
            Resources["LocalColorTheme"] = new SolidColorBrush(DefaultColor);
            Resources["LocalDarkColorTheme"] = new SolidColorBrush(DefaultDarkColor);
        }

        private void CreateResourcesFromReport(ReportModel report)
        {
            var (themeColor, darkThemeColor) = GetReportColors(report);
            Resources["LocalColorTheme"] = new SolidColorBrush(themeColor);
            Resources["LocalDarkColorTheme"] = new SolidColorBrush(darkThemeColor);
        }

        private async System.Threading.Tasks.Task InitializeVariables(ReportModel report)
        {
            if (report == null) return;

            await Dispatcher.InvokeAsync(() =>
            {
                InitializeUserNames(report);
                InitializeTextContent(report);
                lbDate.Content = report.Date_time.ToString("dd.MM.yyyy");
                lbTime.Content = report.Date_time.ToString("HH:mm");
            });

            _currentStatus = GetController.Instance.GetMessageStatus(report.Id_status);
            if (_currentStatus == null)
            {
                await GetController.Instance.GetMessageStatuses(true);
                _currentStatus = GetController.Instance.GetMessageStatus(report.Id_status);
            }

            UpdateStatusDisplay(report.Id_status);
            RefreshColors();
        }

        private void InitializeUserNames(ReportModel report)
        {
            var sender = GetController.Instance.GetUser(report.Id_student);
            Sender.Content = sender != null
                ? $"{sender.Lastname} {sender.Name} {sender.Surname}"
                : $"Студент #{report.Id_student}";

            var receiver = GetController.Instance.GetUser(report.Id_teacher);
            Receiver.Content = receiver != null
                ? $"{receiver.Lastname} {receiver.Name} {receiver.Surname}"
                : $"Преподаватель #{report.Id_teacher}";
        }

        private void InitializeTextContent(ReportModel report)
        {
            var displayText = string.IsNullOrEmpty(report.Text) ? "Текст отсутствует" : report.Text;
            lbFullText.Text = displayText;
            tbText.Text = displayText;

            if (displayText.Length > MaxTextLength)
            {
                lbText.Text = displayText.Substring(0, DisplayTextCutoff) + "...";
                bdText.Cursor = Cursors.Hand;
            }
            else
            {
                lbText.Text = displayText;
                bdText.Cursor = Cursors.Arrow;
            }
        }

        private async System.Threading.Tasks.Task LoadStatuses()
        {
            try
            {
                _statuses = await GetController.Instance.GetMessageStatuses(true);
                if (_statuses != null && _currentStatus != null)
                {
                    UpdateStatusDisplay(_report.Id_status);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading statuses: {ex.Message}");
            }
        }

        #endregion

        #region Permissions

        private bool CheckEditPermissions()
        {
            var currentUserRole = App.CurrentUser?.Id_role;
            return currentUserRole == 1 || currentUserRole == 2 || currentUserRole == 3;
        }

        private void UpdateVisibility()
        {
            if (!_canEdit)
            {
                MainGrid.MouseEnter -= MainGrid_MouseEnter;
                MainGrid.MouseLeave -= MainGrid_MouseLeave;
                gdActions.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        #region Colors

        private (Color themeColor, Color darkThemeColor) GetReportColors(ReportModel report)
        {
            if (report == null)
                return (DefaultColor, DefaultDarkColor);

            try
            {
                return report.Id_status switch
                {
                    1 => (StatusNewColor, DarkNewColor),
                    2 => (StatusInProgressColor, DarkInProgressColor),
                    3 => (StatusResolvedColor, DarkResolvedColor),
                    4 => (StatusRejectedColor, DarkRejectedColor),
                    _ => (StatusUnknownColor, DarkUnknownColor)
                };
            }
            catch
            {
                return (DefaultColor, DefaultDarkColor);
            }
        }

        private void RefreshColors()
        {
            if (_report == null) return;

            var (themeColor, darkThemeColor) = GetReportColors(_report);
            var newColorBrush = new SolidColorBrush(themeColor);
            var newDarkColorBrush = new SolidColorBrush(darkThemeColor);

            Resources["LocalColorTheme"] = newColorBrush;
            Resources["LocalDarkColorTheme"] = newDarkColorBrush;

            ApplyColorsToElements(newColorBrush, newDarkColorBrush);
        }

        private void ApplyColorsToElements(SolidColorBrush colorBrush, SolidColorBrush darkColorBrush)
        {
            MainBorder.Background = colorBrush;
            bdId.Background = colorBrush;
            gdPeople.Background = colorBrush;
            bdText.Background = colorBrush;
            bdDateTime.Background = colorBrush;
            bdActions.Background = colorBrush;
            ExpandedArea.Background = colorBrush;
            Sender.Background = colorBrush;
            Receiver.Background = colorBrush;
        }

        private void UpdateStatusDisplay(int statusId)
        {
            var status = GetController.Instance.GetMessageStatus(statusId);

            if (status != null)
            {
                lbStatus.Text = status.Name;
                var (themeColor, _) = GetReportColors(_report ?? new ReportModel { Id_status = statusId });
                lbStatus.Foreground = new SolidColorBrush(themeColor);
            }
            else
            {
                lbStatus.Text = "Неизвестно";
                lbStatus.Foreground = new SolidColorBrush(StatusUnknownColor);
            }
        }

        #endregion

        #region Expand/Collapse

        private void OnTextClick(object sender, MouseButtonEventArgs e)
        {
            if (lbFullText.Text.Length <= MaxTextLength)
                return;

            ToggleExpand();
        }

        private void ToggleExpand()
        {
            if (_isEditing)
                return;

            _isExpanded = !_isExpanded;

            if (_isExpanded)
            {
                ExpandedArea.Visibility = Visibility.Visible;
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (this.IsLoaded)
                    {
                        UpdateExpandedHeight();
                    }
                }), System.Windows.Threading.DispatcherPriority.Render);
            }
            else
            {
                ExpandedArea.Visibility = Visibility.Collapsed;
                this.Height = NormalHeight;
                InvalidateParentLayout();
            }
        }

        private void ExpandedArea_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_isExpanded && this.IsLoaded)
            {
                UpdateExpandedHeight();
            }
        }

        private void UpdateExpandedHeight()
        {
            if (_isExpanded && this.IsLoaded)
            {
                double expandedHeight = NormalHeight + ExpandedArea.ActualHeight + ExpandedAreaMargin;
                this.Height = expandedHeight;
                InvalidateParentLayout();
            }
        }

        private void InvalidateParentLayout()
        {
            if (Parent is FrameworkElement parent && parent.IsLoaded)
            {
                parent.InvalidateMeasure();
                parent.InvalidateArrange();
            }
        }

        #endregion

        #region Navigation

        private void GoToSender(object sender, RoutedEventArgs e)
        {
            NavigateToUserProfile(_report?.Id_student);
        }

        private void GoToReceiver(object sender, RoutedEventArgs e)
        {
            NavigateToUserProfile(_report?.Id_teacher);
        }

        private void NavigateToUserProfile(int? userId)
        {
            if (_report == null || userId == null) return;

            var user = GetController.Instance.GetUser(userId.Value);
            if (user != null && MainWindowOwner.OwnerWindow?.PageParent != null)
            {
                MainWindowOwner.OwnerWindow.PageParent.Navigate(
                    new Pages.OwnerAdmin.UserProfile(user));
            }
        }

        #endregion

        #region Mouse Events

        private void MainGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            if (_canEdit && !_isEditing && this.IsLoaded)
            {
                DeleteButton.Visibility = Visibility.Visible;
                EditButton.Visibility = Visibility.Visible;
            }
        }

        private void MainGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!_isEditing && this.IsLoaded)
            {
                DeleteButton.Visibility = Visibility.Collapsed;
                EditButton.Visibility = Visibility.Collapsed;
            }
        }

        private void SenderButton_MouseEnter(object sender, MouseEventArgs e)
        {
            Sender.Background = (SolidColorBrush)Resources["LocalDarkColorTheme"];
        }

        private void SenderButton_MouseLeave(object sender, MouseEventArgs e)
        {
            Sender.Background = (SolidColorBrush)Resources["LocalColorTheme"];
        }

        private void ReceiverButton_MouseEnter(object sender, MouseEventArgs e)
        {
            Receiver.Background = (SolidColorBrush)Resources["LocalDarkColorTheme"];
        }

        private void ReceiverButton_MouseLeave(object sender, MouseEventArgs e)
        {
            Receiver.Background = (SolidColorBrush)Resources["LocalColorTheme"];
        }

        #endregion

        #region Edit Mode

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_canEdit || _isLoading)
                return;

            EnterEditMode();
        }

        private void EnterEditMode()
        {
            _isEditing = true;

            EditButton.Visibility = Visibility.Collapsed;
            DeleteButton.Visibility = Visibility.Collapsed;
            SaveButton.Visibility = Visibility.Visible;
            CancelButton.Visibility = Visibility.Visible;

            lbText.Visibility = Visibility.Collapsed;
            tbText.Visibility = Visibility.Visible;
            tbText.Text = lbFullText.Text;

            if (_isExpanded)
            {
                _isExpanded = false;
                ExpandedArea.Visibility = Visibility.Collapsed;
                this.Height = NormalHeight;
            }

            CreateStatusComboBox();
        }

        private void CreateStatusComboBox()
        {
            try
            {
                _statusComboBox = new ComboBox
                {
                    Name = "StatusComboBox",
                    Margin = new Thickness(5),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Width = 80,
                    Height = 25,
                    FontSize = 12
                };

                var statuses = GetController.Instance.MessageStatuses;
                if (statuses != null)
                {
                    foreach (var status in statuses)
                    {
                        var item = new ComboBoxItem
                        {
                            Content = status.Name,
                            Tag = status.Id
                        };
                        _statusComboBox.Items.Add(item);

                        if (status.Id == _report?.Id_status)
                            _statusComboBox.SelectedItem = item;
                    }
                }

                // Теперь заменяем lbStatus в gdDateTime (Grid)
                var parent = gdDateTime;
                if (parent != null)
                {
                    var row = Grid.GetRow(lbStatus);
                    var column = Grid.GetColumn(lbStatus);

                    if (parent.Children.Contains(lbStatus))
                    {
                        parent.Children.Remove(lbStatus);
                    }

                    Grid.SetRow(_statusComboBox, row);
                    Grid.SetColumn(_statusComboBox, column);
                    parent.Children.Add(_statusComboBox);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating status combobox: {ex.Message}");
            }
        }

        private void RemoveStatusComboBox()
        {
            if (_statusComboBox != null)
            {
                var parent = _statusComboBox.Parent as Grid;
                if (parent != null)
                {
                    var row = Grid.GetRow(_statusComboBox);
                    var column = Grid.GetColumn(_statusComboBox);

                    if (parent.Children.Contains(_statusComboBox))
                    {
                        parent.Children.Remove(_statusComboBox);
                    }

                    if (!parent.Children.Contains(lbStatus))
                    {
                        Grid.SetRow(lbStatus, row);
                        Grid.SetColumn(lbStatus, column);
                        parent.Children.Add(lbStatus);
                    }
                }

                _statusComboBox = null;
            }
        }

        private void ExitEditMode()
        {
            _isEditing = false;

            SaveButton.Visibility = Visibility.Collapsed;
            CancelButton.Visibility = Visibility.Collapsed;

            if (_canEdit)
            {
                EditButton.Visibility = Visibility.Visible;
                DeleteButton.Visibility = Visibility.Visible;
            }

            RemoveStatusComboBox();

            tbText.Visibility = Visibility.Collapsed;
            lbText.Visibility = Visibility.Visible;

            this.Height = NormalHeight;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ExitEditMode();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isLoading || _report == null)
                return;

            int selectedStatusId = GetSelectedStatusId();

            if (string.IsNullOrWhiteSpace(tbText.Text))
            {
                MessageBox.Show("Текст жалобы не может быть пустым", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _isLoading = true;
            SaveButton.IsEnabled = false;

            try
            {
                var updatedReport = new ReportModel
                {
                    Id = _report.Id,
                    Id_student = _report.Id_student,
                    Id_teacher = _report.Id_teacher,
                    Id_status = selectedStatusId,
                    Id_inspector = _report.Id_inspector,
                    Date_time = _report.Date_time,
                    Text = tbText.Text.Trim(),
                    Is_active = _report.Is_active
                };

                var result = await PutController.Instance.OwnerUpdateReport(updatedReport);

                if (result != null)
                {
                    _report = result;
                    await GetController.Instance.GetReports(true);

                    ExitEditMode();
                    await Dispatcher.InvokeAsync(() => InitializeVariables(_report));

                    MessageBox.Show("Жалоба успешно обновлена", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Не удалось обновить жалобу", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
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

        private int GetSelectedStatusId()
        {
            if (_statusComboBox?.SelectedItem is ComboBoxItem selectedItem)
            {
                return (int)selectedItem.Tag;
            }
            return _report?.Id_status ?? 0;
        }

        #endregion

        #region Delete

        private async void Delete(object sender, RoutedEventArgs e)
        {
            if (_isLoading || _isEditing || _report == null)
                return;

            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить жалобу?\nОтправитель: {Sender.Content}\nПолучатель: {Receiver.Content}",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            _isLoading = true;
            DeleteButton.IsEnabled = false;

            try
            {
                var deletedReport = await DeleteController.Instance.DeleteReport(_report.Id);

                if (deletedReport != null)
                {
                    RemoveFromParent();
                    await GetController.Instance.GetReports(true);

                    MessageBox.Show("Жалоба успешно удалена", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Не удалось удалить жалобу", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _isLoading = false;
                DeleteButton.IsEnabled = true;
            }
        }

        private void RemoveFromParent()
        {
            if (Parent is Panel parentPanel && parentPanel.Children.Contains(this))
            {
                parentPanel.Children.Remove(this);
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            if (disposing)
            {
                CleanupResources();
                _statusComboBox = null;
                _statuses = null;
                _currentStatus = null;
            }

            _isDisposed = true;
        }

        ~Report()
        {
            Dispose(false);
        }

        #endregion
    }
}