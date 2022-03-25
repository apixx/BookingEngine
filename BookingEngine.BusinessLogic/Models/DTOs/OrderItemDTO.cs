using System.ComponentModel.DataAnnotations;
using BookingEngine.Entities.Models;

namespace BookingEngine.BusinessLogic.Models.DTOs;

public class OrderItemDTO
{
    [Key]
    public int OrderItemId { get; set; }
    //JsonResponse from API
    public string ProductItem { get; set; }
    public int OrderId { get; set; }
    public virtual Order Order { get; set; }
}