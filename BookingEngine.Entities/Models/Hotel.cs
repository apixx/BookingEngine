using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingEngine.Entities.Models
{
    public class Hotel
    {
        public string HotelId { get; set; }
        public string Name { get; set;}
        public string? Description { get; set;}
        public int Rating { get; set; }
        public ICollection<SearchRequestHotel> SearchRequestHotels { get; set; }
    }
}
