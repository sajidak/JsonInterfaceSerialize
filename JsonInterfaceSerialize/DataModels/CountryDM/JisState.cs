using JsonInterfaceSerialize.DataModels.DataInterfaces;
using System.Collections.Generic;

namespace JsonInterfaceSerialize.DataModels.CountryDM
{
    public class JisState : IJisState
    {
        public string Name { get; set; }

        public string Capital { get; set; }

        public long Area { get; set; }

        public long Population { get; set; }

        public IList<IJisDistrict> Districts { get; set; } = new List<IJisDistrict>();
    }
}
