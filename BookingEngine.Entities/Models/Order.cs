using System.ComponentModel.DataAnnotations;
using BookingEngine.Entities.Models.Authentication;

namespace BookingEngine.Entities.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public int SequenceNumber { get; set; }
        public DateTime DateCreated { get; set; }
        public int OrderStatusId { get; set; }
        public virtual OrderStatus OrderStatus { get; set; }
        // orderItem foreign key is added
        public int OrderItemId { get; set; }
        // orderItem entity is added
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}