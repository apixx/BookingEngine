using AutoMapper;
using BookingEngine.BusinessLogic.Models;
using BookingEngine.BusinessLogic.Services.Interfaces;
using BookingEngine.Data.Repositories.Interfaces;
using BookingEngine.Entities.Models;
using Microsoft.Extensions.Logging;

namespace BookingEngine.BusinessLogic.Services
{
    public class HotelsSearchService : IHotelsSearchService
    {
        private readonly ILogger<HotelsSearchService> _logger;
        private readonly IAmadeusApiServiceProvider _amadeusApiServiceProvider;
        private readonly IAmadeusApiHotelRoomsServiceProvider _amadeusApiHotelRoomsServiceProvider;
        private readonly ISearchRequestRepository _searchRequestRepository;
        private readonly ISearchRequestHotelRepository _searchRequestHotelRepository;
        private readonly IHotelRepository _hotelRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public HotelsSearchService(ILogger<HotelsSearchService> logger, IAmadeusApiServiceProvider amadeusApiServiceProvider, IAmadeusApiHotelRoomsServiceProvider amadeusApiHotelRoomsServiceProvider, ISearchRequestRepository searchRequestRepository,
                IUnitOfWork unitOfWork, ISearchRequestHotelRepository searchRequestHotelRepository, IHotelRepository hotelRepository, IMapper mapper)
        {
            _logger = logger;
            _amadeusApiServiceProvider = amadeusApiServiceProvider;
            _amadeusApiHotelRoomsServiceProvider = amadeusApiHotelRoomsServiceProvider;
            _searchRequestRepository = searchRequestRepository;
            _searchRequestHotelRepository = searchRequestHotelRepository;
            _hotelRepository = hotelRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        //public async Task<HotelRoomsResponse> SearchHotelRooms(HotelRoomsUserRequest hotelRoomsUserRequest, CancellationToken cancellationToken)
        //{
        //    var response = new HotelRoomsResponse(hotelRoomsUserRequest);

        //    var amadeusFetchModel =
        //        await _amadeusApiHotelRoomsServiceProvider.FetchAmadeusHotelRooms(hotelRoomsUserRequest,
        //            cancellationToken);

        //    response.Items = amadeusFetchModel.Item.Offers;

        //    foreach (var offerItem in amadeusFetchModel.Item.Offers)
        //    {
        //        response.Items.Add(offerItem);
        //    }

        //    return response;
        //}

        /// <summary>
        /// Main method for hotels search data, combines logic for getting data from database or fetching it from Amadues Api service or combination of the two
        /// </summary>
        /// <param name="hotelsSearchRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>HotelsSearchAmaduesFetchModel - requested items and nextItemsUrl with url from Amadues Api to get next items</returns>
        public async Task<HotelsSearchResponse> SearchHotels(HotelsSearchUserRequest hotelsSearchRequest, CancellationToken cancellationToken)
        {
            var response = new HotelsSearchResponse(hotelsSearchRequest);

            var tuple = await _searchRequestRepository.GetTupleWithItemsCountAsync(hotelsSearchRequest.CityCode,
                                              hotelsSearchRequest.CheckInDate, hotelsSearchRequest.CheckOutDate, true);

            SearchRequest searchRequestInDb = tuple.Item1;
            int currentlyItemsInDb = tuple.Item2;

            int minimumItemsNeededInDb = hotelsSearchRequest.PageSize * (hotelsSearchRequest.PageOffset + 1);

            bool commonAmadeusNextLinkError = false;

            // there is some data in database, try to get it from db
            if (searchRequestInDb != null)
            {
                // if there is enough items in database, return them
                if (currentlyItemsInDb >= minimumItemsNeededInDb)
                {
                    var requstedItemsFromDb = await _searchRequestHotelRepository.GetForCurrentPageIncludedAsync(searchRequestInDb.SearchRequestId, hotelsSearchRequest.PageSize, hotelsSearchRequest.PageOffset);

                    response.Items = _mapper.Map<List<HotelSearchItemResponse>>(requstedItemsFromDb);
                    response.HasNextPage = currentlyItemsInDb > minimumItemsNeededInDb || !String.IsNullOrEmpty(searchRequestInDb.NextItemsLink);

                    _logger.LogInformation($"All items returned from database. There was no fetching data from API. CityCode: {hotelsSearchRequest.CityCode}, " +
                    $"CheckIn: {hotelsSearchRequest.CheckInDate}, CheckOut: { hotelsSearchRequest.CheckOutDate}, pageSize: {hotelsSearchRequest.PageSize}, pageOffset: {hotelsSearchRequest.PageOffset}");
                    return response;
                }
                // no enough items in database, try to fetch next items from stored NextItemsLink (Amadeus api does not provide classical pagination, returns only nextItemsLink)
                // if there is no NextItemsLink, that is it, page will be empty
                else if (!String.IsNullOrEmpty(searchRequestInDb.NextItemsLink))
                {
                    // get NextItemsLink from db, if that is not enough
                    int moreItemsToFetch = minimumItemsNeededInDb - currentlyItemsInDb;

                    try
                    {
                        var amaduesFetchModelFromNextLink = await _amadeusApiServiceProvider.FetchNextAmadeusHotelsRecursively(searchRequestInDb.NextItemsLink, moreItemsToFetch, cancellationToken);

                        // NextItemsLink that is stored to db is fetched from Amadeus Api Response
                        searchRequestInDb.NextItemsLink = amaduesFetchModelFromNextLink.nextItemsUrl;

                        _searchRequestRepository.Update(searchRequestInDb);
                        await _unitOfWork.CompleteAsync();

                        await this.SaveFetchedItems(amaduesFetchModelFromNextLink, searchRequestInDb.SearchRequestId, cancellationToken);

                        var requstedItemsFromDb = await _searchRequestHotelRepository.GetForCurrentPageIncludedAsync(searchRequestInDb.SearchRequestId, hotelsSearchRequest.PageSize, hotelsSearchRequest.PageOffset);

                        response.Items = _mapper.Map<List<HotelSearchItemResponse>>(requstedItemsFromDb);
                        response.HasNextPage = amaduesFetchModelFromNextLink.Items.Count > minimumItemsNeededInDb || !String.IsNullOrEmpty(searchRequestInDb.NextItemsLink);

                        _logger.LogInformation($"Some items were in database, but other items where needed to fetch from API. CityCode: {hotelsSearchRequest.CityCode}, " +
                            $"CheckIn: {hotelsSearchRequest.CheckInDate}, CheckOut: { hotelsSearchRequest.CheckOutDate}, pageSize: {hotelsSearchRequest.PageSize}, pageOffset: {hotelsSearchRequest.PageOffset}");
                        return response;
                    }
                    // Problem with amadeus api is that NextItemsLink is valid for undefined amount of time, after that returns an error.
                    // Customer support was contacted and confirmed they have problem with pagination (next items link)
                    catch (HttpRequestException ex)
                    {
                        _logger.LogWarning(ex, "Expected Error from Amadeus API they have some problems with pagination for getting next items. Link expires after some undefined time." +
                            "Even Customer Support was contacted. They confirmed they have some problems regarding pagination.");
                        commonAmadeusNextLinkError = true;
                    }

                }
            }
            // no data in database, we need to fetch it from Api and store in db - searchRequestInDb == null
            // Amadeus Api problem with pagination and next link, fetch from beggining - commonAmadeusNextLinkError
            if (searchRequestInDb == null || commonAmadeusNextLinkError)
            {
                var amaduesFetchModel = await _amadeusApiServiceProvider.FetchAmadeusHotels(hotelsSearchRequest, cancellationToken);

                var searchRequest = _mapper.Map<SearchRequest>(hotelsSearchRequest);
                // NextItemsLink that is stored to db is fetched from Amadeus Api Response
                searchRequest.NextItemsLink = amaduesFetchModel.nextItemsUrl;

                await _searchRequestRepository.AddAsync(searchRequest);
                await _unitOfWork.CompleteAsync();

                await this.SaveFetchedItems(amaduesFetchModel, searchRequest.SearchRequestId, cancellationToken);

                var requstedItemsFromDb = await _searchRequestHotelRepository.GetForCurrentPageIncludedAsync(searchRequest.SearchRequestId, hotelsSearchRequest.PageSize, hotelsSearchRequest.PageOffset);

                response.Items = _mapper.Map<List<HotelSearchItemResponse>>(requstedItemsFromDb);
                response.HasNextPage = amaduesFetchModel.Items.Count > minimumItemsNeededInDb || !String.IsNullOrEmpty(searchRequest.NextItemsLink);

                _logger.LogInformation($"All items fetched from API from first to last requested. CityCode: {hotelsSearchRequest.CityCode}, " +
                    $"CheckIn: {hotelsSearchRequest.CheckInDate}, CheckOut: { hotelsSearchRequest.CheckOutDate}, pageSize: {hotelsSearchRequest.PageSize}, pageOffset: {hotelsSearchRequest.PageOffset}");
                return response;
            }

            return response;
        }

        private async Task SaveFetchedItems(HotelsSearchAmadeusFetchModel amadeusFetchModel, int searchRequestId, CancellationToken cancellationToken)
        {
            int fetchedItemsCount = amadeusFetchModel.Items.Count;
            if (amadeusFetchModel.Items != null && fetchedItemsCount > 0)
            {
                foreach (var fetchModelItem in amadeusFetchModel.Items)
                {
                    var searchRequestHotel = _mapper.Map<SearchRequestHotel>(fetchModelItem);
                    var hotel = _mapper.Map<Hotel>(fetchModelItem.Hotel);
                    await _hotelRepository.InsertOrUpdate(hotel);

                    searchRequestHotel.SearchRequestId = searchRequestId;
                    searchRequestHotel.HotelId = hotel.HotelId;

                    _searchRequestHotelRepository.Update(searchRequestHotel);
                }
                await _unitOfWork.CompleteAsync();
            }
        }

    }
}
