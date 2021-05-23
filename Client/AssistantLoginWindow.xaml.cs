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
    public partial class AssistantLoginWindow : Window
    {        

        public AssistantLoginWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var username = usernameTB.Text;
            var password = passwordTB.Password;

            if (await HttpHandler.LoginAssistant(username, password))
            {
                AssistantWindow win1 = new AssistantWindow();
                win1.Show();
                Close();
            }
        }
            
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow winOrigin = new MainWindow();
            winOrigin.Show();
            Close();
        }
    }
}
