using System.Collections.Generic;

namespace JsonInterfaceSerialize.DataModels.DataInterfaces
{
    public interface IValidatorResultObject<T>
    {
        T DataObject { get; set; }
        bool HaveValidationErrors { get; }
        IList<IValidationError> ValidationErrors { get; set; }
    }
}
