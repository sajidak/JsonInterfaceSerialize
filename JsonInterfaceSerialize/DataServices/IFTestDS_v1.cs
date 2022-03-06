using JsonInterfaceSerialize.DataModels.Containers;
using JsonInterfaceSerialize.DataModels.CountryDM;
using JsonInterfaceSerialize.DataModels.DataInterfaces;
using JsonInterfaceSerialize.Utilities.Enums;
using JsonInterfaceSerialize.Utilities.Helpers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JsonInterfaceSerialize.DataServices
{
    public interface IIFTestDS_v1 : IDisposable
    {
        Task<IInternalResultObject<IJisCountry>> Country_GetOne_v1_Async(string Name, ILogger _logger);
    }

    public class IFTestDS_v1 : IIFTestDS_v1
    {
        // TODO: Write proper logs

        public IFTestDS_v1() { }
        public IFTestDS_v1(ILogger logger) : this() { Log = logger; }

        private ILogger Log { get; set; }


        public async Task<IInternalResultObject<IJisCountry>> Country_GetOne_v1_Async(string Name, ILogger _logger = null)
        {
            // BEGIN - SIMULATE AN UNHANDLED EXCEPTION
            // This should never occur is good code.
            // Manage this scenario to ensure low-standard code does not break application unexpectedly.
            if (!string.IsNullOrWhiteSpace(Name) && Name.Equals("throw_exception", StringComparison.InvariantCultureIgnoreCase))
            {
                // throw new Exception("Simulating an unhandled exception");
                string lsVar1 = null; _ = lsVar1.Trim();  // This generates a more realistic exception, with proper stack trace
            }
            // FINIS - SIMULATE AN UNHANDLED EXCEPTION

            if (Log is null && _logger is null) throw new NullReferenceException("No usable logger availaible");
            if (_logger != null) Log = _logger;

            IInternalResultObject<IJisCountry> IRO = new InternalResultObject<IJisCountry>
            {
                Result = null
            };
            IJisCountry loCountry = null;

            try
            {
                Log.LogInformation("Get Country with Name = {0}", Name);    // Keep logger inside try-catch block, in case unusable logger is sent
                // Sample object till a data source is implemented
                if (Name.Equals("India", StringComparison.InvariantCultureIgnoreCase))
                {
                    loCountry = new JisCountry
                    {   // https://en.wikipedia.org/wiki/India
                        Name = "India",
                        OfficialName = "Republic of India",
                        Capital = "New Delhi",
                        Area = 3287263,
                        Population = 1352642280,
                        States = new List<IJisState>
                        { new JisState
                            {   // https://en.wikipedia.org/wiki/Goa
                                Name       = "Goa",
                                Capital    = "Panaji / Panjim",
                                Area       = 3702,
                                Population = 1458545,
                                Districts  = new List<IJisDistrict>
                                {
                                    new JisDistrict
                                    {   // https://en.wikipedia.org/wiki/North_Goa_district
                                        Name       = "North Goa",
                                        Capital    = "Panaji",
                                        Area       = 1736,
                                        Population = 818008,
                                    },
                                    new JisDistrict
                                    {   // https://en.wikipedia.org/wiki/South_Goa_district
                                        Name       = "South Goa",
                                        Capital    = "Margao",
                                        Area       = 1966,
                                        Population = 640537,
                                    },
                                },
                            },
                        },
                    };
                    IRO.Errors.Add(new Error { Type = ErrorTypes.INFO, Message = "Found queried country" });
                }
                else
                {
                    IRO.Errors.Add(new Error { Type = ErrorTypes.WARN, Message = $"Unknown country name {Name}" });
                    IRO.Errors.Add(new Error { Type = ErrorTypes.INFO, Message = "Queried country not found" });
                }
            }
            catch (Exception se)
            {
                string lsErrData = ExceptionHelpers.SerializeExceptionTxt(se, $"Failed to get Country with Name = {Name}.");
                IRO.Errors.Add(new Error { Type = ErrorTypes.ERROR, Message = lsErrData });
                Log.LogError(lsErrData);
                if (se is null) await Task.Delay(0); // Dummy entry to bypass code analysis warning CS1998.
            }
            IRO.Result = loCountry;
            return IRO;
        }

        public void Dispose() { }
    }
}
