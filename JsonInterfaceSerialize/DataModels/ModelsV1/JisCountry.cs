using System.Collections.Generic;

namespace JsonInterfaceSerialize.DataModels.ModelsV1
{
    public class JisCountry
    {
        public string Name { get; set; }
        public string OfficialName { get; set; }

        public string Capital { get; set; }

        public long Area { get; set; }

        public long Population { get; set; }

        public List<JisState> States { get; set; } = new List<JisState>();
    }
}
