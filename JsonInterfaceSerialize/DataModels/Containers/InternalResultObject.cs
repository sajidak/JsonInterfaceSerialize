using JsonInterfaceSerialize.DataModels.DataInterfaces;
using JsonInterfaceSerialize.Utilities.Constants;
using JsonInterfaceSerialize.Utilities.Enums;
using System.Collections.Generic;
using System.Linq;

namespace JsonInterfaceSerialize.DataModels.Containers
{
    public class InternalResultObject<T> : IInternalResultObject<T>
    {
        public T Result { get; set; }
        public int RecordsAffected { get; set; } = Common.ROW_COUNT_ERROR;
        public IList<IError> Errors { get; set; } = new List<IError>();
        public bool HaveErrors { get => Errors.Where(e => e.Type == ErrorTypes.ERROR).Count() > 0; }
        public string Message { get; set; } = string.Empty;

        public int IngestErrors(IList<IError> errs)
        {
            if (errs is null || errs.Count == 0) return 0;
            (Errors as List<IError>).AddRange(errs);
            return errs.Where(v => v.Type == ErrorTypes.ERROR).Count();
        }
    }
}
