namespace JsonInterfaceSerialize.DataModels.ModelsV3
{
    public interface IJisDistrict
    {
        long Area { get; set; }
        string Capital { get; set; }
        string Name { get; set; }
        long Population { get; set; }
    }

    public class JisDistrict : IJisDistrict
    {
        public string Name { get; set; }

        public string Capital { get; set; }

        public long Area { get; set; }

        public long Population { get; set; }
    }
}
