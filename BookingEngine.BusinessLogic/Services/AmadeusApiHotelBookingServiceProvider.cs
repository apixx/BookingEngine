using BookingEngine.BusinessLogic.Models;
using BookingEngine.BusinessLogic.Models.AmadeusApiCustomModels;
using BookingEngine.BusinessLogic.Models.AmadeusApiCustomModels.Hotel.Booking;
using BookingEngine.BusinessLogic.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using BookingEngine.Data.Repositories.Interfaces;
using Newtonsoft.Json.Serialization;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace BookingEngine.BusinessLogic.Services;

public class AmadeusApiHotelBookingServiceProvider : IAmadeusApiHotelBookingServiceProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AmadeusApiHotelBookingResponse> _logger;
    private readonly IAmadeusTokenService _amadeusTokenService;
    private readonly IProcessApiResponse _processApiResponse;
    //private readonly IOrderRepository _orderRepository;

    public AmadeusApiHotelBookingServiceProvider(HttpClient httpClient, ILogger<AmadeusApiHotelBookingResponse> logger, IAmadeusTokenService amadeusTokenService, IProcessApiResponse processApiResponse) //IOrderRepository orderRepository)
    {
        _httpClient = httpClient;
        _logger = logger;
        _processApiResponse = processApiResponse;
        _amadeusTokenService = amadeusTokenService;
        //_orderRepository = orderRepository;
    }

    public async Task CompleteBooking(HotelBookingUserRequest bookingUserRequest, RoomDetailsAmadeusFetchModel roomDetailsResponse, HotelBookingAmadeusFetchModel bookingResponse, DateTime checkInDate, DateTime checkOutDate, CancellationToken cancellationToken)
    {

        // This goes in BLL
        //var orderItem =
        //    _mapper.Map<OrderItem>(response.Item.Data.FirstOrDefault());
        //var associatedRecordItem =
        //    _mapper.Map<AssociatedRecordItem>(response.Item.Data.FirstOrDefault().AssociatedRecords.FirstOrDefault());

        // TODO: Store the reservation in DB

        throw new NotImplementedException();
    }

    public async Task<HotelBookingAmadeusFetchModel> FetchAmadeusHotelBooking(HotelBookingUserRequest hotelBookingUserRequest, CancellationToken cancellationToken)
    {
        HotelBookingAmadeusFetchModel amadeusFetchModel = new HotelBookingAmadeusFetchModel();

        string tokenString = await _amadeusTokenService.GetAmadeusToken(cancellationToken);

        _httpClient.DefaultRequestHeaders.Add("Accept", MediaTypeNames.Application.Json);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenString);
        

        var userReq = hotelBookingUserRequest.HotelBookingRequest;


        var requestHotelBookingModel = new AmadeusApiHotelBookingRequest(userReq);

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
            var errors = await _processApiResponse.ProcessError<AmadeusApiErrorResponse>(response);
            var firstError = errors.Errors.FirstOrDefault();
            throw new HttpRequestException(firstError.Code + " - " + firstError.Title);
        }

        // Check the response before processing - Debugging purpose
        //var checkResponse = 
        //    await response.Content.ReadAsStringAsync();
        
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