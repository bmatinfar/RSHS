using System;
using System.Runtime.Serialization;

namespace Data.DataObjects
{
    /// <summary>
    /// Consultation contains a patient, a doctor, a registration data, a consultation date, and a treatment room
    /// </summary>
    [Serializable]
    [DataContract]
    public class Consultation
    {
        [DataMember]
        public Patient Patient { get; set; }

        [DataMember]
        public Doctor Doctor { get; set; }

        [DataMember]
        public DateTime RegistraionDate { get; set; }

        [DataMember]
        public DateTime ConsultationDate { get; set; }

        [DataMember]
        public TreatmentRoom TreatmentRoom { get; set; }

        public override string ToString()
        {
            return string.Format(
                "{0}, {1}, Registration date: {2}, Consultation date: {3}, {4}",
                Patient, Doctor, RegistraionDate, ConsultationDate, TreatmentRoom);
        }
    }
}
