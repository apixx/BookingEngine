using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingEngine.Entities.Models
{
    public class OrderItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        //JsonResponse from API
        [ForeignKey("OrderId")]
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }

        public string Type { get; set; }
        public string BookingItemId { get; set; }
        public string ProviderConfirmationId { get; set; }
        public virtual List<AssociatedRecordItem> AssociatedRecords { get; set; } = new List<AssociatedRecordItem>();
    }
}
