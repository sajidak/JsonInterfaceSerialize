using System.Collections.Generic;

namespace JsonInterfaceSerialize.DataModels.ModelsV3
{
    public interface IJisState
    {
        long Area { get; set; }
        string Capital { get; set; }
        IList<IJisDistrict> Districts { get; set; }
        string Name { get; set; }
        long Population { get; set; }
    }

    public class JisState : IJisState
    {
        public string Name { get; set; }

        public string Capital { get; set; }

        public long Area { get; set; }

        public long Population { get; set; }

        public IList<IJisDistrict> Districts { get; set; } = new List<IJisDistrict>();
    }
}
