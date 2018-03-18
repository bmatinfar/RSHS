using System;
using System.Linq;
using Data.DataObjects;

namespace HospitalSimulatorClient
{
    /// <summary>
    /// Simple console application that calls Hospital WebClient to communicate with Hospital WebService
    /// </summary>
    class Program
    {
        private static HospitalWebClient.HospitalWebClient _client;
        static void Main(string[] args)
        {
            _client = new HospitalWebClient.HospitalWebClient();

            ListConsultations();
            RegisterPatient();

            Console.WriteLine("\nWould you like to register a patient (Y/N)?");
            var response = Console.ReadLine();
            if (string.Equals(response.ToUpper(), "Y"))
                EnterPatient();
            else
                Console.WriteLine("Enter a key to exit");
                
            Console.Read();
        }

        private static void ListConsultations()
        {
            var consultations = _client.GetConsultations();

            Console.WriteLine("List of consultations:\n");
            foreach (var consultation in consultations)
            {
                Console.WriteLine(consultation);
                Console.WriteLine("---------------");

            }
            Console.WriteLine("*******************");
        }


        private static void ListPatients()
        {
            var patients = _client.GetRegisteredPatients();
            foreach (var patient in patients)
            {
                Console.WriteLine(patient);
            }
            Console.WriteLine("---------------");
        }

        private static void RegisterPatient()
        {
            var patient = new Patient("John", PatientCondition.HeadAndNeck);
            _client.RegisterPatient(patient);

            ListPatients();
        }

        private static void EnterPatient()
        {
            Console.Write("Name of patient? ");
            var name = Console.ReadLine();
            Console.Write("Condition? (F)lu, (B)reast Cancer, (H)eadAndNeck Cancer?");
            var condition = Console.ReadLine();
            PatientCondition patientCondition = PatientCondition.Flu;
            switch (condition.ToUpper())
            {
                case "B":
                    patientCondition = PatientCondition.Breast;
                    break;
                case "H":
                    patientCondition = PatientCondition.HeadAndNeck;
                    break;
            }

            var pateint = new Patient(name, patientCondition);          
            _client.RegisterPatient(pateint);
            ListPatients();
        }

    }
}
