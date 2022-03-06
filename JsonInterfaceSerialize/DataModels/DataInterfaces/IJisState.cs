using System.Collections.Generic;

namespace JsonInterfaceSerialize.DataModels.DataInterfaces
{
    public interface IJisState
    {
        long Area { get; set; }
        string Capital { get; set; }
        IList<IJisDistrict> Districts { get; set; }
        string Name { get; set; }
        long Population { get; set; }
    }
}
