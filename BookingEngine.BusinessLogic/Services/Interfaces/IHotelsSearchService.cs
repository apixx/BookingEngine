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
        /// <summary>
        /// Main method for hotels search data, combines logic for getting data from database or fetching it from Amadeus Api service or combination of the two
        /// </summary>
        /// <param name="hotelsSearchRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>HotelsSearchAmadeusFetchModel - requested items and nextItemsUrl with url from Amadeus Api to get next items</returns>
        Task<HotelOffersResponse> SearchHotels(HotelSearchRequestModel hotelsSearchRequest, CancellationToken cancellationToken);
    }
}
