using System;
using System.Collections.Generic;

namespace JsonInterfaceSerialize.DataModels.DataInterfaces
{
    public interface IResultObject<T>
    {
        string CorrelationId { get; set; }
        T Data { get; set; }
        IList<IError> Errors { get; set; }
        bool HaveErrors { get; }
        IList<IValidationError> ValidationErrors { get; set; }
        bool HaveValidationErrors { get; }

        string Message { get; set; }
        long RequestDurationInMilliseconds { get; }
        System.Net.HttpStatusCode StatusCode { get; set; }
        bool Successful { get; set; }
        DateTimeOffset TimestampUtc { get; set; }

        int IngestErrors(IList<IError> errs);
        int IngestValidationErrors(IList<IValidationError> verrs);
    }
}
