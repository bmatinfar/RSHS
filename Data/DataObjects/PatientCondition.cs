using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Data.DataObjects
{
    /// <summary>
    /// Patient condition could be cancer or flu. Cancer could be head and neck or breast
    /// To simplify the implementation, instead having a type and a topology, the above three conditions are modeled by an enum 
    /// </summary>
    [Serializable]
    [DataContract(Name = "PatientCondition")]
    public enum PatientCondition
    {
        [EnumMember]
        [Description("Flu")]
        Flu,

        [EnumMember]
        [Description("HeadAndNeck")]
        HeadAndNeck,

        [EnumMember]
        [Description("Breast")]
        Breast
    }
}