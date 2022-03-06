using JsonInterfaceSerialize.DataModels.Containers;
using JsonInterfaceSerialize.DataModels.DataInterfaces;
using JsonInterfaceSerialize.Utilities.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;

namespace JsonInterfaceSerialize.Validators
{
    public interface ICountryValidator : IDisposable
    {
        IValidatorResultObject<string> Countries_Validator_GetOne(HttpRequest req, ILogger log);
    }

    public class CountryValidator : ICountryValidator
    {
        public CountryValidator() { }
        public CountryValidator(ILogger logger) : this() { Log = logger; }

        private ILogger Log { get; set; }


        public IValidatorResultObject<string> Countries_Validator_GetOne(HttpRequest req, ILogger _logger = null)
        {
            if (Log is null && _logger is null) throw new NullReferenceException("No usable logger availaible");
            if (_logger != null) Log = _logger;

            IValidatorResultObject<string> VRO = new ValidatorResultObject<string>() { DataObject = string.Empty };

            // Mandatory fields
            if (req.Query.ContainsKey("CountryName") && !string.IsNullOrWhiteSpace(req.Query["CountryName"].ToString()))
            {
                VRO.DataObject = req.Query["CountryName"].ToString().Trim();
            }
            else
            {
                VRO.ValidationErrors.Add(new ValidationError { Type = ErrorTypes.ERROR, Field = "CountryName", Message = "Expected a non-empty string value, but is missing or empty." });
            }

            return VRO;
        }
        public void Dispose() { }
    }
}
