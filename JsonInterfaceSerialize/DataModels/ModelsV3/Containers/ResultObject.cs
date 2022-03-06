using System;
using System.Collections.Generic;
using System.Linq;

namespace JsonInterfaceSerialize.DataModels.ModelsV3.Containers
{
    // TODO: Refactor and Cleanup into seperate namespaces and files

    public class ResultObject<T>
    {
        public ResultObject()
        {
            this.TimestampUtc = DateTimeOffset.UtcNow;
        }
        public ResultObject(string CorrelationId) : this()
        {
            this.CorrelationId = CorrelationId;
        }

        readonly DateTimeOffset StartTime = DateTimeOffset.UtcNow;

        public string CorrelationId { get; set; }

        public System.Net.HttpStatusCode StatusCode { get; set; } = System.Net.HttpStatusCode.InternalServerError;

        public bool Successful { get; set; } = false;

        public long RequestDurationInMilliseconds { get => (long)(DateTimeOffset.UtcNow - StartTime).TotalMilliseconds; }

        public DateTimeOffset TimestampUtc
        {
            get; set;
        }

        public T Data { get; set; }
        public string Message { get; set; }

        public IList<Error> Errors { get; set; } = new List<Error>();
        public bool HaveErrors { get => Errors.Where(e => e.Type == ErrorTypes.ERROR).Count() > 0; }

    }


    public class Error
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

    public enum ErrorTypes : int
    {
        NOTSET = 0,
        INFO = 1,
        WARN = 2,
        ERROR = 3,
    }

}
