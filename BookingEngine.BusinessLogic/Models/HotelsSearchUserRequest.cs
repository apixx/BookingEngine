using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingEngine.BusinessLogic.Models
{
    public class HotelsSearchUserRequest
    {
        /// <summary>
        /// City Code - only IATA code values are valid
        /// </summary>
        public string CityCode { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int Adults { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int PageOffset { get; set; } = 0;

        public HotelsSearchUserRequest(string cityCode, DateTime checkInDate, DateTime checkOutDate, int adults, int pageSize, int pageOffset)
        {
            CityCode = cityCode;
            CheckInDate = checkInDate;
            CheckOutDate = checkOutDate;
            Adults = adults;
            PageSize = pageSize;
            PageOffset = pageOffset;
        }

        public string ToCacheKey()
        {
            return String.Format("{0},{1},{2},{3},{4},{5}", this.CityCode, this.CheckInDate, this.CheckOutDate, this.PageSize, this.PageOffset, this.Adults);
        }
    }
}
