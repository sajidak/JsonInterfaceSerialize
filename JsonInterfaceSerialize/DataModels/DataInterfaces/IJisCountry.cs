using System.Collections.Generic;

namespace JsonInterfaceSerialize.DataModels.DataInterfaces
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
}
