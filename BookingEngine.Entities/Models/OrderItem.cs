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
    }
}
