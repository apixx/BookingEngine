using BookingEngine.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingEngine.Data.Repositories.Interfaces
{
    public interface ISearchRequestRepository
    {
        Task<Tuple<SearchRequest, int>> GetTupleWithItemsCountAsync(string cityCode, DateTime checkInDate, DateTime checkOutDate, bool onlyValid = true);
        Task<SearchRequest> GetWithSearchRequestHotelsIncludeAsync(string cityCode, DateTime checkInDate, DateTime checkOutDate, bool onlyValid = true);
        Task AddAsync(SearchRequest searchRequest);
        Task<SearchRequest> FindByIdAsync(int id);
        void Update(SearchRequest searchRequest);
    }
}
