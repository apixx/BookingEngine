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
        /// <summary>
        /// Fetch from Amadeus API, maximum nubmer of items Amadeus API returns in one request is 100 (we always query for this maximum number), so if more items are needed it fetches recursively.
        /// Method returns data from Amadues Search Hotels Api including all preceding data and data for requested page (+ surplus up to 100 from current request).
        /// </summary>
        /// <param name="hotelsSearchRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<HotelByCitySearchResponse> FetchHotelsByCity(HotelSearchRequest hotelsSearchRequest, CancellationToken cancellationToken);
        Task<HotelOffersResponse> FetchHotelOffers(HotelOffersRequest request, CancellationToken cancellationToken);
        Task<HotelBookingAmadeusFetchModel> CreateHotelBooking(HotelBookingRequestDTO hotelBookingUserRequest, CancellationToken cancellationToken);

    }
}
