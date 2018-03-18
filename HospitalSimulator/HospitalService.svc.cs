using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using Data.DataObjects;
using Data.DataService;

namespace HospitalSimulator
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class HospitalService
    {
        private readonly PatientDataService _service;

        public HospitalService()
        {
            _service = new PatientDataService();           
        }

        //Read
        [WebGet(UriTemplate = "/Patients")]
        public Patient[] GetRegisteredPatients()
        {
            return _service.GetRegisteredPatients().ToArray();
        }

        //Read
        [WebGet(UriTemplate = "/Consultations")]
        public Consultation[] GetConsultations()
        {
            return _service.GetConsultations().ToArray();
        }

        //Create
        [WebInvoke(UriTemplate = "/Patients")]
        public void RegisterPatient(Patient patient)
        {
            _service.PatientRegistration(patient);
        }

        //Delete
        [WebInvoke(Method = "DELETE", UriTemplate = "/Patient/{PatientName}")]
        public void DeletePatient(string patientName)
        {
            _service.DeletePatient(patientName);
        }

        //Update
        [WebInvoke(Method = "PUT", UriTemplate = "/Patient/{PatientName}")]
        public Patient UpdatePatient(string patientName, Patient patient)
        {
            return _service.UpdatePatient(patient);
        }

        //Read
        [WebGet(UriTemplate = "/Patient/{patientName}")]
        public Patient[] GetatientsByName(string patientName)
        {
            return _service.GetPatientsByName(patientName);
        }

    }
}
