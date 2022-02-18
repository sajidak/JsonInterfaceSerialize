using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using JsonInterfaceSerialize.DataModels.ModelsV2;

namespace JsonInterfaceSerialize.Services
{
    public class TestDataService_v2
    {
        public static readonly JsonSerializerSettings JsonSerializerSettings_V2 = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            TypeNameHandling = TypeNameHandling.None,

            NullValueHandling = NullValueHandling.Include,
            CheckAdditionalContent = false,
            MaxDepth = 5,
        };

        public static JisCountry SampleCountry_V2(string tag = null)
        {
            JisCountry loCountry = new JisCountry
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
            return loCountry;
        }

        public static string SerializeObjectV2<T>(T obj)
        { return JsonConvert.SerializeObject(obj, JsonSerializerSettings_V2); }

    }
}
