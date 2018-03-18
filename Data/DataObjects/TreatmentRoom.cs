using System;
using System.Runtime.Serialization;

namespace Data.DataObjects
{
    /// <summary>
    /// Treatment room has a name and a teatment machine
    /// </summary>
    [Serializable]
    [DataContract]
    public class TreatmentRoom
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public TreatmentMachine TreatmentMachine { get; set; }

        public override string ToString()
        {
            return string.Format("Treatment Room: {0}, Treatment Machine: {1}", Name, TreatmentMachine);
        }
    }
}
