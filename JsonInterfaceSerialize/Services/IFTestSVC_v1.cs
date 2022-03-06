using JsonInterfaceSerialize.DataModels.Containers;
using JsonInterfaceSerialize.DataModels.DataInterfaces;
using JsonInterfaceSerialize.DataServices;
using JsonInterfaceSerialize.Utilities.Enums;
using JsonInterfaceSerialize.Utilities.Helpers;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace JsonInterfaceSerialize.Services
{
    public interface IIFTestSVC_v1 : IDisposable
    {
        Task<IInternalResultObject<IJisCountry>> Country_GetOne_v1_Async(string Name, ILogger _logger);
    }

    public class IFTestSVC_v1 : IIFTestSVC_v1
    {
        // TODO: Write proper logs

        public IFTestSVC_v1() { }
        public IFTestSVC_v1(ILogger logger) : this() { Log = logger; }

        private ILogger Log { get; set; }

        public async Task<IInternalResultObject<IJisCountry>> Country_GetOne_v1_Async(string Name, ILogger _logger = null)
        {
            // BEGIN - RETURN A NULL CONTAINER
            // Returning a null container is NOT good coding practice.
            // Manage this scenario to ensure low-standard code does not break application unexpectedly.
            if (!string.IsNullOrWhiteSpace(Name) && Name.Equals("null_value", StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }
            // BEGIN - RETURN A NULL CONTAINER

            if (Log is null && _logger is null) throw new NullReferenceException("No usable logger availaible");
            if (_logger != null) Log = _logger;

            IInternalResultObject<IJisCountry> IRO = new InternalResultObject<IJisCountry>
            {
                Result = null
            };
            int liErrCount = 0;

            try
            {
                Log.LogInformation("Get Country with Name = {0}", Name);    // Keep logger inside try-catch block, in case unusable logger is sent

                using (IIFTestDS_v1 loCountryDataService = new IFTestDS_v1(Log))
                {
                    IInternalResultObject<IJisCountry> loDSResult;
                    loDSResult = await loCountryDataService.Country_GetOne_v1_Async(Name, Log);
                    liErrCount = IRO.IngestErrors(loDSResult?.Errors);
                    IRO.Result = loDSResult?.Result;
                }
                if (IRO.Result is null)
                {
                    IRO.Errors.Add(new Error { Type = ErrorTypes.WARN, Message = $"Country with Name {Name} could not be found." });
                }
                if (liErrCount > 0)
                {
                    IRO.Errors.Add(new Error { Type = ErrorTypes.ERROR, Message = $"Unexpected error from DataSource. For Country Name = {Name}" });
                }
            }
            catch (Exception se)
            {
                string lsErrData = ExceptionHelpers.SerializeExceptionTxt(se, $"Failed to get Country with Name = {Name} from DataService.");
                IRO.Errors.Add(new Error { Type = ErrorTypes.ERROR, Message = lsErrData });
                Log.LogError(lsErrData);
            }
            return IRO;
        }

        public void Dispose() { }
    }
}
