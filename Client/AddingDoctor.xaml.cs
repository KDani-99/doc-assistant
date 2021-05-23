using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Windows;
using DocAssistant_Common.Models;
using Newtonsoft.Json.Linq;
using System.Net;
using Client.Utils;

namespace Client
{

    public partial class AddingDoctor : Window
    {
        public AddingDoctor()
        {
            InitializeComponent();
        }

        private async void DoctorFinishAdding_Click(object sender, RoutedEventArgs e)
        {

            var credentials = new Credentials
            {
                Username = doctorNameAdd.Text,
                Password = doctorPWAdd.Password
            };

            if (await HttpHandler.RegisterDoctor(credentials))
            {
                MessageBox.Show("Successful doctor registration!", "Successful request");
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                Close();
            }

        }

        private void DoctorQuitAdding_Click(object sender, RoutedEventArgs e)
        {
            MainWindow winOrigin = new MainWindow();
            winOrigin.Show();
            Close();
        }
    }
}
