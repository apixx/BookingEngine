using BookingEngine.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingEngine.Data.Repositories.Interfaces
{
    public interface ISearchRequestHotelRepository
    {
        Task<List<SearchRequestHotel>> GetForCurrentPageIncludedAsync(int searchRequestId, int pageSize, int pageOffset);
        Task AddAsync(SearchRequestHotel searchRequestHotel);
        void Update(SearchRequestHotel searchRequestHotel);
    }
}
