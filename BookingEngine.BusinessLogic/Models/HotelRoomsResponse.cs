namespace BookingEngine.BusinessLogic.Models;

public class HotelRoomsResponse
{
    public List<HotelRoomsItemResponse> Items { get; set; }

    public HotelRoomsResponse(HotelRoomsUserRequest hotelRoomsRequest)
    {
        Items = new List<HotelRoomsItemResponse>();
    }
}