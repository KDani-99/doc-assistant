using System;
using System.Windows;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private void AssistantLoginButton_Click(object sender, RoutedEventArgs e)
        {
            AssistantLoginWindow win2 = new AssistantLoginWindow();
            win2.Show();
            this.Close();
        }

        private void DoctorLoginButton_Click(object sender, RoutedEventArgs e)
        {            
            DoctorLoginWindow win1 = new DoctorLoginWindow();
            win1.Show();
            this.Close();
        }

        private void AddingDoctorButton_Click(object sender, RoutedEventArgs e)
        {
            AddingDoctor win3 = new AddingDoctor();
            win3.Show();
            this.Close();
        }
    }
                     
}


 