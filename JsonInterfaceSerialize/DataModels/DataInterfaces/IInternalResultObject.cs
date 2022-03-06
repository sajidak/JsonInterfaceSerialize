using System.Collections.Generic;

namespace JsonInterfaceSerialize.DataModels.DataInterfaces
{
    public interface IInternalResultObject<T>
    {
        IList<IError> Errors { get; set; }
        bool HaveErrors { get; }
        string Message { get; set; }
        int RecordsAffected { get; set; }
        T Result { get; set; }

        int IngestErrors(IList<IError> errs);
    }
}
