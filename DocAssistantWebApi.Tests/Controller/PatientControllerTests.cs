using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DocAssistant_Common.Models;
using DocAssistantWebApi.Controllers;
using DocAssistantWebApi.Database.Repositories;
using DocAssistantWebApi.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DocAssistantWebApi.Tests
{
    [TestClass]
    public class PatientControllerTests
    {

        private PatientController CreatePatientController(IRepository<Patient> mockPatientRepository, IRepository<Assistant> mockAssistantRepository, IRepository<Diagnosis> mockDiagnosisRepository)
        {
            var patientController = new PatientController(mockPatientRepository,mockAssistantRepository,mockDiagnosisRepository);
            patientController.ControllerContext = new Mock<ControllerContext>().Object;
            patientController.ControllerContext.HttpContext = new DefaultHttpContext();
            patientController.HttpContext.Items = new Dictionary<object, object?>
            {
                {"Id", 1L},
                {"AccountType",Filters.Roles.Doctor}
            };

            return patientController;
        }
        
        [TestMethod]
        public async Task LoadPatient_ShouldLoadSingleEntity()
        {
            // Arrange
            var type = "single";

            var patient = new Patient();
            patient.Id = 1L;
            patient.DoctorId = 1L;

            var mockPatientRepository = new Mock<IRepository<Patient>>();
            mockPatientRepository.Setup(mock => mock.Where(It.IsAny<Expression<Func<Patient,bool>>>()))
                .ReturnsAsync(patient);

            var mockAssistantRepository = new Mock<IRepository<Assistant>>();

            var mockDiagnosisRepository = new Mock<IRepository<Diagnosis>>();
            
            var patientController =
                CreatePatientController(mockPatientRepository.Object, mockAssistantRepository.Object,mockDiagnosisRepository.Object);

            // Act
            var result = await patientController.LoadPatient(type, patient.Id);
            
            // Assert
            
            var objectResult = result as OkObjectResult;
            
            Assert.IsNotNull(objectResult);

            var data = objectResult.Value as Patient;
            
            Assert.IsNotNull(data);
            Assert.AreEqual(patient, data);
        }
        
        [TestMethod]
        public async Task LoadPatient_ShouldLoadAllEntity()
        {
            // Arrange
            var type = "all";

            var mockPatientRepository = new Mock<IRepository<Patient>>();
            mockPatientRepository.Setup(mock => mock.WhereMulti(It.IsAny<Expression<Func<Patient,bool>>>()))
                .ReturnsAsync(new List<Patient>());
            
            var mockAssistantRepository = new Mock<IRepository<Assistant>>();

            var mockDiagnosisRepository = new Mock<IRepository<Diagnosis>>();
            
            var patientController =
                CreatePatientController(mockPatientRepository.Object, mockAssistantRepository.Object,mockDiagnosisRepository.Object);

            // Act
            var result = await patientController.LoadPatient(type, null);
            
            // Assert
            var objectResult = result as OkObjectResult;
            
            Assert.IsNotNull(objectResult);

            var data = objectResult.Value as IEnumerable<Patient>;
            
            Assert.IsNotNull(data);
        }
        
        
        [TestMethod]
        public async Task AddPatient_ShouldAddEntity()
        {
            // Arrange
            var patient = new Patient
            {
                Id = 1,
                DoctorId = 1,
                SSN = "000-000-000"
            };
            
            var mockPatientRepository = new Mock<IRepository<Patient>>();
            mockPatientRepository.Setup(mock => mock.Where(It.IsAny<Expression<Func<Patient,bool>>>()))
                .ReturnsAsync((Patient) null);
            mockPatientRepository.Setup(mock => mock.Save(It.IsAny<Patient>()))
                .ReturnsAsync(true);
            
            var mockAssistantRepository = new Mock<IRepository<Assistant>>();
            
            var mockDiagnosisRepository = new Mock<IRepository<Diagnosis>>();
            
            var patientController =
                CreatePatientController(mockPatientRepository.Object, mockAssistantRepository.Object,mockDiagnosisRepository.Object);
            // Act
            var result = await patientController.AddPatient(patient);
            // Assert
            var objectResult = result as OkObjectResult;
            
            Assert.IsNotNull(objectResult);
        }
        
        
        [TestMethod]
        public async Task AddPatient_ShouldNotAddEntity()
        {
            // Arrange
            var patient = new Patient
            {
                Id = 1,
                DoctorId = 1,
                SSN = "000-000-000"
            };
            
            var mockPatientRepository = new Mock<IRepository<Patient>>();
            mockPatientRepository.Setup(mock => mock.Where(It.IsAny<Expression<Func<Patient,bool>>>()))
                .ReturnsAsync(patient);
            mockPatientRepository.Setup(mock => mock.Save(It.IsAny<Patient>()))
                .ReturnsAsync(true);
            
            var mockAssistantRepository = new Mock<IRepository<Assistant>>();
            
            var mockDiagnosisRepository = new Mock<IRepository<Diagnosis>>();
            
            var patientController =
                CreatePatientController(mockPatientRepository.Object, mockAssistantRepository.Object,mockDiagnosisRepository.Object);
            // Assert
            await Assert.ThrowsExceptionAsync<GenericRequestException>(() => patientController.AddPatient(patient));
        }

        [TestMethod]
        public async Task UpdateData_ShouldUpdateEntity()
        {
            // Arrange
            var patient = new Patient
            {
                Id = 1,
                DoctorId = 1,
                SSN = "000-000-000"
            };
            
            var mockPatientRepository = new Mock<IRepository<Patient>>();
            mockPatientRepository.Setup(mock => mock.Where(It.IsAny<Expression<Func<Patient,bool>>>()))
                .ReturnsAsync(patient);
            
            mockPatientRepository.Setup(mock => mock.UpdateChangedProperties(It.IsAny<Patient>(), null))
                .ReturnsAsync(true);
            
            var mockAssistantRepository = new Mock<IRepository<Assistant>>();
            
            var mockDiagnosisRepository = new Mock<IRepository<Diagnosis>>();
            
            var patientController =
                CreatePatientController(mockPatientRepository.Object, mockAssistantRepository.Object,mockDiagnosisRepository.Object);
            // Act
            var result = await patientController.UpdateData(patient);
            
            // Assert
            var objectResult = result as OkResult;
            
            Assert.IsNotNull(objectResult);
        }

        [TestMethod]
        public async Task DeletePatient_ShouldDeleteEntity()
        {
            // Arrange
            var patient = new Patient
            {
                Id = 1,
                DoctorId = 1
            };
            
            var mockPatientRepository = new Mock<IRepository<Patient>>();
            mockPatientRepository.Setup(mock => mock.Where(It.IsAny<Expression<Func<Patient,bool>>>()))
                .ReturnsAsync(patient);
            mockPatientRepository.Setup(mock => mock.DeleteWhere(It.IsAny<Expression<Func<Patient,bool>>>()))
                .ReturnsAsync(1);
            
            var mockAssistantRepository = new Mock<IRepository<Assistant>>();
            
            var mockDiagnosisRepository = new Mock<IRepository<Diagnosis>>();
            
            var patientController =
                CreatePatientController(mockPatientRepository.Object, mockAssistantRepository.Object,mockDiagnosisRepository.Object);
            // Act
            var result = await patientController.DeletePatient(patient.Id);
            // Assert
            var objectResult = result as OkResult;
            
            Assert.IsNotNull(objectResult);
        }
        
        [TestMethod]
        public async Task AddDiagnosis_ShouldUpdatePatientData()
        {
            // Arrange
            var patient = new Patient
            {
                Id = 1,
                DoctorId = 1
            };

            var diagnosis = new Diagnosis();
            
            var mockPatientRepository = new Mock<IRepository<Patient>>();
            mockPatientRepository.Setup(mock => mock.Where(It.IsAny<Expression<Func<Patient,bool>>>()))
                .ReturnsAsync(patient);
            mockPatientRepository.Setup(mock => mock.Update(It.IsAny<Patient>()))
                .ReturnsAsync(true);
            
            var mockAssistantRepository = new Mock<IRepository<Assistant>>();
            
            var mockDiagnosisRepository = new Mock<IRepository<Diagnosis>>();
            
            var patientController =
                CreatePatientController(mockPatientRepository.Object, mockAssistantRepository.Object,mockDiagnosisRepository.Object);
            // Act
            
            var result = await patientController.AddDiagnosis(patient.Id,diagnosis);
            
            // Assert
            var objectResult = result as OkObjectResult;
            
            Assert.IsNotNull(objectResult);
        }
    }
}