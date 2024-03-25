using BookingEngine.Data.Repositories.Interfaces;
using BookingEngine.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingEngine.Data.Repositories;

public class OrderRepository : BaseRepository, IOrderRepository
{
    private readonly DatabaseContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    public OrderRepository(DatabaseContext dbContext, IUnitOfWork unitOfWork) : base(dbContext)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
    }

    public async Task AddAsync(Order order)
    {
        await _dbContext.Orders.AddAsync(order);
    }
    public async Task<int> AddOrderAsync(Order order)
    {
        using (var transaction = _dbContext.Database.BeginTransaction())
        {
            try
            {
                // Step 1: Add OrderStatus (it's predefined and won't change)
                var defaultOrderStatus = await _dbContext.OrderStatus
                    .FirstOrDefaultAsync(status => status.StatusValue == "CONFIRMED");

                if (defaultOrderStatus == null)
                {
                    throw new InvalidOperationException("CONFIRMED OrderStatus not found.");
                }

                order.OrderStatusId = defaultOrderStatus.OrderStatusId;

                // Step 2: Add Order
                _dbContext.Orders.Add(order);

                // Step 3: Add OrderItems
                foreach (var orderItem in order.OrderItems)
                {
                    // Link OrderItem with the created Order
                    orderItem.Order = order;
                    // Step 4: Add AssociatedRecordItems
                    foreach (var associatedRecord in orderItem.AssociatedRecords)
                    {
                        _dbContext.AssociatedRecords.Add(associatedRecord);
                    }
                    _dbContext.OrderItems.Add(orderItem);
                }

                await _unitOfWork.CompleteAsync(); // Save changes for the OrderItems

                transaction.Commit();

                return order.Id; // Return the OrderId for reference
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}