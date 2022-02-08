using BookingEngine.Data.Repositories.Interfaces;
using BookingEngine.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingEngine.Data.Repositories
{
    public class SearchRequestHotelRepository : BaseRepository, ISearchRequestHotelRepository
    {
        public SearchRequestHotelRepository(DatabaseContext dbContext) : base(dbContext) { }

        public async Task<List<SearchRequestHotel>> GetForCurrentPageIncludedAsync(int searchRequestId, int pageSize, int pageOffset)
        {
            int skip = pageOffset == 0 ? 0 : pageOffset * pageSize;
            return await _dbContext.SearchRequestHotels
                .Where(x => x.SearchRequestId == searchRequestId)
                .Include(x => x.Hotel)
                .OrderBy(x => x.Distance)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task AddAsync(SearchRequestHotel searchRequestHotel)
        {
            await _dbContext.SearchRequestHotels.AddAsync(searchRequestHotel);
        }

        public void Update(SearchRequestHotel searchRequestHotel)
        {
            _dbContext.SearchRequestHotels.Update(searchRequestHotel);
        }
    }
}
