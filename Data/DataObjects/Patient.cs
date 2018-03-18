using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Data.DataObjects
{
    /// <summary>
    /// Patient has a name and a condition
    /// </summary>
    [Serializable]
    [DataContract]
    public class Patient
    {
        public bool Registered { get; set; }

        [DataMember]
        public string  Name { get; set; }

        [DataMember]
        [JsonConverter(typeof(StringEnumConverter))]
        public PatientCondition Condition { get; set; }

        public Patient(string name, PatientCondition patientCondition)
        {
            Name = name;
            Condition = patientCondition;
            Registered = false;
        }

        public override string ToString()
        {
            return string.Format("Patient: {0}, Patient Condition: {1}", Name, Condition);
        }
    }
}
