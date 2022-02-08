using BookingEngine.BusinessLogic.Models.AmadeusApiCustomModels;
using BookingEngine.BusinessLogic.Models.AmadeusApiCustomModels.Hotel.HotelSearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingEngine.BusinessLogic.Models
{
    public class HotelsSearchAmadeusFetchModel
    {
        public List<AmadeusApiHotelsSearchResponseItem> Items { get; set; }
        public string nextItemsUrl {  get; set; }
    }
}
    