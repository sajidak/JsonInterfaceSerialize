using JsonInterfaceSerialize.DataModels.ModelsV4;
using JsonInterfaceSerialize.DataServices;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using JsonInterfaceSerialize.DataModels.ModelsV4.Containers;
using System.Threading.Tasks;

namespace JsonInterfaceSerialize.Services
{
    public class IFTestSVC_v1
    {
        // TODO: Write proper logs

        public static async Task<IInternalResultObject<IJisCountry>> Country_GetOne_v1(string Name, ILogger log)
        {
            // BEGIN - RETURN A NULL CONTAINER
            // Returning a null container is NOT good coding practice.
            // Manage this scenario to ensure low-standard code does not break application unexpectedly.
            if (!string.IsNullOrWhiteSpace(Name) && Name.Equals("null_value", StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }
            // BEGIN - RETURN A NULL CONTAINER

            IInternalResultObject<IJisCountry> IRO = new InternalResultObject<IJisCountry>
            {
                Result = null
            };
            try
            {
                log.LogInformation("Get Country with Name = {0}", Name);    // Keep logger inside try-catch block, in case unusable logger is sent
                IInternalResultObject<IJisCountry> loDSResult = await IFTestDS_v1.Country_GetOne_v1(Name, log);
                if (loDSResult == null)
                {
                    IRO.Errors.Add(new Error { Type = ErrorTypes.ERROR, Message = $"Unexpected error from DataSource. For Country Name = {Name}" });
                }
                else
                {
                    IRO.IngestErrors(loDSResult.Errors);
                    IRO.Result = loDSResult.Result;
                }
                if (IRO.Result is null)
                {
                    IRO.Errors.Add(new Error { Type = ErrorTypes.WARN, Message = $"Country with Name {Name} could not be found." });
                }
            }
            catch (Exception se)
            {
                IRO.Errors.Add(new Error { Type = ErrorTypes.ERROR, Message = $"Failed to get Country with Name = {Name}. ErrMesg = {se.Message}" });
                log.LogError(se, "Failed to get Country with Name = {0}", Name);
            }
            return IRO;
        }
    }
}
