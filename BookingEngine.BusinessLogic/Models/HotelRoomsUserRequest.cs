using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingEngine.BusinessLogic.Models
{
    public class HotelRoomsUserRequest
    {
        public string HotelId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int Adults { get; set; }

        public HotelRoomsUserRequest(string hotelId, DateTime checkInDate, DateTime checkOutDate, int adults)
        {
            HotelId = hotelId;
            CheckInDate = checkInDate;
            CheckOutDate = checkOutDate;
            Adults = adults;
        }
    }
}
