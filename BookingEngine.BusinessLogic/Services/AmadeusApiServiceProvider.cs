using BookingEngine.BusinessLogic.Models;
using BookingEngine.BusinessLogic.Models.AmadeusApiCustomModels;
using BookingEngine.BusinessLogic.Models.AmadeusApiCustomModels.Hotel.HotelSearch;
using BookingEngine.BusinessLogic.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;

namespace BookingEngine.BusinessLogic.Services
{
    public class AmadeusApiServiceProvider : IAmadeusApiServiceProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AmadeusApiServiceProvider> _logger;
        private readonly IAmadeusTokenService _amadeusTokenService;
        private readonly IProcessApiResponse _processApiResponse;

        public AmadeusApiServiceProvider(HttpClient httpClient, ILogger<AmadeusApiServiceProvider> logger, IAmadeusTokenService amadeusTokenService, IProcessApiResponse processApiResponse)
        {
            _httpClient = httpClient;
            _amadeusTokenService = amadeusTokenService;
            _logger = logger;
            _processApiResponse = processApiResponse;
        }


        public async Task<HotelByCitySearchResponse> FetchHotelsByCity(HotelByCitySearchRequest request, CancellationToken cancellationToken)
        {
            //HotelByCitySearchResponse amadeusFetchModel = new HotelByCitySearchResponse();

            string tokenString = await _amadeusTokenService.GetAmadeusToken(cancellationToken);

            _httpClient.DefaultRequestHeaders.Add("Accept", MediaTypeNames.Application.Json);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenString);

            // Request to Amadeus API is made in a way that Amadeus API returns maximum possible number of items it can (it seems that limit is 100 items per request)
            //var requestHotelsModel = new AmadeusApiHotelsSearchRequest(hotelsSearchRequest.CityCode, hotelsSearchRequest.CheckInDate, hotelsSearchRequest.CheckOutDate);

            // Flag - if our user requests certain page, all preceeding data should be fetched so they can be stored in db
            //int minimumItemsToReturn = request.PageSize * (request.PageOffset + 1);
            int currentItemsReturnedCount;

            var urlParams = await request.ToUrlParamsString();

