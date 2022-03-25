using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingEngine.BusinessLogic.Models.AmadeusApiCustomModels.Hotel.HotelSearch
{
    public class AmadeusApiHotelsSearchRequest
    {
        #region userProvided
        public string CityCode { get; set; }

        public DateTime CheckInDate { get; set; }

        public DateTime CheckOutDate { get; set; }
        #endregion

        #region defaults by requirements in assingment
        public int Radius { get; } = 100;

        public string RadiusUnit { get; } = "KM";

        public int Adults { get; set; } = 1;

        public bool BestRateOnly { get; set; } = true;

        public bool IncludeClosed { get; } = false;

        public string Sort { get; } = "DISTANCE";
        #endregion

        public AmadeusApiHotelsSearchRequest(string cityCode, DateTime checkInDate, DateTime checkOutDate)
        {
            CityCode = cityCode;
            CheckInDate = checkInDate;  
            CheckOutDate = checkOutDate;
        }

        public async Task<string> ToUrlParamsString()
        {
            var urlParams = new Dictionary<string, string>();

            urlParams.Add("cityCode", CityCode);
            urlParams.Add("radius", Radius.ToString());
            urlParams.Add("radiusUnit", RadiusUnit.ToString());
            urlParams.Add("checkInDate", CheckInDate.ToString("yyyy-MM-dd"));
            urlParams.Add("checkOutDate", CheckOutDate.ToString("yyyy-MM-dd"));
            urlParams.Add("adults", Adults.ToString());
            urlParams.Add("includeClosed", IncludeClosed.ToString().ToLower());
            urlParams.Add("bestRateOnly", BestRateOnly.ToString().ToLower());
            urlParams.Add("sort", Sort.ToString().ToUpper());

            using (HttpContent content = new FormUrlEncodedContent(urlParams))
            {
                return await content.ReadAsStringAsync();
            }
        }
    }
}
