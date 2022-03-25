using BookingEngine.BusinessLogic.Models.AmadeusApiCustomModels.Hotel.HotelSearch;

namespace BookingEngine.BusinessLogic.Models.AmadeusApiCustomModels;

public class AmadeusApiRoomDetailsResponse
{
    public AmadeusApiRoomDetailsResponseItem Data { get; set; }
    public MetaItem Meta { get; set; }
    public List<Warning> Warnings { get; set; }
}

public class AmadeusApiRoomDetailsResponseItem
{
    public AmadeusApiHotelItem Hotel { get; set; }
    public bool Available { get; set; }
    public List<AmadeusApiOfferItem> Offers { get; set; }
}