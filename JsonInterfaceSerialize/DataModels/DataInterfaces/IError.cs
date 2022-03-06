using JsonInterfaceSerialize.Utilities.Enums;

namespace JsonInterfaceSerialize.DataModels.DataInterfaces
{
    public interface IError
    {
        string Message { get; set; }
        ErrorTypes Type { get; set; }
    }
}
