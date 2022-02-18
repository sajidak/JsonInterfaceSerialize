using System.Collections.Generic;

namespace JsonInterfaceSerialize.DataModels.ModelsV1
{
    public class JisState
    {
        public string Name { get; set; }

        public string Capital { get; set; }

        public long Area { get; set; }

        public long Population { get; set; }

        public List<JisDistrict> Districts { get; set; } = new List<JisDistrict>();
    }
}
