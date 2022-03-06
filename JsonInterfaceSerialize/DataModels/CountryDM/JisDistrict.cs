using JsonInterfaceSerialize.DataModels.DataInterfaces;

namespace JsonInterfaceSerialize.DataModels.CountryDM
{
    public class JisDistrict : IJisDistrict
    {
        public string Name { get; set; }

        public string Capital { get; set; }

        public long Area { get; set; }

        public long Population { get; set; }
    }
}
