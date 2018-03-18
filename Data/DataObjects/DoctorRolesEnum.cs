using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Data.DataObjects
{
    /// <summary>
    /// Doctors can be general practitioners or oncologist
    /// </summary>
    [Serializable]
    [DataContract]
    public enum DoctorRolesEnum
    {
        [EnumMember]
        [Description("General Practitioner")]
        GeneralPractioner,

        [EnumMember]
        [Description("Oncologist")]
        Oncologist
    }
}
