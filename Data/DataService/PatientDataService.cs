using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Data.DataObjects;
using Newtonsoft.Json;

namespace Data.DataService
{
    /// <summary>
    /// The data service provider that implements:
    /// 1- Patient registration
    /// 2- List of registered patients
    /// 3- List of consultation
    /// </summary>
    public class PatientDataService
    {
        private List<Patient> Patients { get; set; }
        private List<Consultation> Consultations { get; set; }

        //Last date which a consultation has been scheduled. This date is unique and because a consultation takes a whole day, 
        //guarantees that resources are available for the next date.
        private DateTime _lastScheduledDate;

        private readonly List<Doctor> _doctors = new List<Doctor>();
        private readonly List<TreatmentMachine> _treatmentMachines = new List<TreatmentMachine>(); 
        private readonly List<TreatmentRoom> _treatmentRooms = new List<TreatmentRoom>();
        private readonly object _thisLock = new object();

        //Random function is used to create a uniform distribution of resources that satisfy a predicate. 
        private readonly Random _rnd = new Random();

        //folder to maintain data
        private readonly string _tempPath;
        
        public PatientDataService()
        {
            _tempPath = AppDomain.CurrentDomain.BaseDirectory;

            ParseResources();
            SeedData();
        }

        /// <summary>
        /// Method to register a new patient based on its condition
        /// </summary>
        /// <param name="patient"></param>
        public void PatientRegistration(Patient patient)
        {
            //Make sure two operators cannot call this method at the same time. Prevent double booking of the same resource
            lock (_thisLock)
            {
                if (patient.Registered)
                {
                    //this patient has already been registered
                    return;
                }

                Consultation consultation;
                patient.Registered = true;

                switch (patient.Condition)
                {
                    case PatientCondition.Flu:
                        consultation = new Consultation
                        {
                            //Assign next day of last scheduled date to new consultation
                            ConsultationDate = _lastScheduledDate.AddDays(1),
                            //Randomly select a general practitioner. 
                            Doctor = _doctors.FindAll(d => d.Role.Contains(DoctorRolesEnum.GeneralPractioner)).OrderBy(r => _rnd.Next()).Take(1).FirstOrDefault(),
                            Patient = patient,
                            RegistraionDate = DateTime.Now,
                            //Randomly select a treatment room that does not contain a machine. Free up the treatment rooms with machine for cancer patients
                            TreatmentRoom = _treatmentRooms.FindAll(tr => tr.TreatmentMachine == null).OrderBy(r => _rnd.Next()).Take(1).FirstOrDefault()
                        };
                        break;
                    case PatientCondition.Breast:
                        consultation = new Consultation
                        {
                            ConsultationDate = _lastScheduledDate.AddDays(1),
                            //Randomly select an oncologist.
                            Doctor = _doctors.FindAll(d => d.Role.Contains(DoctorRolesEnum.Oncologist)).OrderBy(r => _rnd.Next()).Take(1).FirstOrDefault(),
                            Patient = patient,
                            RegistraionDate = DateTime.Now,
                            //Select a room with a treatment machine. Can be Advanced or Simple
                            TreatmentRoom = _treatmentRooms.FindAll(tr => tr.TreatmentMachine != null).OrderBy(r => _rnd.Next()).Take(1).FirstOrDefault()
                        };
                        break;
                    case PatientCondition.HeadAndNeck:
                        consultation = new Consultation
                        {
                            ConsultationDate = _lastScheduledDate.AddDays(1),
                            Doctor = _doctors.FindAll(d => d.Role.Contains(DoctorRolesEnum.Oncologist)).OrderBy(r => _rnd.Next()).Take(1).FirstOrDefault(),
                            Patient = patient,
                            RegistraionDate = DateTime.Now,
                            //Randomly select an advanced treatment machine
                            TreatmentRoom = _treatmentRooms.FindAll(tr => tr.TreatmentMachine != null && tr.TreatmentMachine.Capability == MachineCapabilityEnum.Advanced).OrderBy(r => _rnd.Next()).Take(1).FirstOrDefault()
                        };
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                _lastScheduledDate = consultation.ConsultationDate;
                Consultations.Add(consultation);
                Patients.Add(patient);
            }    
            SaveData();
        }

        public IEnumerable<Patient> GetRegisteredPatients()
        {
           return Patients.Where(p => p.Registered);
        }

        public IEnumerable<Consultation> GetConsultations()
        {
            return Consultations;
        }

        public void DeletePatient(string patientName)
        {
            Patients.Remove(Patients.Find(p => p.Name == patientName));
            SaveData();
        }

        public Patient UpdatePatient(Patient patient)
        {
            var found = Patients.SingleOrDefault(p => p.Name == patient.Name && p.Registered);
            if (found == null) return patient;
            Patients.Remove(found);
            PatientRegistration(patient);
            return patient;
        }

        public Patient[] GetPatientsByName(string patientName)
        {
            var found = Patients.FindAll(p => p.Name == patientName && p.Registered);
            return found.ToArray();
        }

        private void ParseResources()
        {
            dynamic dynObj;
            using (StreamReader reader = new StreamReader(_tempPath + "\\resources.json"))
            {
                string jsonString = reader.ReadToEnd();
                dynObj = JsonConvert.DeserializeObject(jsonString);
            }

            foreach (var dc in dynObj.Doctors)
            {
                var roles = new List<DoctorRolesEnum>();
                foreach (var rl in dc.Roles)
                {
                    roles.Add(rl == "Oncologist" ? DoctorRolesEnum.Oncologist : DoctorRolesEnum.GeneralPractioner);
                }

                //var roles = dc.Roles.ToObject<List<DoctorRolesEnum>>();

                _doctors.Add(new Doctor
                {
                    Name = dc.Name,
                    Role = roles
                });

            }

            foreach (var tm in dynObj.TreatmentMachines)
            {
                _treatmentMachines.Add(new TreatmentMachine
                {
                    Name = tm.Name,
                    Capability = tm.Capability
                });
            }

            foreach (var tr in dynObj.TreatmentRooms)
            {
                if (tr.TreatmentMachine != null)
                {
                    _treatmentRooms.Add(new TreatmentRoom
                    {
                        Name = tr.Name,
                        TreatmentMachine =
                            _treatmentMachines.FirstOrDefault(tm => tm.Name == (string) tr.TreatmentMachine)
                    });
                }
                else
                {
                    _treatmentRooms.Add(new TreatmentRoom
                    {
                        Name = tr.Name,
                    });
                }
            }
        }
      
        //Create a dummy data layer to persist data
        private void SeedData()
        {
            if (File.Exists(_tempPath + "\\patients.dat") && File.Exists(_tempPath + "\\consultation.dat"))
            {
                var formatter = new BinaryFormatter();
                using (var stream = new FileStream(_tempPath + "\\patients.dat", FileMode.Open, FileAccess.Read,
                    FileShare.Read))
                {
                    Patients = (List<Patient>)formatter.Deserialize(stream);
                }

                using (var stream = new FileStream(_tempPath + "\\consultation.dat", FileMode.Open, FileAccess.Read,
                    FileShare.Read))
                {
                    Consultations = (List<Consultation>)formatter.Deserialize(stream);
                    _lastScheduledDate = Consultations.Max(c => c.ConsultationDate);
                }
            }
            else
            {
                Patients = new List<Patient>();
                Consultations = new List<Consultation>();
                _lastScheduledDate = DateTime.Now;

                var pA = new Patient("patA", PatientCondition.Flu);
                PatientRegistration(pA);
                var pB = new Patient("patB", PatientCondition.Flu);
                PatientRegistration(pB);
                var pC = new Patient("patC", PatientCondition.HeadAndNeck);
                PatientRegistration(pC);
                var pD = new Patient("patD", PatientCondition.Breast);
                PatientRegistration(pD);
                var pE = new Patient("patE", PatientCondition.HeadAndNeck);
                PatientRegistration(pE);
                var pF = new Patient("patF", PatientCondition.HeadAndNeck);
                PatientRegistration(pF);
                SaveData();
            }
        }

        private void SaveData()
        {
            var formatter = new BinaryFormatter();

            if (!Directory.Exists(_tempPath))
            {
                DirectoryInfo di = Directory.CreateDirectory(_tempPath);
            }
       
            using (var stream = new FileStream(_tempPath+"\\patients.dat", FileMode.Create, FileAccess.Write))
            {
                formatter.Serialize(stream, Patients);
            }

            using (var stream = new FileStream(_tempPath+"\\consultation.dat", FileMode.Create, FileAccess.Write))
            {
                formatter.Serialize(stream, Consultations);
            }
        }
    }

}
