using BookingEngine.Data.Repositories.Interfaces;
using BookingEngine.Entities.Models;

namespace BookingEngine.Data.Repositories;

public class OrderRepository : BaseRepository, IOrderRepository
{
    public OrderRepository(DatabaseContext dbContext) : base(dbContext)
    {
    }

    public async Task AddAsync(Order order)
    {
        await _dbContext.Orders.AddAsync(order);
    }
}