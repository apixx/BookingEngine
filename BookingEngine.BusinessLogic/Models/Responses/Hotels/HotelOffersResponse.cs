using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingEngine.BusinessLogic.Models
{
    public class HotelOffersResponse
    {
        public List<HotelOfferResponse> Data { get; set; }
    }
    public class HotelOfferResponse
    {
        public string Type { get; set; }
        public HotelData Hotel { get; set; }
        public bool Available { get; set; }
        public List<HotelOffer> Offers { get; set; }
        public string Self { get; set; }
    }
    public class HotelData
    {
        public string Type { get; set; }
        public string HotelId { get; set; }
        public string ChainCode { get; set; }
        public string DupeId { get; set; }
        public string Name { get; set; }
        public string CityCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class HotelOffer
    {
        public string Id { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public string RateCode { get; set; }
        public RateFamilyEstimated RateFamilyEstimated { get; set; }
        public Room Room { get; set; }
        public Guests Guests { get; set; }
        public Price Price { get; set; }
        public Policies Policies { get; set; }
        public string Self { get; set; }
    }

    public class RateFamilyEstimated
    {
        public string Code { get; set; }
        public string Type { get; set; }
    }

    public class Room
    {
        public string Type { get; set; }
        public TypeEstimated TypeEstimated { get; set; }
        public Description Description { get; set; }
    }

    public class TypeEstimated
    {
        public string Category { get; set; }
    }

    public class Description
    {
        public string Text { get; set; }
        public string Lang { get; set; }
    }

    public class Guests
    {
        public int Adults { get; set; }
    }

    public class Price
    {
        public string Currency { get; set; }
        public string Base { get; set; }
        public string Total { get; set; }
        public List<Tax> Taxes { get; set; }
        public Variations Variations { get; set; }
    }

    public class Tax
    {
        public string Code { get; set; }
        public string PricingFrequency { get; set; }
        public string PricingMode { get; set; }
        public string Percentage { get; set; }
        public bool Included { get; set; }
    }

    public class Variations
    {
        public Average Average { get; set; }
        public List<Change> Changes { get; set; }
    }

    public class Average
    {
        public string Base { get; set; }
    }

    public class Change
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Base { get; set; }
    }

    public class Policies
    {
        public List<Cancellation> Cancellations { get; set; }
        public Deposit Deposit { get; set; }
        public string PaymentType { get; set; }
    }

    public class Cancellation
    {
        public string Type { get; set; }
        public Description Description { get; set; }
    }

    public class Deposit
    {
        public AcceptedPayments AcceptedPayments { get; set; }
    }

    public class AcceptedPayments
    {
        public List<string> CreditCards { get; set; }
        public List<string> Methods { get; set; }
    }

}
