using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingEngine.BusinessLogic.Models;
using BookingEngine.BusinessLogic.Models.AmadeusApiCustomModels;

namespace BookingEngine.BusinessLogic.Services.Interfaces
{
    public interface IAmadeusApiHotelRoomsServiceProvider
    {
        Task<HotelRoomsAmadeusFetchModel> FetchAmadeusHotelRooms(HotelRoomsUserRequest hotelRoomsRequest, CancellationToken cancellationToken);
        Task<RoomDetailsAmadeusFetchModel> FetchAmadeusRoomDetails(RoomDetailsUserRequest roomDetailsRequest, CancellationToken cancellationToken);
    }
}
