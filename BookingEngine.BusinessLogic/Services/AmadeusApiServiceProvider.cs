using BookingEngine.BusinessLogic.Models;
using BookingEngine.BusinessLogic.Models.AmadeusApiModels.Hotel.Booking;
using BookingEngine.BusinessLogic.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using AutoMapper;

namespace BookingEngine.BusinessLogic.Services
{
    public class AmadeusApiServiceProvider : IAmadeusApiServiceProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AmadeusApiServiceProvider> _logger;
        private readonly IAmadeusTokenService _amadeusTokenService;
        private readonly IProcessApiResponse _processApiResponse;
        private readonly IMapper _mapper;

        public AmadeusApiServiceProvider(HttpClient httpClient, ILogger<AmadeusApiServiceProvider> logger, IAmadeusTokenService amadeusTokenService, IProcessApiResponse processApiResponse, IMapper mapper)
        {
            _httpClient = httpClient;
            _amadeusTokenService = amadeusTokenService;
            _logger = logger;
            _processApiResponse = processApiResponse;
            _mapper = mapper;
        }

        public async Task<HotelByCitySearchResponse> FetchHotelsByCity(HotelSearchRequest request, CancellationToken cancellationToken)
        {
            string tokenString = await _amadeusTokenService.GetAmadeusToken(cancellationToken);

            _httpClient.DefaultRequestHeaders.Add("Accept", MediaTypeNames.Application.Json);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenString);

            int? currentItemsReturnedCount;

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

            currentItemsReturnedCount = currentHotelsResponse?.Data?.Count();
            
            var hotelCount = currentItemsReturnedCount == null ? 0 : currentItemsReturnedCount;
            _logger.LogInformation($"Succcessful in getting data from Amadeus API. Returned Search Hotels items: {hotelCount}");

            return currentHotelsResponse;

        }
        public async Task<HotelOffersResponse> FetchHotelOffers(HotelOffersRequest request, CancellationToken cancellationToken)
        {
            string tokenString = await _amadeusTokenService.GetAmadeusToken(cancellationToken);

            _httpClient.DefaultRequestHeaders.Add("Accept", MediaTypeNames.Application.Json);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenString);

            int? currentItemsReturnedCount;

            var urlParams = await request.ToUrlParamsString();

            HttpResponseMessage response = await _httpClient.GetAsync("/v3/shopping/hotel-offers" + "?" + urlParams, cancellationToken);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errors =
                    await _processApiResponse.ProcessError<ErrorResponse>(response);
                var firstError = errors.Errors.FirstOrDefault();
                throw new HttpRequestException(firstError.Code + " - " + firstError.Title);
            }

            response.EnsureSuccessStatusCode();
            var currentHotelsResponse =
                await _processApiResponse.ProcessResponse<HotelOffersResponse>(response);
            _logger.LogInformation("Successful in first request from Amadeus API");

            currentItemsReturnedCount = currentHotelsResponse?.Data?.Count();
            
            var hotelCount = currentItemsReturnedCount == null ? 0 : currentItemsReturnedCount;
            _logger.LogInformation($"Succcessful in getting data from Amadeus API. Returned Search Hotels items: {hotelCount}");
            
            return currentHotelsResponse;

        }
        public async Task<HotelBookingAmadeusFetchModel> CreateHotelBooking(HotelBookingRequestDTO hotelBookingRequest, CancellationToken cancellationToken)
        {
            HotelBookingAmadeusFetchModel amadeusFetchModel = new HotelBookingAmadeusFetchModel();

            string tokenString = await _amadeusTokenService.GetAmadeusToken(cancellationToken);

            _httpClient.DefaultRequestHeaders.Add("Accept", MediaTypeNames.Application.Json);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenString);


            var userReq = hotelBookingRequest;


            var requestHotelBookingModel = new AmadeusApiHotelBookingRequest(_mapper.Map<AmadeusApiHotelBookingRequestItem>(userReq));

            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var json = JsonConvert.SerializeObject(requestHotelBookingModel, new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            });
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response =
                await _httpClient.PostAsync("https://test.api.amadeus.com/v1/booking/hotel-bookings", data,
                    cancellationToken);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errors = await _processApiResponse.ProcessError<ErrorResponse>(response);
                var firstError = errors.Errors.FirstOrDefault();
                throw new HttpRequestException(firstError.Code + " - " + firstError.Title);
            }

            response.EnsureSuccessStatusCode();

            var currentBookingResponse =
               await _processApiResponse.ProcessResponse<AmadeusApiHotelBookingResponse>(response);

            _logger.LogInformation("Successful in first POST request from Amadeus API");

            amadeusFetchModel.Item = currentBookingResponse;

            int currentItemsReturnedCount = amadeusFetchModel.Item.Data.Count;
            _logger.LogInformation("Succcessful in getting data from Amadeus API. Returned Hotel Booking items: " + currentItemsReturnedCount);

            return amadeusFetchModel;
        }
    }
}
