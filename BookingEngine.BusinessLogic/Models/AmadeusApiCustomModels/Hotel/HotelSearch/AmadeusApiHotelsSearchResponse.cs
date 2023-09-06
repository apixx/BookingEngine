using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingEngine.BusinessLogic.Models.AmadeusApiCustomModels.Hotel.HotelSearch
{
    public class AmadeusApiHotelsSearchResponse
    {
        public List<AmadeusApiHotelsSearchResponseItem> Data { get; set; }
        public MetaItem Meta { get; set; }
    }

    public class AmadeusApiHotelsSearchResponseItem
    {
        public AmadeusApiHotelItem Hotel { get; set; }
        public bool Available { get; set; }
        public List<AmadeusApiHotelItemOffer> Offers { get; set; }
        public PriceItem BestOffer
        {
            get
            {
                if (Available)
                {
                    return Offers.OrderBy(x => x.Price.Total).FirstOrDefault().Price;
                }
                else
                {
                    return null;
                }
            }
        }
    }
    public class AmadeusApiHotelItem
    {
        public string HotelId { get; set; }
        public string Name { get; set; }
        public int Rating { get; set; }
        public string CityCode { get; set; }
        public DescriptionItem Description { get; set; }
        public DistanceItem HotelDistance { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public AddressItem Address { get; set; }
        public List<string> Amenities { get; set; }
        public List<MediaItem> media { get; set; }
    }

    public class AmadeusApiHotelItemOffer
    {
        public PriceItem Price { get; set; }
    }

    //public class PriceItem
    //{
    //    public float Total { get; set; }
    //    public string Currency { get; set; }
    //}

    //public class DescriptionItem
    //{
    //    public string? Lang { get; set; }
    //    public string? Text { get; set; }
    //}

    public class DistanceItem
    {
        public float? Distance { get; set; }
        public string? DistanceUnit { get; set; }
    }

    public class MetaItem
    {
        public LinksItem? Links { get; set; }
    }

    public class LinksItem
    {
        public string? Next { get; set; }
    }

    public class AddressItem
    {
        public List<string>? Lines { get; set; }
        public string? PostalCode { get; set; }
        public string? CityName { get; set; }
        public string? CountryCode { get; set; }
        public string? StateCode { get; set; }
    }

    public class ContactItem
    {
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string? Email { get; set; }
    }
}
