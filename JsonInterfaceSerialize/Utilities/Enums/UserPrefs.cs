using System.Runtime.Serialization;

namespace JsonInterfaceSerialize.Utilities.Enums
{
    public enum SavedQuerySearchType
    {
        [EnumMember(Value = "Substance")]
        Substance,
        [EnumMember(Value = "Text")]
        Regulation,
        [EnumMember(Value = "Regulation")]
        Text
    }
}
