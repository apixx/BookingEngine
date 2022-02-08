using BookingEngine.BusinessLogic.Models;
using BookingEngine.BusinessLogic.Models.AmadeusApiCustomModels;
using BookingEngine.BusinessLogic.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using BookingEngine.BusinessLogic.Models.AmadeusApiCustomModels.Hotel.HotelSearch;
using Newtonsoft.Json;

namespace BookingEngine.BusinessLogic.Services
{
    public class AmadeusApiHotelRoomsServiceProvider : IAmadeusApiHotelRoomsServiceProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AmadeusApiHotelRoomsResponse> _logger;
        private readonly IAmadeusTokenService _amadeusTokenService;
        private readonly IProcessApiResponse _processApiResponse;

        public AmadeusApiHotelRoomsServiceProvider(HttpClient httpClient, ILogger<AmadeusApiHotelRoomsResponse> logger, IAmadeusTokenService amadeusTokenService, IProcessApiResponse processApiResponse)
        {
            _httpClient = httpClient;
            _amadeusTokenService = amadeusTokenService;
            _processApiResponse = processApiResponse;
            _logger = logger;
        }

        public async Task<HotelRoomsAmadeusFetchModel> FetchAmadeusHotelRooms(HotelRoomsUserRequest hotelRoomsSearchRequest, CancellationToken cancellationToken)
        {
            HotelRoomsAmadeusFetchModel amadeusFetchModel = new HotelRoomsAmadeusFetchModel();
            //amadeusFetchModel.Item = new AmadeusApiHotelRoomsResponseItem();
            amadeusFetchModel.Item = new AmadeusApiHotelRoomsResponseItem();

            string tokenString = await _amadeusTokenService.GetAmadeusToken(cancellationToken);

            _httpClient.DefaultRequestHeaders.Add("Accept", MediaTypeNames.Application.Json);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenString);

            var requestHotelRoomsModel = new AmadeusApiHotelRoomsRequest(hotelRoomsSearchRequest.HotelId,
                hotelRoomsSearchRequest.CheckInDate, hotelRoomsSearchRequest.CheckOutDate,
                hotelRoomsSearchRequest.Adults);

            var urlParams = await requestHotelRoomsModel.ToUrlParamsString();

            // https://test.api.amadeus.com
            HttpResponseMessage response =
                await _httpClient.GetAsync("https://test.api.amadeus.com/v2/shopping/hotel-offers/by-hotel" + "?" + urlParams, cancellationToken);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errors = await _processApiResponse.ProcessError<AmadeusApiErrorResponse>(response);
                var firstError = errors.Errors.FirstOrDefault();
                throw new HttpRequestException(firstError.Code + " - " + firstError.Title);
            }

            response.EnsureSuccessStatusCode();
            var checkResponse = await response.Content.ReadAsStringAsync();

            var currentRoomsResponse = 
                await ProcessResponse(response);
            //  _processApiResponse.ProcessResponse<AmadeusApiHotelRoomsResponse>(response);


            _logger.LogInformation("Successful in first request from Amadeus API");

            amadeusFetchModel.Item = currentRoomsResponse.Data;
            //amadeusFetchModel.Item.Offers.AddRange(currentRoomsResponse.Data.Offers);


            int? currentItemsReturnedCount = amadeusFetchModel?.Item?.Offers?.Count;
            _logger.LogInformation("Succcessful in getting data from Amadeus API. Returned Search Hotels items: " + currentItemsReturnedCount);

            return amadeusFetchModel;
        }

        public async Task<RoomDetailsAmadeusFetchModel> FetchAmadeusRoomDetails(RoomDetailsUserRequest roomDetailsRequest, CancellationToken cancellationToken)
        {
            RoomDetailsAmadeusFetchModel amadeusFetchModel = new RoomDetailsAmadeusFetchModel();
            amadeusFetchModel.Item = new AmadeusApiRoomDetailsResponseItem();

            string tokenString = await _amadeusTokenService.GetAmadeusToken(cancellationToken);

            _httpClient.DefaultRequestHeaders.Add("Accept", MediaTypeNames.Application.Json);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenString);

            var requestRoomDetailsModel = new AmadeusApiRoomDetailsRequest(roomDetailsRequest.OfferId);

            //var urlParams = await requestRoomDetailsModel.ToUrlParamsString();

            // https://test.api.amadeus.com
            HttpResponseMessage response =
                await _httpClient.GetAsync("https://test.api.amadeus.com/v2/shopping/hotel-offers/" + requestRoomDetailsModel.OfferId, cancellationToken);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errors = await _processApiResponse.ProcessError<AmadeusApiErrorResponse>(response);
                var firstError = errors.Errors.FirstOrDefault();
                throw new HttpRequestException(firstError.Code + " - " + firstError.Title);
            }

            response.EnsureSuccessStatusCode();

            var currentRoomsResponse = // ovde funkcionira preku _processApiResponse, sredi na drugite isto
                await _processApiResponse.ProcessResponse<AmadeusApiRoomDetailsResponse>(response);

            _logger.LogInformation("Successful in first request from Amadeus API");

            amadeusFetchModel.Item = currentRoomsResponse.Data;

            int? currentItemsReturnedCount = amadeusFetchModel?.Item?.Offers?.Count;
            _logger.LogInformation("Succcessful in getting data from Amadeus API. Returned Search Hotels items: " + currentItemsReturnedCount);

            return amadeusFetchModel;
        }

        // AmadeusApiHotelRoomsResponse
        public async Task<AmadeusApiHotelRoomsResponse> ProcessResponse(HttpResponseMessage response)
        {
            try
            {
                var contentStream = await response.Content.ReadAsStreamAsync();

                using var streamReader = new StreamReader(contentStream);
                using var jsonReader = new JsonTextReader(streamReader);

                JsonSerializer serializer = new JsonSerializer();

                var searchResponse = serializer.Deserialize<AmadeusApiHotelRoomsResponse>(jsonReader);

                _logger.LogInformation("Response from Amadeus api succesfull. Model fetched: " + searchResponse.ToString());

                return searchResponse;
            }
            catch (Exception e)
            {
                throw new Exception("Could not parse JSON response for Amadeus hotels search.", e);
            }
        }
    }
}
