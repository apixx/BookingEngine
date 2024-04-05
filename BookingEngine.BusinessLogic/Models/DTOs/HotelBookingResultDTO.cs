using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingEngine.BusinessLogic.Models
{
    public class HotelBookingResultDTO
    {
        public List<HotelBookingItemDTO> Bookings { get; set; }
        public string HotelName { get; set; }
        public string City { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
        public decimal TotalPrice { get; set; }
        public string Currency { get; set; }
        public string BookingStatus { get; set; } // E.g., "Confirmed", "Pending", etc.
    }

    public class HotelBookingItemDTO
    {
        public string Type { get; set; }
        public string BookingItemId { get; set; }
        public string ProviderConfirmationId { get; set; }
        public List<AssociatedRecordDTO> AssociatedRecords { get; set; }
    }

    public class AssociatedRecordDTO
    {
        public string Reference { get; set; }
        public string OriginSystemCode { get; set; }
    }
}
