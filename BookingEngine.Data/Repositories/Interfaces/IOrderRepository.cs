using BookingEngine.Entities.Models;

namespace BookingEngine.Data.Repositories.Interfaces;

public interface IOrderRepository
{
    Task AddAsync (Order order);
    Task<int> AddOrderAsync(Order order);
}