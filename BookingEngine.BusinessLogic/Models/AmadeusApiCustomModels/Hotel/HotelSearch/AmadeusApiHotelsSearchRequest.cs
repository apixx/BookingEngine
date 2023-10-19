using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingEngine.BusinessLogic.Models.AmadeusApiCustomModels.Hotel.HotelSearch
{
    //[
    //{
    //  "key":"cityCode",
    //  "value":"DEL",
    //  "description":"Destination city code or airport code. In case of city code , the search will be done around the city center. Available codes can be found in IATA table codes (3 chars IATA Code).",
    //},
    //{
    //  "key":"radius",
    //  "value":"5",
    //  "description":"Maximum distance from the geographical coordinates express in defined units. The default radius is 5 KM.",
    //}
    //{
    //  "key":"radiusUnit",
    //  "value":"KM",
    //  "description":"Unit of measurement used to express the radius. It can be either metric kilometer or imperial mile."
    //},
    //{
    //  "key":"chainCodes",
    //  "value":"",
    //  "description":"Array of hotel chain codes. Each code is a string consisted of 2 capital alphabetic characters."
    //},
    //{
    //  "key":"amenities",
    //  "value":"",
    //  "description":"List of amenities"
    //},
    //{
    //  "key":"ratings",
    //  "value":"",
    //  "description":"Hotel stars. Up to four values can be requested at the same time in a comma separated list."
    //},
    //{
    //  "key":"hotelSource",
    //  "value":"ALL",
    //  "description":"Hotel source with values BEDBANK for aggregators, DIRECTCHAIN for GDS/Distribution and ALL for both"
    //}
    //]
    public class AmadeusApiHotelsSearchRequest
    {
        // update the class with the above defined properties
        
        #region userProvided
        public string CityCode { get; set; }
        public List<string> ChainCodes { get; set; }
        public List<string> Amenities { get; set; }
        public List<string> Ratings { get; set; }
        public string HotelSource { get; set; }

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
