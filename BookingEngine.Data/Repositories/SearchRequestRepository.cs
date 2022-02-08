using BookingEngine.Data.ConfigAppSettings;
using BookingEngine.Data.Repositories.Interfaces;
using BookingEngine.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingEngine.Data.Repositories
{
    public class SearchRequestRepository : BaseRepository, ISearchRequestRepository
    {
        private readonly IOptionsMonitor<DatabaseOptions> _databaseOptions;

        public SearchRequestRepository(DatabaseContext context, IOptionsMonitor<DatabaseOptions> databaseOptions) : base(context)
        {
            _databaseOptions = databaseOptions;
        }

        public async Task<Tuple<SearchRequest, int>> GetTupleWithItemsCountAsync(string cityCode, DateTime checkInDate, DateTime checkOutDate, bool onlyValid = true)
        {
            var searchRequest = await _dbContext.SearchRequests
                                 .Where(x => x.CityCode == cityCode && x.CheckInDate == checkInDate && x.CheckOutDate == checkOutDate && (onlyValid == false || x.ValidUntil > DateTime.Now))
                                 .OrderByDescending(x => x.ValidUntil)
                                 .FirstOrDefaultAsync();
            int count;
            if (searchRequest == null)
            {
                count = 0;
            }
            else
            {
                count = _dbContext.SearchRequestHotels
                        .AsNoTracking()
                        .Count(x => x.SearchRequestId == searchRequest.SearchRequestId);
            }


            return new Tuple<SearchRequest, int>(searchRequest, count);

        }

        public async Task<SearchRequest> GetWithSearchRequestHotelsIncludeAsync(string cityCode, DateTime checkInDate, DateTime checkOutDate, bool onlyValid = true)
        {
            return await _dbContext.SearchRequests
                                 .Where(x => x.CityCode == cityCode && x.CheckInDate == checkInDate && x.CheckOutDate == checkOutDate && (onlyValid == false || x.ValidUntil > DateTime.Now))
                                 .OrderByDescending(x => x.ValidUntil)
                                 .Include(x => x.SearchRequestHotels.OrderBy(srh => srh.Distance))
                                 .ThenInclude(srh => srh.Hotel)
                                 .FirstOrDefaultAsync();

        }

        public async Task AddAsync(SearchRequest searchRequest)
        {
            int validForInMinutes = _databaseOptions.CurrentValue.SearchRequestValidForMinutes;
            searchRequest.ValidUntil = DateTime.Now.AddMinutes(validForInMinutes);
            await _dbContext.SearchRequests.AddAsync(searchRequest);
        }

        public async Task<SearchRequest> FindByIdAsync(int id)
        {
            return await _dbContext.SearchRequests.FindAsync(id);
        }

        public void Update(SearchRequest searchRequest)
        {
            _dbContext.SearchRequests.Update(searchRequest);
        }

    }
}
