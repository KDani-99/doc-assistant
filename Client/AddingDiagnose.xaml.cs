using System;
using System.Text;
using System.Windows;
using DocAssistant_Common.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;
using Client.Utils;
using Newtonsoft.Json.Linq;

namespace Client
{
    /// <summary>
    /// Interaction logic for AddingPatient.xaml
    /// </summary>

    public partial class AddingDiagnose : Window
    {

        public Patient Patient
        {
            get;
            set;
        }
                
        public AddingDiagnose()
        {
            InitializeComponent();
        }

        private async void DiagnoseFinishAdding_Click(object sender, RoutedEventArgs e)
        {
            var diagnosis = new Diagnosis
            {
                PatientId = Patient.Id,
                Title = DiagnosisTitle.Text,
                Description = DianosisDescription.Text
            };

            var uploadedDiagnosis = await HttpHandler.CreateDiagnosis(diagnosis);
            
            if (uploadedDiagnosis is not null)
            {
                Patient.Diagnoses.Add(uploadedDiagnosis);
                MessageBox.Show("Successful", "Successful request",MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            
        }

        public void DiagnoseQuitAdding_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
