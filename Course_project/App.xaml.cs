using Course_project_wpf.Common;
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


        private void AppStartup()
        {
            // Инициализируем ApiClient
            ApiClient.Initialize("https://localhost:7205/");

            // Выводим окно входа
            MainWindowAuthorization mainWindowAuthorization = new MainWindowAuthorization();
            mainWindowAuthorization.Show();
        }
    }

}
