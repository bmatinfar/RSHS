using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Data.DataObjects
{
    /// <summary>
    /// Treatment machine has a name and a capability
    /// </summary>
    [Serializable]
    [DataContract]
    public class TreatmentMachine
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        [JsonConverter(typeof(StringEnumConverter))]
        public MachineCapabilityEnum Capability { get; set; }

        public override string ToString()
        {
            return string.Format("{0}, Capability: {1}", Name, Capability);
        }
    }

    
}
