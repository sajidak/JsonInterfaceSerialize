using JsonInterfaceSerialize.DataModels.DataInterfaces;
using JsonInterfaceSerialize.Utilities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JsonInterfaceSerialize.DataModels.Containers
{
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

        readonly DateTimeOffset StartTime = DateTimeOffset.UtcNow;

        public string CorrelationId { get; set; }

        public System.Net.HttpStatusCode StatusCode { get; set; } = System.Net.HttpStatusCode.InternalServerError;

        public bool Successful { get; set; } = false;

        public long RequestDurationInMilliseconds { get => (long)(DateTimeOffset.UtcNow - StartTime).TotalMilliseconds; }

        public DateTimeOffset TimestampUtc { get; set; }

        public T Data { get; set; }
        public string Message { get; set; }

        public IList<IError> Errors { get; set; } = new List<IError>();
        public bool HaveErrors { get => Errors.Where(e => e.Type == ErrorTypes.ERROR).Count() > 0; }

        public int IngestErrors(IList<IError> errs)
        {
            if (errs is null || errs.Count == 0) return 0;
            (Errors as List<IError>).AddRange(errs);
            return errs.Where(v => v.Type == ErrorTypes.ERROR).Count();
        }

        public IList<IValidationError> ValidationErrors { get; set; } = new List<IValidationError>();
        public bool HaveValidationErrors { get => ValidationErrors.Where(v => v.Type == ErrorTypes.ERROR).Count() > 0; }
        public int IngestValidationErrors(IList<IValidationError> errs)
        {
            if (errs is null || errs.Count == 0) return 0;
            (Errors as List<IValidationError>).AddRange(errs);
            return errs.Where(v => v.Type == ErrorTypes.ERROR).Count();
        }
    }
}
