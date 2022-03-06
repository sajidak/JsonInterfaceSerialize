using JsonInterfaceSerialize.Utilities.Enums;

namespace JsonInterfaceSerialize.DataModels.DataInterfaces
{
    public interface IValidationError
    {
        string Field { get; set; }
        string Message { get; set; }
        ErrorTypes Type { get; set; }
    }
}
