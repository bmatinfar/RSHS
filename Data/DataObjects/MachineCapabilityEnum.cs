using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Data.DataObjects
{
    /// <summary>
    /// A machine can be advanced to treat head and neck cancer or simple to treat breast cancer
    /// </summary>
    [Serializable]
    [DataContract]
    public enum MachineCapabilityEnum
    {
        [EnumMember] // [XmlEnum("1")]
        [Description("Advanced")]
        Advanced,

        [EnumMember]
        [Description("Simple")]
        Simple
    }
}
