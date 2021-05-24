using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DocAssistant_Common.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Client.Utils
{
    public abstract class HttpHandler
    {
        private static readonly HttpClient Client = new();

        public static async Task<IEnumerable<Patient>> LoadPatients()
        {
            try
            {
                var response = await Client.GetAsync(Config.ServerAddress + "/patient?type=all");


                if (response.StatusCode == HttpStatusCode.BadRequest)
                    ShowRequestErrors(await response.Content.ReadAsStringAsync());
                else if (response.StatusCode != HttpStatusCode.OK) throw new Exception("An unexpected error has occured");

                return JsonConvert.DeserializeObject<List<Patient>>(await response.Content.ReadAsStringAsync());
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Failed to load patients");
                return null;
            }
        }

        public static async Task<Patient> AddPatient(Patient patient)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(patient), Encoding.UTF8, "application/json");     
                var response = await Client.PostAsync(Config.ServerAddress + "/patient",content);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var updatedPatient = JsonConvert.DeserializeObject<Patient>(await response.Content.ReadAsStringAsync());

                    return updatedPatient;
                }
                
                if(response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    MessageBox.Show("Unauthorized", "Patient registration failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }

                if(response.StatusCode == HttpStatusCode.BadRequest)
                {
                    ShowRequestErrors(await response.Content.ReadAsStringAsync());
                }
                
                return null;
                
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Failed to add patient", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public static async Task<bool> EditPatient(Patient patient)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(patient), Encoding.UTF8, "application/json");     
                var response = await Client.PatchAsync(Config.ServerAddress + "/patient",content);

                if (response.StatusCode == HttpStatusCode.OK) return true;

                if(response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    MessageBox.Show("Unauthorized", "Patient edit failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                if(response.StatusCode == HttpStatusCode.BadRequest)
                {
                    MessageBox.Show("ASD");
                    ShowRequestErrors(await response.Content.ReadAsStringAsync());
                }
                
                return false;
                
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Failed to edit patient", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
        
        public static async Task<Diagnosis> CreateDiagnosis(Diagnosis diagnosis)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(diagnosis), Encoding.UTF8, "application/json");     
                var response = await Client.PostAsync(Config.ServerAddress + "/patient/diagnosis?id="+diagnosis.PatientId,content);

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    ShowRequestErrors(await response.Content.ReadAsStringAsync());
                    return null;
                }
                if (response.StatusCode != HttpStatusCode.OK) throw new Exception("An unexpected error has occured");
                
                return JsonConvert.DeserializeObject<Diagnosis>(await response.Content.ReadAsStringAsync());
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Failed to add diagnosis", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public static async Task<bool> RegisterDoctor(Credentials credentials)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(credentials), Encoding.UTF8, "application/json");
                var response = await Client.PostAsync("http://localhost:5000/api/doc/", content);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    JObject data = JObject.Parse(await response.Content.ReadAsStringAsync());

                    if (data["errors"] is JObject errors)
                    {
                        if (errors.TryGetValue("Password", out var passwordErrors))
                        {
                            var errorMessage = passwordErrors.ToObject<JArray>()[0].ToString();
                            MessageBox.Show(errorMessage, "Doctor registration failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        }

                        if (errors.TryGetValue("Username", out var usernameErrors))
                        {
                            var errorMessage = usernameErrors.ToObject<JArray>()[0].ToString();
                            MessageBox.Show(errorMessage, "Doctor registration failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        }

                    }

                    return false;
                }

                return true;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Failed to register a doctor account", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public static async Task<bool> CheckForUpdates(string hashCode)
        {
            try
            {
                var response = await Client.GetAsync(Config.ServerAddress + "/patient/updates?hash="+hashCode);

                if (response.StatusCode == HttpStatusCode.ResetContent)
                    return true;

                return false;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Failed to check for updates", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public static async Task<bool> DeleteDiagnosis(Diagnosis diagnosis)
        {
            try
            {
                var response = await Client.DeleteAsync(Config.ServerAddress + "/patient/diagnosis?patientId="+diagnosis.PatientId + "&diagnosisId="+diagnosis.Id);
                
                if(response.StatusCode == HttpStatusCode.BadRequest)
                    ShowRequestErrors(await response.Content.ReadAsStringAsync());
                else if(response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Exception("Unauthorized");
                else if (response.StatusCode == HttpStatusCode.OK)
                    return true;

                return false;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Failed to delete the diagnosis", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
        
        public static async Task<bool> DeletePatient(long id)
        {
            try
            {
                var response = await Client.DeleteAsync(Config.ServerAddress + "/patient?id="+id);

                if (response.StatusCode != HttpStatusCode.OK) throw new Exception("An unexpected error has occured");

                return true;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Failed to delete patient", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private static async Task<bool> Login(string username, string password, string type)
        {
            try
            {
                Credentials credentials = new Credentials
                {
                    Username = username,
                    Password = password
                };
                
                var content = new StringContent(JsonConvert.SerializeObject(credentials), Encoding.UTF8, "application/json");     
                var response = await Client.PostAsync(Config.ServerAddress + "/auth?type=" + type,content);

                if (response.StatusCode != HttpStatusCode.OK) throw new Exception("Invalid username or password");

                var data = JObject.Parse(await response.Content.ReadAsStringAsync());

                if (!data.TryGetValue("accessToken", out var accessToken)) throw new Exception("Access token was not received");
                
                SetAuthToken(accessToken.ToString());

                return true;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);

                return false;
            }
        }
        
        public static Task<bool> LoginDoctor(string username, string password)
        {
            return Login(username, password, "doctor");
        }

        public static Task<bool> LoginAssistant(string username, string password)
        {
            return Login(username, password, "assistant");
        }

        private static async Task<bool> Logout(string type)
        {
            try
            {
                var response = await Client.PostAsync(Config.ServerAddress + "/logout?type=" + type,null);

                if (response.StatusCode != HttpStatusCode.OK) throw new Exception("An unexpected error has occured");

                RemoveAuthToken();
                
                return true;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Logout failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public static Task<bool> LogoutAssistant()
        {
            return Logout("assistant");
        }
        
        public static Task<bool> LogoutDoctor()
        {
            return Logout("doctor");
        }

        public static async Task<bool> RegisterAssistant(string username, string password)
        {
            try
            {
                Credentials credentials = new Credentials
                {
                    Username = username,
                    Password = password
                };
                
                var content = new StringContent(JsonConvert.SerializeObject(credentials), Encoding.UTF8, "application/json");    
                var response = await Client.PostAsync(Config.ServerAddress + "/assistant",content);

                switch (response.StatusCode)
                {
                    case HttpStatusCode.Unauthorized:
                        throw new Exception("Unauthorized");
                    case HttpStatusCode.BadRequest:
                        ShowRequestErrors(await response.Content.ReadAsStringAsync(), "Assistant registration failed");
                        return false;
                }
                
                return true;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Assistant registration failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private static void ShowRequestErrors(string dataString, string caption = "Error")
        {
            JObject data = JObject.Parse(dataString);

            if (data.TryGetValue("errors", out var errors))
            {
                foreach(var err in errors)
                {
                    var errorArray = err.First as JArray;
                            
                    string errorMessage = String.Join('\n',errorArray);
                    MessageBox.Show(errorMessage,caption);
                }
            }
            else if (data.TryGetValue("error", out _))
            {
                MessageBox.Show(data["error"].ToString(), data["title"].ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private static void SetAuthToken(string accessToken)
        {
            RemoveAuthToken();
            Client.DefaultRequestHeaders.Add("Authorization",accessToken);
        }

        private static void RemoveAuthToken()
        {
            Client.DefaultRequestHeaders.Remove("Authorization");
        }
    }
}