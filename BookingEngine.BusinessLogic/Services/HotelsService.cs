using AutoMapper;
using BookingEngine.BusinessLogic.Models;
using BookingEngine.BusinessLogic.Services.Interfaces;
using BookingEngine.Data.Repositories.Interfaces;
using BookingEngine.Entities.Models;
using Microsoft.Extensions.Logging;
using System.Globalization;
using BookingEngine.Extensions;

namespace BookingEngine.BusinessLogic.Services
{
    public class HotelsService : IHotelsService
    {
        private readonly ILogger<HotelsService> _logger;
        private readonly IAmadeusApiServiceProvider _amadeusApiServiceProvider;
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public HotelsService(ILogger<HotelsService> logger, IAmadeusApiServiceProvider amadeusApiServiceProvider, IOrderRepository orderRepository, IMapper mapper)
        {
            _logger = logger;
            _amadeusApiServiceProvider = amadeusApiServiceProvider;
            _orderRepository = orderRepository;
            _mapper = mapper;
        }


        public async Task<HotelOffersResponse> SearchHotels(HotelSearchRequestModel request, CancellationToken cancellationToken)
        {
            var response = new HotelOffersResponse();

            var hotelsData = await _amadeusApiServiceProvider.FetchHotelsByCity(_mapper.Map<HotelSearchRequest>(request), cancellationToken);

            string hotelIds = String.Empty;
            HotelOffersRequest offersRequest = _mapper.Map<HotelOffersRequest>(request);
            HotelOffersResponse hotelOffers = null;
            if (hotelsData.Data.Count() > 0)
            {
                List<string> hotelIdsList = new List<string>();

                foreach (var hotel in hotelsData.Data.Take(50))
                {
                    hotelIdsList.Add(hotel.HotelId);
                }

                offersRequest.HotelIds = string.Join(",", hotelIdsList);

                hotelOffers = await _amadeusApiServiceProvider.FetchHotelOffers(offersRequest, cancellationToken);

                hotelOffers.Data.Where(x => x.Available);
            }

            response = hotelOffers;
            return response;
        }
        public async Task<HotelBookingResultDTO> ProcessHotelBookingAsync(HotelBookingRequestDTO bookingRequestDto, HotelOffersResponse offers, string userId, CancellationToken cancellationToken)
        {
            try
            {
                var result = new HotelBookingResultDTO();

                // 1. Retrieve hotel offer response
                var hotelOffer = offers.Data
                  .SelectMany(r => r.Offers
                  .Where(offer => offer.Id == bookingRequestDto.OfferId) // try putting the query labda into the FirstOrDefault(*) and delete .Where()
                  .Select(offer => new HotelOfferResponseShortDTO
                  {
                      Type = r.Type,
                      Hotel = r.Hotel,
                      Available = r.Available,
                      Offer = offer, // Only the matching offer
                      Self = r.Self,
                  }))
                  .FirstOrDefault();

                if (hotelOffer == null)
                {
                    throw new InvalidOperationException("Matching hotel offer not found.");
                }

                // 2. Get booking details from Amadeus
                var amadeusResponse = await _amadeusApiServiceProvider.CreateHotelBooking(bookingRequestDto, cancellationToken);

                // 3. Create Order
                var order = CreateOrderFromBookingData(bookingRequestDto, amadeusResponse, hotelOffer, userId);
    
                await _orderRepository.AddAsync(order);

                // 4. Create result object HotelBookingResultDTO
                _mapper.Map<Order, HotelBookingResultDTO>(order, result);

                // 5. Return Booking Result
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during hotel booking process.");

                // Option 1: Rethrow for General Error Handling (in the controller)
                throw;
            }

        }
        private Order CreateOrderFromBookingData(HotelBookingRequestDTO bookingRequestDto, HotelBookingAmadeusFetchModel amadeusResponse, HotelOfferResponseShortDTO hotelOffer, string userId)
        {
            // 1. Extract Relevant Data
            string offerId = bookingRequestDto.OfferId;

            var orderItems = amadeusResponse.Item.Data.Select(item => _mapper.Map<OrderItem>(item)).ToList();

            // 2. Map to Order and OrderItems
            var order = new Order
            {
                OrderItems = orderItems,
                DateCreated = DateTime.Now,
                UserId = userId,
                OfferId = hotelOffer.Offer.Id,
                CheckInDate = hotelOffer.Offer.CheckInDate,
                CheckOutDate = hotelOffer.Offer.CheckOutDate,
                RoomQuantity = hotelOffer.Offer.RoomQuantity,
                BoardType = hotelOffer.Offer.BoardType,
                TotalPrice = float.Parse(hotelOffer.Offer.Price.Total, CultureInfo.InvariantCulture.NumberFormat),
                Currency = hotelOffer.Offer.Price.Currency,
                HotelId = hotelOffer.Hotel.HotelId,
                HotelName = hotelOffer.Hotel.Name,
                HotelCityCode = hotelOffer.Hotel.CityCode,
                Adults = hotelOffer.Offer.Guests.Adults
            };

            return order;
        }

    }
}
