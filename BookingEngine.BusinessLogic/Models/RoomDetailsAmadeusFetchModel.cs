using BookingEngine.BusinessLogic.Models.AmadeusApiCustomModels;

namespace BookingEngine.BusinessLogic.Models;

public class RoomDetailsAmadeusFetchModel
{
    public AmadeusApiRoomDetailsResponseItem Item { get; set; }

    public string ToCacheKey()
    {
        var hotel = this.Item.Hotel;
        var offers = this.Item.Offers;
        return String.Format("{0},{1},{2}", hotel, offers.FirstOrDefault(), Item.Available);
    }
}

