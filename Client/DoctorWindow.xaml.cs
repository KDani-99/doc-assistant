using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Net.Http;
using System.Numerics;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Client.Utils;
using DocAssistant_Common.Models;
using DocAssistant_Common.Utils;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Client
{
    /// <summary>
    /// Interaction logic for DoctorWindow.xaml
    /// </summary>

    public partial class DoctorWindow : Window, IDataWindow
    {
        private bool _update = true;

        private string _collectionHash;

        private async Task GetPatients()
        {
            var patients = new List<Patient>(await HttpHandler.LoadPatients());

            var hash = Convert.ToHexString(
                Security.ComputeHash(JArray.FromObject(patients).ToString(Formatting.None)));

            if (!hash.Equals(_collectionHash))
            {
                DoctorDataGrid.Items.Clear();
                FillData(patients);
                _collectionHash = hash;
            }
        }

        private void FillData(IEnumerable<Patient> patients)
        {
            foreach (var patient in patients)
                DoctorDataGrid.Items.Add(patient);
        }

        public DoctorWindow()
        {
            InitializeComponent();

            this.Closed += (sender, args) => _update = false;

            GetPatients();
            
            new Thread(async () =>
            {
                DateTime previousUpdate = DateTime.Now;
                
                while (_update)
                {
                    var now = DateTime.Now;

                    if (now.Subtract(previousUpdate).Seconds >= 10)
                    {
                        if (await HttpHandler.CheckForUpdates(_collectionHash))
                        {
                            Dispatcher.InvokeAsync(async () => await GetPatients());
                        }
                        
                        previousUpdate = now;
                    }
                    
                }
                Thread.Sleep(1000);
            }).Start();
        }        
        
        private void DiagnosisAdd_Click(object sender, RoutedEventArgs e)
        {
            if (DoctorDataGrid.SelectedItem is Patient selectedItem)
            {
                AddingDiagnose win3 = new();
                win3.Patient = selectedItem;
                win3.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select a patient", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void AssistantAdd_Click(object sender, RoutedEventArgs e)
        {
            AddingAssistant win4 = new AddingAssistant();
            win4.ShowDialog();
        }

        private async void PatientDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (DoctorDataGrid.SelectedItem is Patient selectedItem)
            {
                if (await HttpHandler.DeletePatient(selectedItem.Id))
                    DoctorDataGrid.Items.Remove(selectedItem);
            }
            else
            {
                MessageBox.Show("Please select a patient", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        
        
        public void AddPatient(Patient patient)
        {
            DoctorDataGrid.Items.Add(patient);

            _collectionHash = Convert.ToHexString(
                Security.ComputeHash(JArray.FromObject(DoctorDataGrid.Items).ToString(Formatting.None)));
        }
        
        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow winBack = new MainWindow();
            winBack.Show();
            
            if (!await HttpHandler.LogoutDoctor())
                e.Cancel = true;

        }

        private void PatientAdd_Click(object sender, RoutedEventArgs e)
        {
            AddingPatientAssistant win1 = new AddingPatientAssistant(this);
            win1.ShowDialog();
        }

        public void RefreshGrid()
        {
            DoctorDataGrid.Items.Refresh();
        }
        
        private void PatientEditDataButton_Click(object sender, RoutedEventArgs e)
        {
            if (DoctorDataGrid.SelectedItem is Patient selectedItem)
            {
                EditPatientWindow editPatientWindow = new EditPatientWindow(selectedItem,this);
                editPatientWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select a patient", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void PreviousDiagnoses_Click(object sender, RoutedEventArgs e)
        {
            if (DoctorDataGrid.SelectedItem is Patient selectedItem)
            {
                var openPreviousDiagnosesWindow = new PreviousDiagnoses(selectedItem);
                    
                foreach (var diagnosis in selectedItem.Diagnoses)
                    openPreviousDiagnosesWindow.AddDiagnosis(diagnosis);

                openPreviousDiagnosesWindow.ShowDialog();

            }
            else
            {
                MessageBox.Show("Please select a patient", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

        }
        
    }
}
