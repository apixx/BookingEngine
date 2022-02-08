using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingEngine.Entities.Models
{
    public class SearchRequestHotel
    {
        public int SearchRequestHotelId { get; set; }
        public int SearchRequestId { get; set;}
        public string HotelId { get; set; }
        public bool Available { get; set; }
        public float? PriceTotal { get; set; }
        public string PriceCurrency { get; set; }
        public float? Distance { get; set; }
        public SearchRequest SearchRequest { get; set; }
        public Hotel Hotel { get; set; }
    }
}
