using BookingEngine.BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingEngine.BusinessLogic.Services.Interfaces
{
    public interface IHotelsSearchService
    {
        Task<HotelsSearchResponse> SearchHotels(HotelsSearchUserRequest hotelsSearchRequest, CancellationToken cancellationToken);
    }
}
