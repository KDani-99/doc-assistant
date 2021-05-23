using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Client.Utils;
using DocAssistant_Common.Models;

namespace Client
{
    /// <summary>
    /// Interaction logic for EditPatientWindow.xaml
    /// </summary>
    public partial class EditPatientWindow : Window
    {
        private readonly Patient _patient;
        private DoctorWindow _doctorWindow;
        
        public EditPatientWindow(Patient patient,DoctorWindow doctorWindow)
        {
            InitializeComponent();
            this._patient = patient;
            this._doctorWindow = doctorWindow;
            FillData();
        }

        private void FillData()
        {
            PatientFirstnameAddTB.Text = _patient.FirstName;
            PatientLastnameAddTB.Text = _patient.LastName;

            var male = _patient.Gender == Patient.GenderEnum.Male;
            if (male)
            {
                FemaleCheckBox.IsChecked = false;
                MaleCheckBox.IsChecked = true;
            }
            else
            {
                FemaleCheckBox.IsChecked = true;
                MaleCheckBox.IsChecked = false;
            }

            PatientStateAddTB.Text = _patient.State;
            PatientCityAddTB.Text = _patient.City;
            PatientCountryAddTB.Text = _patient.Country;
            PatientStreetAddTB.Text = _patient.State;
            PatientDateOfBirthAddTB.Text = _patient.DateOfBirth.ToString();
            PatientSSNAddTB.Text = _patient.SSN;
            PatientZIPAddTB.Text = _patient.ZIP;
            PatientComplaintAddTB.Text = _patient.Complaint;
        }

        private async void PatientFinishAdding_Click(object sender, RoutedEventArgs e)
        {
            bool value = FemaleCheckBox.IsChecked.Value;
            Patient.GenderEnum gender = value ? Patient.GenderEnum.Female : Patient.GenderEnum.Male;
            
            _patient.FirstName = PatientFirstnameAddTB.Text;
            _patient.LastName = PatientLastnameAddTB.Text;
            _patient.Gender = gender;
            _patient.Country = PatientCountryAddTB.Text;
            _patient.State = PatientStateAddTB.Text;
            _patient.City = PatientCityAddTB.Text;
            _patient.Street = PatientStreetAddTB.Text;
            _patient.ZIP = PatientZIPAddTB.Text;
            _patient.SSN = PatientSSNAddTB.Text;
            _patient.DateOfBirth = DateTime.Parse(PatientDateOfBirthAddTB.Text);
            _patient.Complaint = PatientComplaintAddTB.Text;


            if (await HttpHandler.EditPatient(_patient))
            {
                _doctorWindow.RefreshGrid();
                MessageBox.Show("Successfully updated the patient's data", "Successful request", MessageBoxButton.OK,
                    MessageBoxImage.Information);
                Close();
            }
        }

        private void PatientQuitAdding_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MaleCheckBox_Click(object sender, RoutedEventArgs e)
        {
            FemaleCheckBox.IsChecked = false;
        }

        private void FemaleCheckBox_Click(object sender, RoutedEventArgs e)
        {
            MaleCheckBox.IsChecked = false;
        }
    }
}
