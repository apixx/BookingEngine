using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BookingEngine.BusinessLogic.Models.AmadeusApiCustomModels.Hotel.Booking
{
    public class AmadeusApiHotelBookingResponse
    {
        [JsonPropertyName("data")]
        public List<HotelBookingItem> Data { get; set; }
    }
    public class HotelBookingItem
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public string ProviderConfirmationId { get; set; }
        public List<AssociatedRecord> AssociatedRecords { get; set; }
    }

    public class AssociatedRecord
    {
        public string Reference { get; set; }
        public string OriginSystemCode { get; set; }
    }
}
