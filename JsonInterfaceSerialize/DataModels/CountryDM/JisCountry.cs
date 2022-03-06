using JsonInterfaceSerialize.DataModels.DataInterfaces;
using System.Collections.Generic;

namespace JsonInterfaceSerialize.DataModels.CountryDM
{
    public class JisCountry : IJisCountry
    {
        public string Name { get; set; }
        public string OfficialName { get; set; }

        public string Capital { get; set; }

        public long Area { get; set; }

        public long Population { get; set; }

        public IList<IJisState> States { get; set; } = new List<IJisState>();
    }
}
