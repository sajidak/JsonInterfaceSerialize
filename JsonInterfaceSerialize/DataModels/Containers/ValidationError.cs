using JsonInterfaceSerialize.DataModels.DataInterfaces;
using JsonInterfaceSerialize.Utilities.Enums;
using System;

namespace JsonInterfaceSerialize.DataModels.Containers
{
    public class ValidationError : IValidationError
    {
        public ValidationError() { }
        public ValidationError(string field, ErrorTypes type, string error_mesg)
        {
            Field = field ?? throw new ArgumentNullException(nameof(field));
            Type = type;
            Message = error_mesg ?? throw new ArgumentNullException(nameof(error_mesg));
        }
        public string Field { get; set; }
        public ErrorTypes Type { get; set; }
        public string Message { get; set; }
    }
}
