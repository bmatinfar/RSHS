using System;
using System.Collections.Generic;
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
    public class Doctor
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        [JsonConverter(typeof(StringEnumConverter))]
        public List<DoctorRolesEnum> Role { get; set; }

        public override string ToString()
        {
            return string.Format("Doctor: {0}, Role(s): {1}", Name, string.Join(", ", Role.ToArray()));
        }
    }
}
