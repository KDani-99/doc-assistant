using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Windows;
using Client.Utils;
using DocAssistant_Common.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Client
{
    /// <summary>
    /// Interaction logic for AddingPatient.xaml
    /// </summary>
    public partial class AddingPatientAssistant : Window
    {
        
        private readonly IDataWindow _dataWindow;
        

        public AddingPatientAssistant(IDataWindow dataWindow)
        {
            InitializeComponent();
            this._dataWindow = dataWindow;
        }

        private async void PatientFinishAdding_Click(object sender, RoutedEventArgs e)
        {
           

            bool value = FemaleCheckBox.IsChecked.Value;
            
            Patient.GenderEnum gender = value ? Patient.GenderEnum.Female : Patient.GenderEnum.Male;

            var isValidDate = DateTime.TryParse(PatientDateOfBirthAddTB.Text, out var dateTime);

            if (!isValidDate)
            {
                MessageBox.Show("Invalid date", "Patient registration failed");
                return;
            }
            
            var patientData = new Patient()
            {
                FirstName = PatientFirstnameAddTB.Text,
                LastName = PatientLastnameAddTB.Text,
                Gender = gender,
                Country = PatientCountryAddTB.Text,
                State = PatientStateAddTB.Text,
                City = PatientCityAddTB.Text,
                Street = PatientStreetAddTB.Text,
                ZIP = PatientZIPAddTB.Text,
                SSN = PatientSSNAddTB.Text,
                DateOfBirth = dateTime,
                Complaint = PatientComplaintAddTB.Text,
            };

            var result = await HttpHandler.AddPatient(patientData);
            
            if (result is not null)
            {
                _dataWindow.AddPatient(result);
                MessageBox.Show("Successful patient registration!", "Successful request");
                Close();
            }
        }

        private void PatientQuitAdding_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        

        private void FemaleCheckBox_Click(object sender, RoutedEventArgs e)
        {
            MaleCheckBox.IsChecked = false;
        }

        private void MaleCheckBox_OnClick(object sender, RoutedEventArgs e)
        {
            FemaleCheckBox.IsChecked = false;
        }
    }
}