            HttpResponseMessage response = await _httpClient.GetAsync("/v1/reference-data/locations/hotels/by-city" + "?" + urlParams, cancellationToken);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errors =
                    await _processApiResponse.ProcessError<ErrorResponse>(response);
                var firstError = errors.Errors.FirstOrDefault();
                throw new HttpRequestException(firstError.Code + " - " + firstError.Title);
            }

            response.EnsureSuccessStatusCode();
            var currentHotelsResponse =
                await _processApiResponse.ProcessResponse<HotelByCitySearchResponse>(response);
            _logger.LogInformation("Successful in first request from Amadeus API");

            currentItemsReturnedCount = currentHotelsResponse.Hotels.Count();
            //amadeusFetchModel.Items.AddRange(currentHotelsResponse.Data);

            //currentItemsReturnedCount = amadeusFetchModel.Items.Count;

            //int iterationCount = 1;
            //bool hasMoreItems = currentHotelsResponse.Meta != null && currentHotelsResponse.Meta.Links != null && !String.IsNullOrEmpty(currentHotelsResponse.Meta.Links.Next);
            //string nextItemsLink = hasMoreItems ? currentHotelsResponse.Meta.Links.Next : null;

            //while(currentItemsReturnedCount < minimumItemsToReturn && hasMoreItems)
            //{
            //    nextItemsLink = null;
            //    hasMoreItems = currentHotelsResponse.Meta != null && currentHotelsResponse.Meta.Links != null && !String.IsNullOrEmpty(currentHotelsResponse.Meta.Links.Next);

            //    if(hasMoreItems)
            //    {
            //        var nextAmadeusHotelsResponse = await FetchNextAmadeusHotels(currentHotelsResponse.Meta.Links.Next, cancellationToken);
            //        nextItemsLink = currentHotelsResponse.Meta.Links.Next;

            //        currentHotelsResponse = await FetchNextAmadeusHotels(currentHotelsResponse.Meta.Links.Next, cancellationToken);

            //        _logger.LogInformation("Iteration count for getting next items: " + iterationCount);
            //        iterationCount++;

            //        amadeusFetchModel.Items.AddRange(currentHotelsResponse.Data);
            //    }

            //    currentItemsReturnedCount = amadeusFetchModel.Items.Count;
            //}

            _logger.LogInformation("Succcessful in getting data from Amadeus API. Returned Search Hotels items: " + currentItemsReturnedCount);
            //amadeusFetchModel.nextItemsUrl = nextItemsLink;
            return currentHotelsResponse;
            
        }


        /// <summary>
        /// Returns next items from link that was stored in db for search request. Keeps getting recursively until at least "itemsToFetch" are fetched 
        /// </summary>
        /// <param name="uri">NextItemsLink that is stored in database for certain SearchRequest</param>
        /// <param name="itemsToFetch">keeps fetching from api, until at least this number of items is fetched</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HotelsSearchAmadeusFetchModel> FetchNextAmadeusHotelsRecursively(string uri, int itemsToFetch, CancellationToken cancellationToken)
        {
            HotelsSearchAmadeusFetchModel amadeusFetchModel = new HotelsSearchAmadeusFetchModel();
            amadeusFetchModel.Items = new List<AmadeusApiHotelsSearchResponseItem>();

            // Fetch next amadeus hotels
            var currentHotelsResponse = await FetchNextAmadeusHotels(uri, cancellationToken);
            _logger.LogInformation("Successful in first request from Amadeus API - (method FetchNextAmadeusHotelsRecursively");

            // Add hotels in amadeusFetchModel.Items list
            amadeusFetchModel.Items.AddRange(currentHotelsResponse.Data);

            // Count the returned hotels
            int currentItemsReturnedCount = amadeusFetchModel.Items.Count;

            int iterationCount = 1;

            // Check for more items
            bool hasMoreItems = currentHotelsResponse.Meta != null && currentHotelsResponse.Meta.Links != null && !String.IsNullOrEmpty(currentHotelsResponse.Meta.Links.Next);
            
            // Get the nextItemsLink
            string? nextItemsLink = hasMoreItems ? currentHotelsResponse.Meta.Links.Next : null;

            while (currentItemsReturnedCount < itemsToFetch && hasMoreItems)
            {
                nextItemsLink = null;
                hasMoreItems = currentHotelsResponse.Meta != null && currentHotelsResponse.Meta.Links != null && !String.IsNullOrEmpty(currentHotelsResponse.Meta.Links.Next);
                
                if(hasMoreItems)
                {
                    nextItemsLink = currentHotelsResponse.Meta.Links.Next;

                    currentHotelsResponse = await FetchNextAmadeusHotels(currentHotelsResponse.Meta.Links.Next, cancellationToken);

                    _logger.LogInformation("Iteration (method FetchNextAmadeusRecursively) count for getting next items: " + iterationCount);
                    iterationCount++;

                    amadeusFetchModel.Items.AddRange(currentHotelsResponse.Data);
                }

                currentItemsReturnedCount = amadeusFetchModel.Items.Count;
            }

            _logger.LogInformation("Successful in getting data from Amadeus API (method FetchNextAmadeusGotelsRecursively). Returned Search Hotels items: " + currentItemsReturnedCount);

            amadeusFetchModel.nextItemsUrl = nextItemsLink;
            
            return amadeusFetchModel;

        }

        /// <summary>
        /// Sends a GET to API and fetches hotels from response.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>AmadeusApiHotelsSearchResponse object - </returns>
        /// <exception cref="HttpRequestException"></exception>
        private async Task<AmadeusApiHotelsSearchResponse> FetchNextAmadeusHotels(string uri, CancellationToken cancellationToken)
        {
            // Get AmadeusAPI Access Token
            string tokenString = await _amadeusTokenService.GetAmadeusToken(cancellationToken);

            // Set the request header for Accept to JSON, and Authorization parameter to Bearer with the passed Access Token
            _httpClient.DefaultRequestHeaders.Add("Accept", MediaTypeNames.Application.Json);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenString);

            // Send asynchronous GET client request to the uri parameter with a passed cancellationToken.
            HttpResponseMessage response = await _httpClient.GetAsync(uri, cancellationToken);

            // If the response is invalid, call ProcessError to deserialize the error object
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var errors =
                    await _processApiResponse.ProcessError<AmadeusApiErrorResponse>(response);
                var firstError = errors.Errors[0];
                throw new HttpRequestException(firstError.Code + " - " + firstError.Title);
            }

            // Check if the HTTP response is successful
            response.EnsureSuccessStatusCode();

            // Deserialize the valid response to object.
            var amadeusHotelsResponse =
                await _processApiResponse.ProcessResponse<AmadeusApiHotelsSearchResponse>(response);

            return amadeusHotelsResponse;
        }
    }
}
