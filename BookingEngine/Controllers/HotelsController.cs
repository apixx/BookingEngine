using BookingEngine.BusinessLogic.Models;
using BookingEngine.BusinessLogic.Services.Interfaces;
using BookingEngine.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Net;
using AutoMapper;
using Newtonsoft.Json;
using BookingEngine.Entities.Models.Authentication;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace BookingEngine.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly ILogger<HotelsController> _logger;
        private readonly IHotelsService _hotelsService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public HotelsController(ILogger<HotelsController> logger, IHotelsService hotelsService, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _logger = logger;
            _hotelsService = hotelsService;
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("search")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<ActionResult<HotelOffersResponse>> GetHotelsByCity([FromQuery] HotelSearchRequestModel request, CancellationToken cancellationToken)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                HotelOffersResponse response;

                ValidateAndSanitazeHotelsSearchRequest(request);

                _logger.LogInformation($"No cache hit. CityCode: {request.CityCode}, " +
                    $"Radius: {request.Radius}, RadiusUnit: {request.RadiusUnit}, HotelSource: {request.HotelSource}");

                response = await _hotelsService.SearchHotels(request, cancellationToken);

                HttpContext.Session.SetObject("HotelSearchResponse", response);

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
        public async Task<ActionResult<HotelBookingResultDTO>> Booking([FromBody] HotelBookingRequestDTO hotelBookingRequestDto, CancellationToken cancellationToken)
        {
            
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                ValidateAndSanitizeHotelBookingRequest(hotelBookingRequestDto);

                var userId = _userManager.Users.Where(u => u.UserName.Contains(User.Identity.Name)).Select(u => u.Id).FirstOrDefault();

                // retrive hotel and offer data from session
                var offers = HttpContext.Session.GetObject<HotelOffersResponse>("HotelSearchResponse");
                
                var bookingResult = await _hotelsService.ProcessHotelBookingAsync(hotelBookingRequestDto, offers, userId, cancellationToken);

                return Ok(_mapper.Map<HotelBookingResultDTO>(bookingResult));
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
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database update failed.");
                return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "Database update failed." });
            }
            catch (InvalidOperationException invOpEx)
            {
                _logger.LogWarning(invOpEx, "Invalid operation.");
                return StatusCode((int)HttpStatusCode.BadRequest, new { message = "Invalid operation." });
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
            if (hotelsSearchRequest.Radius < 1 || hotelsSearchRequest.Radius > 300)
            {
                throw new ArgumentException("Radius must have value between 1 and 300.");
            }
            if (hotelsSearchRequest.CheckInDate < DateTime.Now)
            {
                throw new ArgumentException("Check In Date must be in the future.");
            }
            if (hotelsSearchRequest.CheckOutDate < hotelsSearchRequest.CheckInDate.AddDays(1))
            {
                throw new ArgumentException("Check Out Date must be at least 1 day later than Check In Date.");
            }
            if (hotelsSearchRequest.Adults > hotelsSearchRequest.RoomQuantity * 9 || hotelsSearchRequest.Adults < hotelsSearchRequest.RoomQuantity)
            {
                throw new ArgumentException("Number of adults does not correspond with the number of rooms. The maximum capacity of a Room can be 9 adults.");
            }
        }

        private static void ValidateAndSanitizeHotelBookingRequest(HotelBookingRequestDTO bookingRequest)
        {
            // Validate OfferId
            ValidateOfferId(bookingRequest.OfferId);

            // Validate Guests
            if (bookingRequest.Guests == null || bookingRequest.Guests.Count < 1 || bookingRequest.Guests.Count > 99)
            {
                throw new ArgumentException("Guests count must be between 1 and 99.");
            }
            foreach (var guest in bookingRequest.Guests)
            {
                ValidateGuest(guest);
            }

            // Validate Payments
            ValidatePayments(bookingRequest.Payments);
        }

        private static void ValidateOfferId(string offerId)
        {
            if (string.IsNullOrWhiteSpace(offerId) || offerId.Length < 2 || offerId.Length > 100 || !Regex.IsMatch(offerId, "^[A-Z0-9]*$"))
            {
                throw new ArgumentException("Invalid offerId.");
            }
        }

        private static void ValidateGuest(GuestDTO guest)
        {
            // Validate Name
            ValidateName(guest.Name);

            // Validate Contact
            ValidateContact(guest.Contact);
        }

        private static void ValidateName(NameDTO name)
        {
            if (!Regex.IsMatch(name.Title, "^[A-Za-z -]*$") || name.Title.Length < 1 || name.Title.Length > 54)
            {
                throw new ArgumentException("Invalid title.");
            }
            if (!Regex.IsMatch(name.FirstName, "^[A-Za-z \\p{Lo}\\p{IsKatakana}\\p{IsHiragana}\\p{IsHangulCompatibilityJamo}\\p{IsHangulJamo}\\p{IsHangulSyllables}-]*$") || name.FirstName.Length < 1 || name.FirstName.Length > 56)
            {
                throw new ArgumentException("Invalid firstName.");
            }
            if (!Regex.IsMatch(name.LastName, "^[A-Za-z \\p{Lo}\\p{IsKatakana}\\p{IsHiragana}\\p{IsHangulCompatibilityJamo}\\p{IsHangulJamo}\\p{IsHangulSyllables}-]*$") || name.LastName.Length < 1 || name.LastName.Length > 57)
            {
                throw new ArgumentException("Invalid lastName.");
            }
        }

        private static void ValidateContact(ContactDTO contact)
        {
            if (!Regex.IsMatch(contact.Phone, "^[+][1-9][0-9]{4,18}$") || contact.Phone.Length < 6 || contact.Phone.Length > 20)
            {
                throw new ArgumentException("Invalid phone number.");
            }

            var emailAttribute = new EmailAddressAttribute();
            if (!emailAttribute.IsValid(contact.Email) || contact.Email.Length < 3 || contact.Email.Length > 90)
            {
                throw new ArgumentException("Invalid email address.");
            }
        }

        private static void ValidatePayments(List<PaymentDTO> payments)
        {
            if (payments == null || payments.Count < 1 || payments.Count > 9)
            {
                throw new ArgumentException("Payments count must be between 1 and 9.");
            }
            foreach (var payment in payments)
            {
                ValidatePayment(payment);
            }
        }

        private static void ValidatePayment(PaymentDTO payment)
        {
            // Assuming method is always "creditCard", but you could add checks for other types if they become available
            if (payment.Method.ToLower() != "creditcard")
            {
                throw new ArgumentException("Invalid payment method. Only 'creditCard' is accepted.");
            }

            ValidateCard(payment.Card);
        }

        private static void ValidateCard(CardDTO card)
        {
            if (!Regex.IsMatch(card.VendorCode, "^[A-Z]{2}$"))
            {
                throw new ArgumentException("Invalid vendor code.");
            }
            if (!Regex.IsMatch(card.CardNumber, "^[0-9]*$") || card.CardNumber.Length < 2 || card.CardNumber.Length > 22)
            {
                throw new ArgumentException("Invalid card number.");
            }
            if (!Regex.IsMatch(card.ExpiryDate, "^[0-9]{4}-[0-9]{2}$") || card.ExpiryDate.Length != 7)
            {
                throw new ArgumentException("Invalid expiry date format. Use YYYY-MM.");
            }
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
