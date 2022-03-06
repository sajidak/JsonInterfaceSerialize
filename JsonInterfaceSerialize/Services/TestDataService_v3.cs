using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using JsonInterfaceSerialize.DataModels.ModelsV3;
using JsonInterfaceSerialize.DataModels.ModelsV3.Containers;

namespace JsonInterfaceSerialize.Services
{
    public class TestDataService_v3
    {
        public static readonly JsonSerializerSettings JsonSerializerSettings_V3 = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            TypeNameHandling = TypeNameHandling.None,

            NullValueHandling = NullValueHandling.Include,
            CheckAdditionalContent = false,
            MaxDepth = 5,
        };

        public static JisCountry SampleCountry_V3()
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

        public static ResultObject<JisCountry> SampleContainer_v3()
        {
            ResultObject<JisCountry> RO = new ResultObject<JisCountry>(Guid.NewGuid().ToString())
            {
                Data = SampleCountry_V3()
            };
            // TODO: set other properties
            return RO;
        }
        public static string SerializeObjectV2<T>(T obj) { return JsonConvert.SerializeObject(obj, JsonSerializerSettings_V3); }

    }
}
