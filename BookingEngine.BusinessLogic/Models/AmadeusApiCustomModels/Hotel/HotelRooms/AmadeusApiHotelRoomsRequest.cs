using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingEngine.BusinessLogic.Models.AmadeusApiCustomModels
{
    public class AmadeusApiHotelRoomsRequest
    {
        public string HotelId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int Adults { get; set; }


        public AmadeusApiHotelRoomsRequest(string hotelId, DateTime checkInDate, DateTime checkOutDate, int adults)
        {
            HotelId = hotelId;
            CheckInDate = checkInDate;
            CheckOutDate = checkOutDate;
            Adults = adults;
        }

        public async Task<string> ToUrlParamsString()
        {
            var urlParams = new Dictionary<string, string>();

            urlParams.Add("hotelId", HotelId);
            urlParams.Add("checkInDate", CheckInDate.ToString("yyyy-MM-dd"));
            urlParams.Add("checkOutDate", CheckOutDate.ToString("yyyy-MM-dd"));
            urlParams.Add("adults", Adults.ToString());

            using (HttpContent content = new FormUrlEncodedContent(urlParams))
            {
                return await content.ReadAsStringAsync();
            }
        }
    }
}
