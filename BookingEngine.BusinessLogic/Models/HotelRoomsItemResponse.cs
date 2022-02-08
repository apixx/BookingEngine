using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingEngine.BusinessLogic.Models
{
    public class HotelRoomsItemResponse
    {
        public string OfferId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int RoomQuantity { get; set; }
        public string RateCode { get; set; }
        public string Description { get; set; }
        public string? BoardType { get; set; }
        public string RoomType { get; set; }
        public int Guests { get; set; }
        public float Price { get; set; }
        public List<string> Media { get; set; }
    }
}
