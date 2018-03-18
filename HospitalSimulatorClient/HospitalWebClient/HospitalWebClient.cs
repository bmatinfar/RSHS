using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using Data.DataObjects;

namespace HospitalSimulatorClient.HospitalWebClient
{
    public class HospitalWebClient
    {

        public HospitalWebClient()
        {            
        }

        /// <summary>
        /// Helper method to send data to server
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endpoint"></param>
        /// <param name="method"></param>
        /// <param name="patient"></param>
        /// <returns></returns>
        private static T SendDataToServer<T>(string endpoint, string method, T patient)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(endpoint);
            request.Accept = "application/json";
            request.ContentType = "application/json";
            request.Method = method;

            var serializer = new DataContractJsonSerializer(typeof(T));
            var requestStream = request.GetRequestStream();
            serializer.WriteObject(requestStream, patient);
            requestStream.Close();

            var response = request.GetResponse();
            if (response.ContentLength == 0)
            {
                response.Close();
                return default(T);
            }
            var responseStream = response.GetResponseStream();
            var responseObject = (T)serializer.ReadObject(responseStream);
            responseStream.Close();
            return responseObject;
        }

        public Patient[] GetRegisteredPatients()
        {
            var client = new WebClient();
            //Accept header to indicate client is requesting json format
            client.Headers.Add("Accept", "application/json");
            // Receive a json string and deserialize it to a binary stream of data
            var result = client.DownloadString("http://localhost:54251/HospitalService.svc/Patients");
            var serializer = new DataContractJsonSerializer(typeof(Patient[]));
            Patient[] resultPatients;
            //Using the byte array, read the data stream and cast it to Patient object
            using (var stream = new MemoryStream(Encoding.ASCII.GetBytes(result)))
            {
                resultPatients = (Patient[])(serializer.ReadObject(stream));
            }

            return resultPatients;
        }

        public Consultation[] GetConsultations()
        {
            var client = new WebClient();
            client.Headers.Add("Accept", "application/json");
            var result = client.DownloadString("http://localhost:54251/HospitalService.svc/Consultations");
            var serializer = new DataContractJsonSerializer(typeof(Consultation[]));
            Consultation[] resultConsultations;
            //Using the byte array, read the data stream and cast it to Patient object
            using (var stream = new MemoryStream(Encoding.ASCII.GetBytes(result)))
            {
                resultConsultations = (Consultation[])(serializer.ReadObject(stream));
            }

            return resultConsultations;
        }

        public Patient RegisterPatient(Patient patient)
        {
            return SendDataToServer("http://localhost:54251/HospitalService.svc/Patients", "POST", patient);
        }

        public Patient UpdatePatient(Patient patient)
        {
            return SendDataToServer("http://localhost:54251/HospitalService.svc/Patient/" + patient.Name, "PUT", patient);
        }
    }
}
