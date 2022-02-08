using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingEngine.Entities.Models
{
    public class OrderStatus
    {
        [Key]
        public int OrderStatusId { get; set; }
        public string StatusValue { get; set; }
    }
}
