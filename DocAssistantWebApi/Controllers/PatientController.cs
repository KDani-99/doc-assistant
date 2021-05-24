using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DocAssistant_Common.Models;
using DocAssistant_Common.Utils;
using DocAssistantWebApi.Database.Repositories;
using DocAssistantWebApi.Errors;
using DocAssistantWebApi.Filters;
using DocAssistantWebApi.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DocAssistantWebApi.Controllers
{
    [ApiController]
    public class PatientController : ControllerBase
    {

        private readonly IRepository<Patient> _patientRepository;
        private readonly IRepository<Assistant> _assistantRepository;
        private readonly IRepository<Diagnosis> _diagonisRepository;

        public PatientController(IRepository<Patient> repository,IRepository<Assistant> assistantRepository, IRepository<Diagnosis> diagonisRepository)
        {
            this._patientRepository = repository;
            this._assistantRepository = assistantRepository;
            this._diagonisRepository = diagonisRepository;
        }

        [Authorize(Policy = "AssistantRequirement")]
        [Produces("application/json")]
        [Route("api/patient")]
        [HttpGet]
        public virtual async Task<ActionResult> LoadPatient([FromQuery(Name = "type")] string type, [FromQuery(Name = "id")] long? id)
        {
            var accountType = (Roles)HttpContext.Items["AccountType"];
            long docId;
            
            if (accountType == Roles.Assistant)
            {
                var assistant =
                    await this._assistantRepository.Where(assistant => assistant.Id == (long) HttpContext.Items["Id"]);

                docId = assistant.DoctorId;
            }
            else
                docId = (long)HttpContext.Items["Id"];
            

            switch (type)
            {
                case "single":
                {
                    var patient = await this._patientRepository.Where(patient => patient.PatientId == id);
                    if (patient == null || patient.DoctorId != docId)
                    {
                        throw new GenericRequestException
                        {
                            Title = "Failed to load patient",
                            Error =
                                "Patient with id does not exist or the patient does not belong to this doctor account",
                            StatusCode = 400
                        };
                    }
                    return Ok(patient);
                }
                case "all":
                    var patients =
                        await this._patientRepository.WhereMulti(patient => patient.DoctorId == docId);
                    return Ok(patients);
                default:
                    throw new GenericRequestException
                    {
                        Title = "Failed to load patient",
                        Error = "Invalid request type",
                        StatusCode = 400
                    };
            }
        }

        [Authorize(Policy = "AssistantRequirement")]
        [Produces("application/json")]
        [Route("api/patient")]
        [HttpPost]
        public async Task<ActionResult> AddPatient([FromBody] Patient patient)
        {
            var accountType = (Roles)HttpContext.Items["AccountType"];
            
            if (accountType == Roles.Assistant)
            {
                var assistant =
                    await this._assistantRepository.Where(assistant => assistant.Id == (long) HttpContext.Items["Id"]);

                patient.DoctorId = assistant.DoctorId;
            }
            else
                patient.DoctorId = (long)HttpContext.Items["Id"];

            var isSSNUnique = await this._patientRepository.Where(entity => entity.SSN == patient.SSN) == null;
            if (!isSSNUnique)
            {
                throw new GenericRequestException
                {
                    Title = "Failed to add patient",
                    Error = "SSN must be unique",
                    StatusCode = 400
                };
            }

            if(!await this._patientRepository.Save(patient))
                throw new GenericRequestException
                {
                    Title = "Failed to add patient",
                    Error = "Save failed",
                    StatusCode = 400
                };

            return Ok(patient);
        }

        [Authorize(Policy = "DoctorRequirement")]
        [Produces("application/json")]
        [Route("api/patient")]
        [HttpPatch]
        public async Task<ActionResult> UpdateData([FromBody] Patient patient)
        {
            var check = await this._patientRepository.Where(entity => entity.PatientId == patient.PatientId);
            if(check == null)
                throw new GenericRequestException
                {
                    Title = "Failed to update patient data",
                    Error = "Patient does not exist",
                    StatusCode = 400
                };
            
            if(check.DoctorId != (long)HttpContext.Items["Id"])
                throw new GenericRequestException
                {
                    Title = "Failed to update patient data",
                    Error = "Patient belongs to a different doctor",
                    StatusCode = 400
                };

            if(!await this._patientRepository.UpdateChangedProperties(patient)) 
                throw new GenericRequestException
                {
                    Title = "Failed to update patient data",
                    Error = "No changes were made",
                    StatusCode = 400
                };

            return Ok();
        }

        [Authorize(Policy = "DoctorRequirement")]
        [Produces("application/json")]
        [Route("api/patient")]
        [HttpDelete]
        public async Task<ActionResult> DeletePatient([FromQuery(Name = "id")] long id)
        {
            var check = await this._patientRepository.Where(entity => entity.PatientId == id);
            if(check == null)
                throw new GenericRequestException
                {
                    Title = "Failed to delete patient data",
                    Error = "Patient does not exist",
                    StatusCode = 400
                };
            
            if(check.DoctorId != (long)HttpContext.Items["Id"])
                throw new GenericRequestException
                {
                    Title = "Failed to delete patient data",
                    Error = "Patient belongs to a different doctor",
                    StatusCode = 400
                };
            
            int count = await this._patientRepository.DeleteWhere(patient => patient.PatientId == id);

            if (count == 0)
            {
                throw new GenericRequestException
                {
                    Title = "Failed to delete patient data",
                    Error = "Patient does not exist",
                    StatusCode = 400
                };
            }

            return Ok();
        }

        [Authorize(Policy = "DoctorRequirement")]
        [Produces("application/json")]
        [Route("api/patient/diagnosis")]
        [HttpPost]
        public async Task<ActionResult> AddDiagnosis([FromQuery(Name = "id")] long id,[FromBody] Diagnosis diagnosis)
        {
            var check = await this._patientRepository.Where(entity => entity.PatientId == id);

            if(check == null)
                throw new GenericRequestException
                {
                    Title = "Failed to create a new diagnosis",
                    Error = "Patient does not exist",
                    StatusCode = 400
                };
            
            if(check.DoctorId != (long)HttpContext.Items["Id"])
                throw new GenericRequestException
                {
                    Title = "Failed to create a new diagnosis",
                    Error = "Patient belongs to a different doctor",
                    StatusCode = 400
                };

            diagnosis.Date = DateTime.Now;
            
            check.Diagnoses.Add(diagnosis);

            if (!await this._patientRepository.Update(check))
            {
                throw new GenericRequestException
                {
                    Title = "Failed to delete patient data",
                    Error = "No changes were made",
                    StatusCode = 400
                };
            }
            
            return Ok(diagnosis);
        }
        
        [Authorize(Policy = "DoctorRequirement")]
        [Produces("application/json")]
        [Route("api/patient/diagnosis")]
        [HttpDelete]
        public async Task<ActionResult> DeleteDiagnosis([FromQuery(Name = "patientId")] long patientId,[FromQuery(Name = "diagnosisId")] long diagnosisId)
        {
       
            var check = await this._patientRepository.Where(entity => entity.PatientId == patientId);

            if(check == null)
                throw new GenericRequestException
                {
                    Title = "Failed to delete the diagnosis",
                    Error = "Patient does not exist",
                    StatusCode = 400
                };
            
            if(check.DoctorId != (long)HttpContext.Items["Id"])
                throw new GenericRequestException
                {
                    Title = "Failed to delete the diagnosis",
                    Error = "Patient belongs to a different doctor",
                    StatusCode = 400
                };


            

            if (await _diagonisRepository.DeleteWhere(diagnosis => diagnosis.Id == diagnosisId) == 0)
            {
                throw new GenericRequestException
                {
                    Title = "Failed to delete the diagnosis",
                    Error = "No changes were made",
                    StatusCode = 400
                };
            }
            
            return Ok();
        }

        [Authorize(Policy = "AssistantRequirement")]
        [Produces("text/plain")]
        [Route("api/patient/updates")]
        [HttpGet]
        public async Task<ActionResult> CheckForUpdates([FromQuery] string hash)
        {
            var accountType = (Roles)HttpContext.Items["AccountType"];
            long docId;
            
            if (accountType == Roles.Assistant)
            {
                var assistant =
                    await this._assistantRepository.Where(assistant => assistant.Id == (long) HttpContext.Items["Id"]);

                docId = assistant.DoctorId;
            }
            else
                docId = (long)HttpContext.Items["Id"];

            using var sha = new SHA1Managed();
            
            var patients = await _patientRepository.WhereMulti(patient => patient.DoctorId == docId); 
            
            var computed = Convert.ToHexString(Security.ComputeHash(JArray.FromObject(patients).ToString(Formatting.None)));

            if (computed.Equals(hash))
                return StatusCode(200);
            else
                return StatusCode(205);
        }
    }
}