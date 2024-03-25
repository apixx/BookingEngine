namespace BookingEngine.BusinessLogic.Models
{
    public class HotelByCitySearchResponse
    {
        public List<HotelData> Data { get; set; }
        public Meta Meta { get; set; }
    }

    public class GeoCode
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class Address
    {
        public string CountryCode { get; set; }
    }

    public class Distance
    {
        public double Value { get; set; }
        public string Unit { get; set; }
    }

    public class Meta
    {
        public int Count { get; set; }
        public Links Links { get; set; }
    }

    public class Links
    {
        public string Self { get; set; }
    }

}
