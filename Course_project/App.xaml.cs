using Course_project_wpf.Helpers;
using Course_project_wpf.Models;
using Course_project_wpf.Windows;
using System.Configuration;
using System.Data;
using System.Windows;

namespace Course_project
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static UserAllInfo? CurrentUser { get; set; }
        public static string? JwtToken { get; set; }

        private void AppStartup(object sender, StartupEventArgs e)
        {
            // Инициализируем ApiClient
            ApiClient.Initialize("https://localhost:7205/");

            // Выводим окно входа
            MainWindowAuthorization mainWindowAuthorization = new MainWindowAuthorization();
            mainWindowAuthorization.Show();
        }
    }

}
