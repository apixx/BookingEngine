using BookingEngine.Entities.Models;

namespace BookingEngine.Data.Repositories.Interfaces;

public interface IOrderRepository
{
    Task AddAsync (Order order);
}