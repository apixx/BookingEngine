using BookingEngine.BusinessLogic.Models;

namespace BookingEngine.BusinessLogic.Services.Interfaces;

public interface IAmadeusApiHotelBookingServiceProvider
{
    Task<HotelBookingAmadeusFetchModel> FetchAmadeusHotelBooking(HotelBookingUserRequest hotelBookingUserRequest, CancellationToken cancellationToken);
    Task CompleteBooking(HotelBookingUserRequest hotelBookingUserRequest, RoomDetailsAmadeusFetchModel roomDetailsResponse, HotelBookingAmadeusFetchModel bookingResponse, DateTime checkInDate, DateTime checkOutDate, CancellationToken cancellationToken);
}