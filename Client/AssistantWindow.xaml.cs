using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using DocAssistant_Common.Models;
using Newtonsoft.Json;
using Client.Utils;
using DocAssistant_Common.Utils;

namespace Client
{
    /// <summary>
    /// Interaction logic for AssistantWindow.xaml
    /// </summary>
    public partial class AssistantWindow : Window, IDataWindow
    {
        private string _collectionHash;
        private bool _update = true;
        
        private async Task GetPatients()
        {
            var patients = new PatientList<Patient>(await HttpHandler.LoadPatients());

            var hash = Convert.ToHexString(
                Security.ComputeHash(JArray.FromObject(patients).ToString(Formatting.None)));

            if (!hash.Equals(_collectionHash))
            {
                AsDataGrid.Items.Clear();
                FillData(patients);
                _collectionHash = hash;
            }
        }

        private void FillData(IEnumerable<Patient> patients)
        {
            foreach (var patient in patients)
                AsDataGrid.Items.Add(patient);
        }

        public AssistantWindow()
        {
            InitializeComponent();
            
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

        private void AssistantPatientAdd_Click(object sender, RoutedEventArgs e)
        {
            AddingPatientAssistant win1 = new AddingPatientAssistant(this);
            win1.Show();
        }

        public void AddPatient(Patient patient)
        {
            AsDataGrid.Items.Add(patient);
            var hash = Convert.ToHexString(
                Security.ComputeHash(JArray.FromObject(AsDataGrid.Items).ToString(Formatting.None)));
        }

        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow winBack = new MainWindow();
            winBack.Show();
            
            if (!await HttpHandler.LogoutDoctor())
                e.Cancel = true;
        }
    }
}
