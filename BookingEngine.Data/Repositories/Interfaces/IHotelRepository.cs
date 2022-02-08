using BookingEngine.Entities.Models;

namespace BookingEngine.Data.Repositories.Interfaces
{
    public interface IHotelRepository
    {
        Task InsertOrUpdate(Hotel hotel);
    }
}
