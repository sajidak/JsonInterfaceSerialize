using JsonInterfaceSerialize.DataModels.DataInterfaces;
using JsonInterfaceSerialize.Utilities.Enums;
using System.Collections.Generic;
using System.Linq;

namespace JsonInterfaceSerialize.DataModels.Containers
{
    public class ValidatorResultObject<T> : IValidatorResultObject<T>
    {
        public ValidatorResultObject() { }

        public T DataObject { get; set; }
        public IList<IValidationError> ValidationErrors { get; set; } = new List<IValidationError>();
        public bool HaveValidationErrors { get => ValidationErrors.Where(v => v.Type == ErrorTypes.ERROR).Count() > 0; }

    }
}
