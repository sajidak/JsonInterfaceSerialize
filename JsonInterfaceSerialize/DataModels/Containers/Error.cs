using JsonInterfaceSerialize.DataModels.DataInterfaces;
using JsonInterfaceSerialize.Utilities.Enums;
using System;

namespace JsonInterfaceSerialize.DataModels.Containers
{
    public class Error : IError
    {
        public Error() { }
        public Error(ErrorTypes type, string error_mesg)
        {
            Type = type;
            Message = error_mesg ?? throw new ArgumentNullException(nameof(error_mesg));
        }
        public ErrorTypes Type { get; set; }
        public string Message { get; set; }
    }
}
