using System;
using System.Collections.Generic;
using System.Linq;

namespace JsonInterfaceSerialize.DataModels.ModelsV4.Containers
{
    public interface IResultObject<T>
    {
        string CorrelationId { get; set; }
        T Data { get; set; }
        IList<IError> Errors { get; set; }
        bool HaveErrors { get; }
        string Message { get; set; }
        long RequestDurationInMilliseconds { get; }
        System.Net.HttpStatusCode StatusCode { get; set; }
        bool Successful { get; set; }
        DateTimeOffset TimestampUtc { get; set; }
    }

    // TODO: Refactor and Cleanup into seperate namespaces and files

    public class ResultObject<T> : IResultObject<T>
    {
        public ResultObject()
        {
            this.TimestampUtc = DateTimeOffset.UtcNow;
        }
        public ResultObject(string CorrelationId) : this()
        {
            this.CorrelationId = CorrelationId;
        }

        DateTimeOffset StartTime = DateTimeOffset.UtcNow;

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

        public IList<IError> Errors { get; set; } = new List<IError>();
        public bool HaveErrors { get => Errors.Where(e => e.Type == ErrorTypes.ERROR).Count() > 0; }

    }

    public interface IError
    {
        string Message { get; set; }
        ErrorTypes Type { get; set; }
    }

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

    public enum ErrorTypes : int
    {
        NOTSET = 0,
        INFO = 1,
        WARN = 2,
        ERROR = 3,
    }

}
