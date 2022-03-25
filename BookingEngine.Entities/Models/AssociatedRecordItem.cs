using System.ComponentModel.DataAnnotations;

namespace BookingEngine.Entities.Models;

public class AssociatedRecordItem
{
    [Key]
    public int Id { get; set; }
    public int OrderItemId { get; set; }
    public string Reference { get; set; }
    public string OriginSystemCode { get; set; }
}