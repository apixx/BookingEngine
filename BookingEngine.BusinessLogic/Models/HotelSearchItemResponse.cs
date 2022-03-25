using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingEngine.BusinessLogic.Models
{
    public class HotelSearchItemResponse
    {
        public string HotelId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int Rating { get; set; }
        public bool Available { get; set; }
        public float? PriceTotal { get; set; }
        public string PriceCurrency { get; set; }
        public float? Distance { get; set; }
    }
}
