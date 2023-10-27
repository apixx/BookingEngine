using BookingEngine.BusinessLogic.Models;
using BookingEngine.BusinessLogic.Services.Interfaces;
using BookingEngine.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Security.Claims;
using AutoMapper;
using BookingEngine.BusinessLogic.Models.AmadeusApiCustomModels;
using BookingEngine.Entities.Models;
using Newtonsoft.Json;
using BookingEngine.Entities.Models.Authentication;

namespace BookingEngine.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly ILogger<HotelsController> _logger;
        private readonly IHotelsSearchService _hotelsSearchService;
        private readonly IAmadeusApiHotelRoomsServiceProvider _hotelRoomsService;
        private readonly IAmadeusApiHotelBookingServiceProvider _bookingService;
        private readonly IMapper _mapper;
        private MemoryCache _cache;
        private const string KEY = "user_cache";

        public HotelsController(ILogger<HotelsController> logger, IHotelsSearchService hotelsSearchService, IAmadeusApiHotelRoomsServiceProvider hotelRoomsService, MyMemoryCache memoryCache, IAmadeusApiHotelBookingServiceProvider bookingService, IMapper mapper)
        {
            _logger = logger;
            _hotelsSearchService = hotelsSearchService;
            _hotelRoomsService = hotelRoomsService;
            _cache = memoryCache.Cache;
            _bookingService = bookingService;
            _mapper = mapper;
        }

        /// <summary>
        ///  Returns data for given search request. Each request provides pagination parameters with limitation of 100 items per page.
        ///  This endpoint fetches requested data from Amadeus Api service and stores them in the database with certain expiration time.
        ///  For given combination of parameters for subsequent search requests with same combination of parameters, first chekcs cache memory,
        ///  after that checks if there are enough items in database and then returns them from database.
        ///  If there are some items in database, but not enough for given request, the rest is fetched recursively from stored NextItemsLink.
        ///  After stored data is expired (parameter in appsetings), new search request is stored in database and fetches fresh set of data from Amadeus API
        /// </summary>
        /// <param name="cityCode"><para>Destination City Code (or Airport Code). In case of city code, the search will be done around the city center.<br /> 
        /// Available codes can be found in <see href="https://www.iata.org/en/publications/directories/code-search/">IATA table codes</see> (3 chars IATA Code) <br /> 
        /// Example: PAR</para></param>
        /// <param name="checkInDate">check-in date of the stay (hotel local date). Format YYYY-MM-DD <br /> 
        /// The lowest accepted value is the present date (no dates in the past)</param>
        /// <param name="checkOutDate">check-out date of the stay (hotel local date). Format YYYY-MM-DD<br /> 
        /// The lowest accepted value is checkInDate+1</param>
        /// <param name="adults">Defines the number of adult guests.</param>
        /// <param name="pageSize">Defines the number of items returned in response. Maximum value is 100</param>
        /// <param name="pageOffset">Defines the page offset</param>
        /// <param name="cancellationToken"></param>
        /// <returns>HotelsSearchResponse</returns>
        /// <response code="200">Model HotelsSearchResponse for requested page, with additional information about current page size and offset, and information if there is another page</response>
        /// <response code="400">Bad request with invalid parameters</response>
        /// <response code="500">Unexpected internal error</response>
        /// <response code="502">Problem retrieving Amadeus Hotels information from Amadeus API</response>
        //[HttpGet]
        //[Route("search")]
        //[Produces("application/json")]
        //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(HotelsSearchResponse))]
        //[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(void))]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[ProducesResponseType(StatusCodes.Status502BadGateway)]
        //public async Task<ActionResult<HotelsSearchResponse>> Search(string cityCode, DateTime checkInDate, DateTime checkOutDate, int adults, int pageSize, int pageOffset, CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        HotelsSearchUserRequest hotelsSearchRequest = new HotelsSearchUserRequest(cityCode, checkInDate, checkOutDate, adults, pageSize, pageOffset);
        //    ValidateAndSanitazeHotelsSearchRequest(hotelsSearchRequest);

        //    HotelsSearchResponse response;

        //    // Check the cache if same request already exists
        //    bool isCacheHit = _cache.TryGetValue(hotelsSearchRequest.ToCacheKey(), out response);

        //    if (!isCacheHit)
        //    {
        //        _logger.LogInformation($"No cache hit. CityCode: {hotelsSearchRequest.CityCode}, " +
        //            $"CheckIn: {hotelsSearchRequest.CheckInDate}, CheckOut: {hotelsSearchRequest.CheckOutDate}, PageSize: {hotelsSearchRequest.PageSize}, PageOffset:{hotelsSearchRequest.PageOffset}");

        //        response = await _hotelsSearchService.SearchHotels(hotelsSearchRequest, cancellationToken);

        //        var cacheEntryOptions = new MemoryCacheEntryOptions()
        //            .SetSize(1)
        //            .SetSlidingExpiration(TimeSpan.FromMinutes(2))
        //            // Remove from cache after this time, regardless of sliding expiration
        //            .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

        //        _cache.Set(hotelsSearchRequest.ToCacheKey(), response, cacheEntryOptions);
        //    }
        //    else
        //    {
        //        _logger.LogInformation($"Cache hit for request. CityCode: {hotelsSearchRequest.CityCode}, " +
        //               $"CheckIn: {hotelsSearchRequest.CheckInDate}, CheckOut: { hotelsSearchRequest.CheckOutDate}, pageSize: {hotelsSearchRequest.PageSize}, pageOffset: {hotelsSearchRequest.PageOffset}");
        //    }

        //    return Ok(response);
        //    }
        //    catch (ArgumentException argEx)
        //    {
        //        _logger.LogWarning(argEx, argEx.Message);
        //        return StatusCode((int)HttpStatusCode.BadRequest, new { message = argEx.Message });
        //    }
        //    catch (HttpRequestException reqEx)
        //    {
        //        _logger.LogError(reqEx, "Cannot retrieve Amadeus Hotels information from Amadeus API.");
        //        return StatusCode((int)HttpStatusCode.BadGateway, new { message = "Cannot retrieve Amadeus Hotels information from Amadeus API. Reason: " + reqEx.Message });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Internal error.");
        //        return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "Internal error." });
        //    }
        //}

        [HttpGet]
        [Route("search-rooms")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(HotelRoomsAmadeusFetchModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(void))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        public async Task<ActionResult<HotelRoomsAmadeusFetchModel>> SearchRooms(string hotelId, DateTime checkInDate,
            DateTime checkOutDate, int adults ,CancellationToken cancellationToken)
        {
            try
            {
                HotelRoomsUserRequest hotelRoomsRequest = new HotelRoomsUserRequest(hotelId, checkInDate, checkOutDate, adults);
                ValidateAndSanitazeHotelRoomsSearchRequest(hotelRoomsRequest);

                //HotelRoomsResponse response;
                HotelRoomsAmadeusFetchModel response;

                response = await _hotelRoomsService.FetchAmadeusHotelRooms(hotelRoomsRequest, cancellationToken);

                HttpContext.Session.SetString("CheckInDate", checkInDate.ToString());
                HttpContext.Session.SetString("CheckOutDate", checkOutDate.ToString());

                return Ok(response);
            }
            catch (ArgumentException argEx)
            {
                _logger.LogWarning(argEx, argEx.Message);
                return StatusCode((int)HttpStatusCode.BadRequest, new { message = argEx.Message });
            }
            catch (HttpRequestException reqEx)
            {
                _logger.LogError(reqEx, "Cannot retrieve Amadeus Hotels information from Amadeus API.");
                return StatusCode((int)HttpStatusCode.BadGateway, new { message = "Cannot retrieve Amadeus Hotels information from Amadeus API. Reason: " + reqEx.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal error.");
                return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "Internal error." });
            }
        }


        [HttpGet]
        [Route("room-details")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RoomDetailsAmadeusFetchModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(void))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        [Authorize(Roles = UserRoles.User)]
        public async Task<ActionResult<RoomDetailsAmadeusFetchModel>> RoomDetails(string offerId, CancellationToken cancellationToken)
        {
            try
            {
                RoomDetailsUserRequest roomsDetailsRequest = new RoomDetailsUserRequest(offerId);
                ValidateAndSanitazeRoomDetailsSearchRequest(roomsDetailsRequest);

                //HotelRoomsResponse response;
                RoomDetailsAmadeusFetchModel response;

                response = await _hotelRoomsService.FetchAmadeusRoomDetails(roomsDetailsRequest, cancellationToken);

                // Add hotel and offer info to session, used in Booking action
                HttpContext.Session.SetObject("RoomDetailsResponse", response);

                

                // _cache.Set(response.ToCacheKey(), response, cacheEntryOptions);

                // AddToCache(response);

                return Ok(response);

                //return RedirectToAction("Booking", "Hotels", response);
            }
            catch (ArgumentException argEx)
            {
                _logger.LogWarning(argEx, argEx.Message);
                return StatusCode((int)HttpStatusCode.BadRequest, new { message = argEx.Message });
            }
            catch (HttpRequestException reqEx)
            {
                _logger.LogError(reqEx, "Cannot retrieve Amadeus Room Details information from Amadeus API.");
                return StatusCode((int)HttpStatusCode.BadGateway, new { message = "Cannot retrieve Amadeus Room Details information from Amadeus API. Reason: " + reqEx.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal error.");
                return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "Internal error." });
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hotelBookingUserRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("booking")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(HotelBookingAmadeusFetchModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(void))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        [Authorize(Roles = UserRoles.User)]
        public async Task<ActionResult<HotelBookingAmadeusFetchModel>> Booking([FromBody] HotelBookingUserRequest hotelBookingUserRequest, CancellationToken cancellationToken)
        {
            
            try
            {
                HotelBookingUserRequest bookingUserRequest = new HotelBookingUserRequest(hotelBookingUserRequest.HotelBookingRequest);
                
                //ValidateAndSanitazeRoomDetailsSearchRequest(bookingUserRequest);

                var response = await _bookingService.FetchAmadeusHotelBooking(bookingUserRequest, cancellationToken);

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userEmail = User.FindFirstValue(ClaimTypes.Email);




                //DateTime checkInDate = DateTime.Parse(HttpContext.Session.GetString("CheckInDate"));
                //DateTime checkOutDate = DateTime.Parse(HttpContext.Session.GetString("CheckOutDate"));
                DateTime checkInDate = DateTime.Now;
                DateTime checkOutDate = DateTime.Now;

                // retrive hotel and offer data from session
                var roomDetailsResponse = HttpContext.Session.GetObject<RoomDetailsAmadeusFetchModel>("RoomDetailsResponse");

                // TODO: store in db data from the previous action (RoomDetails)
                await _bookingService.CompleteBooking(hotelBookingUserRequest, roomDetailsResponse, response, checkInDate, checkOutDate, cancellationToken);

                // TODO: save user's reservation to DB
            

                var model = GetCachedModel;

                
                return Ok(response);
            }
            catch (ArgumentException argEx)
            {
                _logger.LogWarning(argEx, argEx.Message);
                return StatusCode((int)HttpStatusCode.BadRequest, new { message = argEx.Message });
            }
            catch (HttpRequestException reqEx)
            {
                _logger.LogError(reqEx, "Cannot retrieve Amadeus Booking information from Amadeus API.");
                return StatusCode((int)HttpStatusCode.BadGateway, new { message = "Cannot retrieve Amadeus Booking information from Amadeus API. Reason: " + reqEx.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal error.");
                return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "Internal error." });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetHotelsByCity([FromQuery] HotelSearchRequestModel request, CancellationToken cancellationToken)
        {
            try
            {
                HotelOffersResponse response;
                
                ValidateAndSanitazeHotelsSearchRequest(request);

                bool isCacheHit = _cache.TryGetValue(request.ToCacheKey(), out response);

                //if (response == null || !response.Data.Any()) // Check if the result is empty
                //{
                //    return NotFound("No hotels found for the specified city.");
                //}

                if (true)//!isCacheHit)
                {
                    _logger.LogInformation($"No cache hit. CityCode: {request.CityCode}, " +
                        $"Radius: {request.Radius}, RadiusUnit: {request.RadiusUnit}, HotelSource: {request.HotelSource}");

                    response = await _hotelsSearchService.SearchHotels(request, cancellationToken);
                        
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSize(1)
                        .SetSlidingExpiration(TimeSpan.FromMinutes(2))
                        // Remove from cache after this time, regardless of sliding expiration
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

                    _cache.Set(request.ToCacheKey(), response, cacheEntryOptions);
                }
                else
                {
                    _logger.LogInformation($"Cache hit for request. CityCode: {request.CityCode}, " +
                        $"Radius: {request.Radius}, RadiusUnit: {request.RadiusUnit}, HotelSource: {request.HotelSource}");
                }

                return Ok(response);
            }
            catch (ArgumentException argEx)
            {
                _logger.LogWarning(argEx, argEx.Message);
                return StatusCode((int)HttpStatusCode.BadRequest, new { message = argEx.Message });
            }
            catch (HttpRequestException reqEx)
            {
                _logger.LogError(reqEx, "Cannot retrieve Amadeus Hotels information from Amadeus API.");
                return StatusCode((int)HttpStatusCode.BadGateway, new { message = "Cannot retrieve Amadeus Hotels information from Amadeus API. Reason: " + reqEx.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal error.");
                return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "Internal error." });
            }
        }

        private void ValidateAndSanitazeHotelsSearchRequest(HotelSearchRequestModel hotelsSearchRequest)
        {
            if (String.IsNullOrEmpty(hotelsSearchRequest.CityCode))
            {
                throw new ArgumentException("City code must be provided.");
            }
            if (hotelsSearchRequest.CityCode.Length != 3)
            {
                throw new ArgumentException("City code must have three letters.");
            }
            //if (hotelsSearchRequest.Radius)
            //{
            //    throw new ArgumentException("Check-in date can not be in past.");
            //}
            //if (hotelsSearchRequest.CheckOutDate <= hotelsSearchRequest.CheckInDate.AddDays(1))
            //{
            //    throw new ArgumentException("Check-out date must be at least one day after check-in date.");
            //}
            //if (hotelsSearchRequest.PageSize < 1 || hotelsSearchRequest.PageOffset < 0)
            //{
            //    throw new ArgumentException("Invalid page size or page offset values.");
            //}
            //if (hotelsSearchRequest.PageSize > 100)
            //{
            //    throw new ArgumentException("Maximum page size is 100.");
            //}

            //hotelsSearchRequest.CheckInDate = hotelsSearchRequest.CheckInDate.Date;
            //hotelsSearchRequest.CheckOutDate = hotelsSearchRequest.CheckOutDate.Date;
        }

        private void ValidateAndSanitazeHotelRoomsSearchRequest(HotelRoomsUserRequest hotelRoomsSearchRequest)
        {
            if (String.IsNullOrEmpty(hotelRoomsSearchRequest.HotelId))
            {
                throw new ArgumentException("Hotel ID must be provided.");
            }
            if (hotelRoomsSearchRequest.HotelId.Length != 8)
            {
                throw new ArgumentException("Hotel ID must have three letters.");
            }
            if (hotelRoomsSearchRequest.CheckInDate.Date < DateTime.Now.Date)
            {
                throw new ArgumentException("Check-in date can not be in past.");
            }
            if (hotelRoomsSearchRequest.CheckOutDate <= hotelRoomsSearchRequest.CheckInDate.AddDays(1))
            {
                throw new ArgumentException("Check-out date must be at least one day after check-in date.");
            }
            if (hotelRoomsSearchRequest.Adults < 1 && hotelRoomsSearchRequest.Adults > 9)
            {
                throw new ArgumentException("Number of adult guests can be between 1 and 9");
            }

            hotelRoomsSearchRequest.CheckInDate = hotelRoomsSearchRequest.CheckInDate.Date;
            hotelRoomsSearchRequest.CheckOutDate = hotelRoomsSearchRequest.CheckOutDate.Date;
        }

        private void ValidateAndSanitazeRoomDetailsSearchRequest(RoomDetailsUserRequest roomDetailsSearchRequest)
        {
            if (String.IsNullOrEmpty(roomDetailsSearchRequest.OfferId))
            {
                throw new ArgumentException("Offer ID must be provided.");
            }
            //if (roomDetailsSearchRequest.Lang.Length != 8)
            //{
            //    throw new ArgumentException("Hotel ID must have three letters.");
            //}
        }

        [NonAction]
        public void AddToCache(RoomDetailsAmadeusFetchModel response)
        {
            var options = new MemoryCacheEntryOptions
            {
                Size = 1,
                SlidingExpiration = TimeSpan.FromMinutes(2),
                // Remove from cache after this time, regardless of sliding expiration
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            };

            _cache.Set<RoomDetailsAmadeusFetchModel>(KEY, response, options);
        }

        [NonAction]
        public RoomDetailsAmadeusFetchModel GetCachedModel()
        {
            RoomDetailsAmadeusFetchModel model = null;
            if(!_cache.TryGetValue(KEY, out model))
            {

            }
            return _cache.Get<RoomDetailsAmadeusFetchModel>(KEY);
        }

    }

    public static class SessionExtensions
    {
        public static void SetObject(this ISession session, string key, object value)
        {
            var obj = JsonConvert.SerializeObject(value);
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
