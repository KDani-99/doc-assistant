using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows;
using DocAssistant_Common.Models;
using Newtonsoft.Json.Linq;
using System.Net;
using Client.Utils;

namespace Client
{
    /// <summary>
    /// Interaction logic for AddingAssistant.xaml
    /// </summary>
    public partial class AddingAssistant : Window
    {

        public AddingAssistant()
        {
            InitializeComponent();
        }

        private async void AssistantFinishAdding_Click(object sender, RoutedEventArgs e)
        {
            var username = UsernameTextBox.Text;
            var password = PasswordBox.Password;

            if (!await HttpHandler.RegisterAssistant(username, password)) return;
                
            MessageBox.Show("Successful assistant registration!", "Successful request");
            Close();
        }

        private void AssistantQuitAdding_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
