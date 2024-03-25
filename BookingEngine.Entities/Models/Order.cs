using System.ComponentModel.DataAnnotations;
using BookingEngine.Entities.Models.Authentication;

namespace BookingEngine.Entities.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public int SequenceNumber { get; set; }
        public DateTime DateCreated { get; set; }

        // Offer info
        public string OfferId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int RoomQuantity { get; set; }
        public float TotalPrice { get; set; }
        public string Currency { get; set; }
        public string? BoardType { get; set; }
        public int Adults { get; set; }

        // Hotel info
        public string HotelId { get; set; }
        public string HotelName { get; set; }
        public string HotelCityCode { get; set; }

        public int OrderStatusId { get; set; }
        public virtual OrderStatus OrderStatus { get; set; }
        // orderItem entity is added
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}