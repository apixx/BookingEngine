using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingEngine.Entities.Models
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }
        //JsonResponse from API
        public string ProductItem { get; set; }
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }

        public string Type { get; set; }
        public string BookingItemId { get; set; }
        public string ProviderConfirmationId { get; set; }
        public string SelfUri { get; set; }

        // Hotel info
        public string HotelId { get; set; }
        public string HotelName { get; set; }
        public string HotelCityCode { get; set; }

        // Offer info
        public int OfferId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int RoomQuantity { get; set; }
        public string BoardType { get; set; }
        public string CommisionPercentage { get; set; }
        public string CommisionAmount { get; set; }
        public float TotalPrice { get; set; }
        public string Currency { get; set; }
    }
}
