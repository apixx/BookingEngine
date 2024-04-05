using BookingEngine.Entities.Models;

namespace BookingEngine.Data.Repositories.Interfaces;

public interface IOrderRepository
{
    Task<int> AddAsync(Order order);
}