using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookingEngine.BusinessLogic.Models
{
    public class HotelSearchRequestModel
    {
        [Required]
        [RegularExpression("[a-zA-Z]{3}")]
        [StringLength(3, MinimumLength = 3)]
        public string CityCode { get; set; }

        [Range(1, 300)]
        public int? Radius { get; set; } = 5;

        [DefaultValue("KM")]
        public string RadiusUnit { get; set; } = "KM";

        [MaxLength(99)]
        public List<string>? ChainCodes { get; set; }

        [MaxLength(3)]
        public List<string>? Amenities { get; set; }

        [MaxLength(4)]
        public List<string>? Ratings { get; set; }

        [DefaultValue("ALL")]
        public string HotelSource { get; set; }
        public int Adults { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }

        public string ToCacheKey()
        {
            return String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}", this.CityCode, this.Radius, this.RadiusUnit, this.ChainCodes, this.Amenities, this.Ratings, this.HotelSource, this.Adults, this.CheckInDate, this.CheckOutDate);
        }

        public async Task<string> ToUrlParamsString()
        {
            var urlParams = new Dictionary<string, string>();

            urlParams.Add("cityCode", CityCode);
            urlParams.Add("radius", Radius.ToString());
            urlParams.Add("radiusUnit", RadiusUnit.ToString());
            if (ChainCodes != null && ChainCodes.Any())
            {
                urlParams.Add("chainCodes", string.Join(",", ChainCodes));
            }

            if (Amenities != null && Amenities.Any())
            {
                urlParams.Add("amenities", string.Join(",", Amenities));
            }

            if (Ratings != null && Ratings.Any())
            {
                urlParams.Add("ratings", string.Join(",", Ratings));
            }

            if (!string.IsNullOrEmpty(HotelSource))
            {
                urlParams.Add("hotelSource", HotelSource);
            }

            using (HttpContent content = new FormUrlEncodedContent(urlParams))
            {
                return await content.ReadAsStringAsync();
            }
        }
    }

    public class HotelSearchRequest
    {
        [Required]
        [RegularExpression("[a-zA-Z]{3}")]
        [StringLength(3, MinimumLength = 3)]
        public string CityCode { get; set; }

        [Range(1, 300)]
        public int? Radius { get; set; } = 5;

        [DefaultValue("KM")]
        public string RadiusUnit { get; set; } = "KM";

        [MaxLength(99)]
        public List<string>? ChainCodes { get; set; }

        [MaxLength(3)]
        public List<string>? Amenities { get; set; }

        [MaxLength(4)]
        public List<string>? Ratings { get; set; }

        [DefaultValue("ALL")]
        public string HotelSource { get; set; }
        
        public string ToCacheKey()
        {
            return String.Format("{0},{1},{2},{3},{4},{5},{6}", this.CityCode, this.Radius, this.RadiusUnit, this.ChainCodes, this.Amenities, this.Ratings, this.HotelSource);
        }

        public async Task<string> ToUrlParamsString()
        {
            var urlParams = new Dictionary<string, string>();

            urlParams.Add("cityCode", CityCode);
            urlParams.Add("radius", Radius.ToString());
            urlParams.Add("radiusUnit", RadiusUnit.ToString());
            if (ChainCodes != null && ChainCodes.Any())
            {
                urlParams.Add("chainCodes", string.Join(",", ChainCodes));
            }

            if (Amenities != null && Amenities.Any())
            {
                urlParams.Add("amenities", string.Join(",", Amenities));
            }

            if (Ratings != null && Ratings.Any())
            {
                urlParams.Add("ratings", string.Join(",", Ratings));
            }

            if (!string.IsNullOrEmpty(HotelSource))
            {
                urlParams.Add("hotelSource", HotelSource);
            }

            using (HttpContent content = new FormUrlEncodedContent(urlParams))
            {
                return await content.ReadAsStringAsync();
            }
        }
    }

    public class HotelOffersRequest
    {
        public string HotelIds { get; set; }
        public int Adults { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }

        public string ToCacheKey()
        {
            return String.Format("{0},{1},{2},{3}", this.HotelIds, this.Adults, this.CheckInDate, this.CheckOutDate);
        }

        public async Task<string> ToUrlParamsString()
        {
            var urlParams = new Dictionary<string, string>();

            urlParams.Add("hotelIds", HotelIds);
            urlParams.Add("adults", Adults.ToString());
            urlParams.Add("checkInDate", CheckInDate.ToString("yyyy-MM-dd"));
            urlParams.Add("checkOutDate", CheckOutDate.ToString("yyyy-MM-dd"));

            using (HttpContent content = new FormUrlEncodedContent(urlParams))
            {
                return await content.ReadAsStringAsync();
            }
        }
    }

    public class HotelOffersRequestModel
    {
        public string HotelIds { get; set; }
        public int Adults { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }

        public string ToCacheKey()
        {
            return String.Format("{0},{1},{2},{3}", this.HotelIds, this.Adults, this.CheckInDate, this.CheckOutDate);
        }

        public async Task<string> ToUrlParamsString()
        {
            var urlParams = new Dictionary<string, string>();

            urlParams.Add("hotelIds", HotelIds);
            urlParams.Add("adults", Adults.ToString());
            urlParams.Add("checkInDate", CheckInDate.ToString("yyyy-MM-dd"));
            urlParams.Add("checkOutDate", CheckOutDate.ToString("yyyy-MM-dd"));

            using (HttpContent content = new FormUrlEncodedContent(urlParams))
            {
                return await content.ReadAsStringAsync();
            }
        }
    }

}
