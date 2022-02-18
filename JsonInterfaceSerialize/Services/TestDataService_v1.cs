using JsonInterfaceSerialize.DataModels.ModelsV1;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace JsonInterfaceSerialize.Services
{
    public class TestDataService_v1
    {

        public static readonly JsonSerializerSettings JsonSerializerSettings_V1 = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            TypeNameHandling = TypeNameHandling.None,

            NullValueHandling = NullValueHandling.Include,
            CheckAdditionalContent = false,
            MaxDepth = 5,
        };

        public static JisCountry SampleCountry_V1(string tag = null)
        {
            JisCountry loCountry = new JisCountry
            {
                // https://en.wikipedia.org/wiki/India
                Name = "India",
                OfficialName = "Republic of India",
                Capital = "New Delhi",
                Area = 3287263,
                Population = 1352642280,
                States = new List<JisState>
                { new JisState
                    {
                        // https://en.wikipedia.org/wiki/Goa
                        Name       = "Goa",
                        Capital    = "Panaji / Panjim",
                        Area       = 3702,
                        Population = 1458545,
                        Districts  = new List<JisDistrict>
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

        public static string SerializeObjectV1<T>(T obj)
        { return JsonConvert.SerializeObject(obj, JsonSerializerSettings_V1); }

    }
}