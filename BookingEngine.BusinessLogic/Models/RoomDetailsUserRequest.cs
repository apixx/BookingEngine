namespace BookingEngine.BusinessLogic.Models;

public class RoomDetailsUserRequest
{
    public string OfferId { get; set; }
    //public string Lang { get; set; } = "en";

    public RoomDetailsUserRequest(string offerId)
    {
        OfferId = offerId;
    }
}