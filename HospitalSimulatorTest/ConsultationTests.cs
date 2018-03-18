using System;
using System.Diagnostics;
using System.Linq;
using HospitalSimulatorClient.HospitalWebClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Data.DataObjects;

namespace HospitalSimulatorTest
{
    [TestClass]
    public class ConsultationTests
    {
        private static readonly HospitalWebClient _service = new HospitalWebClient();
        private static Process _iisProcess;

        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            _iisProcess = Process.Start("C:\\Program Files (x86)\\IIS Express\\iisexpress.exe", " /site:HospitalSimulator");

            if (_service.GetConsultations().Length > 0) return;
            _service.RegisterPatient(new Patient("TestPat1", PatientCondition.Flu));
            _service.RegisterPatient(new Patient("TestPat2", PatientCondition.Flu));
            _service.RegisterPatient(new Patient("TestPat3", PatientCondition.Flu));
        }

        [ClassCleanup]
        public static void MyClassCleanup()
        {
            _iisProcess.Kill();
            _iisProcess.WaitForExit();
        }

        [TestMethod]
        public void TestConsultationsForFluPatient()
        {
            var patient = new Patient("PatBG", PatientCondition.Flu);

            _service.RegisterPatient(patient);

            var consultations = _service.GetConsultations();
            var found = Array.FindAll(consultations, c => c.Patient.Name == patient.Name && c.Patient.Condition == patient.Condition);

            foreach (var consultation in found)
            {
                Assert.IsTrue(consultation.Doctor.Role.Contains(DoctorRolesEnum.GeneralPractioner));
                Assert.IsNotNull(consultation.TreatmentRoom);
                Assert.IsNull(consultation.TreatmentRoom.TreatmentMachine);
            }
        }

        [TestMethod]
        public void TestConsultationsForBreastPatient()
        {
            var patient = new Patient("PatA", PatientCondition.Breast);
            _service.RegisterPatient(patient);

            var consultations = _service.GetConsultations();
            var found = Array.FindAll(consultations, c => c.Patient.Name == patient.Name && c.Patient.Condition == patient.Condition);

            foreach (var consultation in found)
            {
                Assert.IsTrue(consultation.Doctor.Role.Contains(DoctorRolesEnum.Oncologist));
                Assert.IsNotNull(consultation.TreatmentRoom);
                Assert.IsNotNull(consultation.TreatmentRoom.TreatmentMachine);
            }
        }

        [TestMethod]
        public void TestConsultationsForHeadAndNeckPatient()
        {
            var patient = new Patient("PatA", PatientCondition.HeadAndNeck);
            _service.RegisterPatient(patient);

            var consultations = _service.GetConsultations();
            var found = Array.FindAll(consultations, c => c.Patient.Name == patient.Name && c.Patient.Condition == patient.Condition);

            foreach (var consultation in found)
            {
                Assert.IsTrue(consultation.Doctor.Role.Contains(DoctorRolesEnum.Oncologist));
                Assert.IsNotNull(consultation.TreatmentRoom);
                Assert.AreEqual(consultation.TreatmentRoom.TreatmentMachine.Capability, MachineCapabilityEnum.Advanced);
            }
        }

        [TestMethod]
        public void TestUniqueConsultationDates()
        {
            if (_service.GetConsultations().Length <= 0)
            {
                _service.RegisterPatient(new Patient("TestPat1", PatientCondition.Flu));
                _service.RegisterPatient(new Patient("TestPat2", PatientCondition.Flu));
                _service.RegisterPatient(new Patient("TestPat3", PatientCondition.Flu));
            }
            var consultations = _service.GetConsultations();
            var cDate = consultations[0].ConsultationDate;
            for (var i = 1; i < consultations.Length; i++)
            {
                Assert.IsFalse(DateTime.Compare(cDate, consultations[i].ConsultationDate) == 0);
                cDate = consultations[i].ConsultationDate;
            }
        }

        [TestMethod]
        public void TestConsultationAndRegistrationDateAreDifferent()
        {
            var consultations = _service.GetConsultations();
            foreach (var consultation in consultations)
            {
                Assert.AreNotEqual(consultation.ConsultationDate, consultation.RegistraionDate);
            }
        }

        [TestMethod]
        public void TestLastConsultationDate()
        {
            var lastConsultationDate = _service.GetConsultations().Max(c => c.ConsultationDate);
            var patient = new Patient("TestPatinet4", PatientCondition.HeadAndNeck);
            _service.RegisterPatient(patient);
            var consultations = _service.GetConsultations();
            var found = Array.FindAll(consultations, c => c.Patient.Name == patient.Name && c.Patient.Condition == patient.Condition);
            
            Assert.IsTrue(found.Last().ConsultationDate == lastConsultationDate.AddDays(1));
        }
    }
}
