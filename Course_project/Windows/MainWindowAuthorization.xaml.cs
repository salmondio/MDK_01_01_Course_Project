using Course_project;
using Course_project_wpf.Helpers;
using Course_project_wpf.Models;
using Microsoft.AspNetCore.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Course_project_wpf.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindowAuthorization.xaml
    /// </summary>
    public partial class MainWindowAuthorization : Window
    {
        public MainWindowAuthorization()
        {
            InitializeComponent();
        }


        private void LogIn(object sender, RoutedEventArgs e)
        {
            LoginRequest loginRequest = new LoginRequest()
            {
                Email = tbEmail.Text,
                Password = pbPassword.Password
            };

            TryLogIn(loginRequest);
        }


        private void WindowKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                LoginRequest loginRequest = new LoginRequest()
                {
                    Email = tbEmail.Text,
                    Password = pbPassword.Password
                };

                TryLogIn(loginRequest);
            }
        }



        private async void TryLogIn(LoginRequest loginRequest)
        {
            btnLogIn.IsEnabled = false;

            if (await AuthorizationHelper.TryLogIn(loginRequest))
                this.Close();
            else
                btnLogIn.IsEnabled = true;
        }
    }
}
