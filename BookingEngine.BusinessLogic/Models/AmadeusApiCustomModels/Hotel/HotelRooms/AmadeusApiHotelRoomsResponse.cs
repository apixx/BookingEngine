using BookingEngine.BusinessLogic.Models.AmadeusApiCustomModels.Hotel.HotelSearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingEngine.BusinessLogic.Models.AmadeusApiCustomModels
{
    public class AmadeusApiHotelRoomsResponse
    {
        public AmadeusApiHotelRoomsResponseItem Data { get; set; }
        public MetaItem Meta { get; set; }
        public List<Warning> Warnings { get; set; }
    }

    public class AmadeusApiHotelRoomsResponseItem
    {
        public AmadeusApiHotelItem Hotel { get; set; }            
        public bool Available { get; set; }
        public List<AmadeusApiOfferItem> Offers { get; set; }


    }

    public class HotelItem
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

    public class AmadeusApiOfferItem
    {
        public string Id { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int RoomQuantity { get; set; }
        public string RateCode { get; set; }
        public RateFamilyEstimatedItem RateFamilyEstimated { get; set; }
        public DescriptionItem? Description { get; set; }
        public CommissionItem? Commission { get; set; }
        public string? BoardType { get; set; }  
        public RoomItem Room { get; set; }
        public GuestsItem? Guests { get; set; }
        public PriceItem Price { get; set; }
        // public List<MediaItem> Media { get; set; }  - no media items in offer item
    }

    public class RateFamilyEstimatedItem
    {
        public string Code { get; set; }
        public string Type { get; set; }
    }

    public class CommissionItem
    {
        public string? Percentage { get; set; }
        public string? Amount { get; set; }
        public DescriptionItem? Description { get; set; }
    }

    public class RoomItem
    {
        public string Type { get; set; }
        public EstimatedRoomTypeItem? TypeEstimated { get; set; }
        public DescriptionItem Description { get; set; }
    }

    public class EstimatedRoomTypeItem
    {
        public DescriptionItem Description { get; set; }
        public string Category { get; set; }
        public int Beds { get; set; }
        public string BedType { get; set; }
    }

    public class GuestsItem
    {
        public int Adults { get; set; }
    }

    public class DescriptionItem
    {
        public string? Lang { get; set; }
        public string? Text { get; set; }
    }
    public class AddressItem
    {
        public List<string>? Lines { get; set; }
        public string? PostalCode { get; set; }
        public string? CityName { get; set; }
        public string? CountryCode { get; set; }
        public string? StateCode { get; set; }
    }

    public class DistanceItem
    {
        public float? Distance { get; set; }
        public string? DistanceUnit { get; set; }
    }

    public class PriceItem
    {
        public float Total { get; set; }
        public string Currency { get; set; }
    }

    public class MediaItem
    {
        public string Uri { get; set; }
        public string Category { get; set; }
    }

    public class MetaItem
    {
        public LinksItem? Links { get; set; }
    }

    public class LinksItem
    {
        public string? Next { get; set; }
    }

    public class Warning
    {
        public int Code { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
    }
}
