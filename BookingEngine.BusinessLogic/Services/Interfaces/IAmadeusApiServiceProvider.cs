using BookingEngine.BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingEngine.BusinessLogic.Services.Interfaces
{
    public interface IAmadeusApiServiceProvider
    {
        Task<HotelsSearchAmadeusFetchModel> FetchAmadeusHotels(HotelsSearchUserRequest hotelsSearchRequest, CancellationToken cancellationToken);
        /// <summary>
        /// Returns next items from link that was stored in db for search request. Keeps getting recursively until at least "itemsToFetch" are fetched 
        /// </summary>
        /// <param name="uri">NextItemsLink that is stored in database for certain SearchRequest</param>
        /// <param name="itemsToFetch">keeps fetching from api, until at least this number of items is fetched</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<HotelsSearchAmadeusFetchModel> FetchNextAmadeusHotelsRecursively(string url, int itemsToFetch, CancellationToken cancellationToken);
    }
}
