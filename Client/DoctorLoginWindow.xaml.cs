using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Windows;
using Client.Utils;
using DocAssistant_Common.Models;
using Newtonsoft.Json.Linq;

namespace Client
{
    public partial class DoctorLoginWindow : Window
    {

        public DoctorLoginWindow()
        {
            InitializeComponent();
        }
        
        private async void DoctorLoginButton_Click(object sender, RoutedEventArgs e)
        {
            var username = drUsernameTB.Text;
            var password = drPasswordTB.Password;

            if (await HttpHandler.LoginDoctor(username, password))
            {
                DoctorWindow win1 = new DoctorWindow();
                win1.Show();
                Close();
            }

        }

        private void DoctorBackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow winOrigin = new MainWindow();
            winOrigin.Show();
            Close();
        }
    }

    
}
