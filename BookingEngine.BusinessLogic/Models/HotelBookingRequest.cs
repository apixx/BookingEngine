using BookingEngine.BusinessLogic.Models.AmadeusApiCustomModels.Hotel.Booking;

namespace BookingEngine.BusinessLogic.Models;

public class HotelBookingRequest
{
    public AmadeusApiHotelBookingRequestItem HotelBookingRequestData { get; set; }

    public HotelBookingRequest(AmadeusApiHotelBookingRequestItem request)
    {
        HotelBookingRequestData = request;
    }
}