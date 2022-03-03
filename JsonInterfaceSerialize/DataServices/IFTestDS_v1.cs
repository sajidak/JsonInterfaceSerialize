using JsonInterfaceSerialize.DataModels.ModelsV4;
using JsonInterfaceSerialize.DataModels.ModelsV4.Containers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JsonInterfaceSerialize.DataServices
{
    public class IFTestDS_v1
    {
        // TODO: Write proper logs

        public static async Task<IInternalResultObject<IJisCountry>> Country_GetOne_v1(string Name, ILogger log)
        {
            // BEGIN - SIMULATE AN UNHANDLED EXCEPTION
            // This should never occur is good code.
            // Manage this scenario to ensure low-standard code does not break application unexpectedly.
            if (!string.IsNullOrWhiteSpace(Name) && Name.Equals("throw_exception", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception("Simulating an unhandled exception");
            }
            // FINIS - SIMULATE AN UNHANDLED EXCEPTION

            IInternalResultObject<IJisCountry> IRO = new InternalResultObject<IJisCountry>
            {
                Result = null
            };
            IJisCountry loCountry = null;
            try
            {
                log.LogInformation("Get Country with Name = {0}", Name);    // Keep logger inside try-catch block, in case unusable logger is sent
                // Sample object till a data source is implemented
                if (Name.Equals("India", StringComparison.InvariantCultureIgnoreCase))
                {

                    loCountry = new JisCountry
                    {
                        // https://en.wikipedia.org/wiki/India
                        Name = "India",
                        OfficialName = "Republic of India",
                        Capital = "New Delhi",
                        Area = 3287263,
                        Population = 1352642280,
                        States = new List<IJisState>
                    { new JisState
                        {
                            // https://en.wikipedia.org/wiki/Goa
                            Name       = "Goa",
                            Capital    = "Panaji / Panjim",
                            Area       = 3702,
                            Population = 1458545,
                            Districts  = new List<IJisDistrict>
                            {
                                new JisDistrict
                                {
                                    // https://en.wikipedia.org/wiki/North_Goa_district
                                    Name       = "North Goa",
                                    Capital    = "Panaji",
                                    Area       = 1736,
                                    Population = 818008,
                                },
                                new JisDistrict
                                {
                                    // https://en.wikipedia.org/wiki/South_Goa_district
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
                IRO.Errors.Add(new Error { Type = ErrorTypes.ERROR, Message = $"Failed to get Country with Name = {Name}. ErrMesg = {se.Message}" });
                log.LogError(se, "Failed to get Country with Name = {0}", Name);
            }
            IRO.Result = loCountry;
            return IRO;
        }
    }
}
