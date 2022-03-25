using BookingEngine.BusinessLogic.Models.AmadeusApiCustomModels.Hotel.Booking;

namespace BookingEngine.BusinessLogic.Models;

public class HotelBookingUserRequest
{
    public AmadeusApiHotelBookingRequestItem HotelBookingRequest { get; set; }

    public HotelBookingUserRequest(AmadeusApiHotelBookingRequestItem request)
    {
        HotelBookingRequest = request;
    }
}