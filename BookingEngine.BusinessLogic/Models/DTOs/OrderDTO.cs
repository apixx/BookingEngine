using BookingEngine.Entities.Models.Authentication;
using BookingEngine.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingEngine.BusinessLogic.Models
{
    public class OrderDTO
    {
        public int Id { get; set; }
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
