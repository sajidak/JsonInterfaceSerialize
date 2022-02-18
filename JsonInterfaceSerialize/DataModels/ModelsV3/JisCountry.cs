using System.Collections.Generic;

namespace JsonInterfaceSerialize.DataModels.ModelsV3
{
    public interface IJisCountry
    {
        long Area { get; set; }
        string Capital { get; set; }
        string Name { get; set; }
        string OfficialName { get; set; }
        long Population { get; set; }
        IList<IJisState> States { get; set; }
    }

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
