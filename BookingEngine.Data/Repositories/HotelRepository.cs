using BookingEngine.Data.Repositories.Interfaces;
using BookingEngine.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingEngine.Data.Repositories
{
    public class HotelRepository : BaseRepository, IHotelRepository
    {
        public HotelRepository(DatabaseContext dbContext) : base(dbContext) { }

        public async Task InsertOrUpdate(Hotel hotel)
        {
            var existingHotel = _dbContext.Hotels.Find(hotel.HotelId);
            if (existingHotel == null)
            {
                await _dbContext.Hotels.AddAsync(hotel);
            }
            else
            {
                _dbContext.Entry(existingHotel).CurrentValues.SetValues(hotel);
            }
        }
    }
}
