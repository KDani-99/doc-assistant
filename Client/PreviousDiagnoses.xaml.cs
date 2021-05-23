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
using Newtonsoft.Json.Linq;

namespace Client
{
    /// <summary>
    /// Interaction logic for PreviousDiagnoses.xaml
    /// </summary>
    public partial class PreviousDiagnoses : Window
    {
        private readonly Patient _patient;
        public PreviousDiagnoses(Patient doctorWindow)
        {
            InitializeComponent();
            this._patient = doctorWindow;
        }

        public void AddDiagnosis(Diagnosis diagnosis)
        {
            PreviousDiagnosesGrid.Items.Add(diagnosis);
        }
        
        private async void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
            {
                if (PreviousDiagnosesGrid.SelectedItem is Diagnosis diagnosis)
                {
                    if (await HttpHandler.DeleteDiagnosis(diagnosis))
                    {
                        _patient.Diagnoses.Remove(diagnosis);
                    
                        PreviousDiagnosesGrid.Items.Remove(PreviousDiagnosesGrid.SelectedItem);
                        PreviousDiagnosesGrid.Items.Refresh();
                    }
                }
                else
                {
                    MessageBox.Show("Please select a diagnosis", "Delete failed", MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }
        }
    }
}
