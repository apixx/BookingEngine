using AutoMapper;
using BookingEngine.BusinessLogic.Mapping;
using BookingEngine.BusinessLogic.Models;
using BookingEngine.BusinessLogic.Services.Interfaces;
using BookingEngine.Controllers;
using BookingEngine.Data.Repositories.Interfaces;
using BookingEngine.Entities.Models.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace BookingEngine.Tests
{
    [TestFixture]
    public class HotelsControllerTests
    {
        private Mock<IHotelsService> _mockHotelsService;
        private Mock<ILogger<HotelsController>> _mockLogger;
        private IMapper _mapper;
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private HotelsController _controller;
        private Mock<HttpContext> mockHttpContext;
        private Mock<ISession> mockSession;

        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<HotelsController>>();
            _mockHotelsService = new Mock<IHotelsService>();
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()).CreateMapper();

            var store = new Mock<IUserStore<ApplicationUser>>();  // Example placeholder 
            mockHttpContext = new Mock<HttpContext>();
            mockSession = new Mock<ISession>();
            mockHttpContext.Setup(h => h.Session).Returns(mockSession.Object);
            //_mockUserManager = new Mock<UserManager<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                new Mock<IUserStore<ApplicationUser>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<ApplicationUser>>().Object,
                new IUserValidator<ApplicationUser>[0],
                new IPasswordValidator<ApplicationUser>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<ApplicationUser>>>().Object);

            _controller = new HotelsController(_mockLogger.Object, _mockHotelsService.Object, _mockUserManager.Object, _mapper)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
        }

        [Test]
        public async Task GetHotelsByCity_ValidRequest_ReturnsOkObjectResult()
        {
            // Arrange
            var request = new HotelSearchRequestModel { CityCode = "NYC", Adults = 2, CheckInDate = new DateTime(year: 2024, month: 9, day: 1), CheckOutDate = new DateTime(year: 2024, month: 9, day: 6), Radius = 5, RadiusUnit = "KM", RoomQuantity = 1, HotelSource = "ALL" }; // Valid request

            var serviceResult = _mockHotelsService.Setup(x => x.SearchHotels(It.IsAny<HotelSearchRequestModel>(), It.IsAny<CancellationToken>()))
                              .ReturnsAsync(new HotelOffersResponse()
                              {
                                  Data = new List<HotelOfferResponse>()
                                  {
                                    new HotelOfferResponse
                                    {
                                        Type = "hotel-offers",
                                        Available = true,
                                        Hotel = new HotelData
                                        {
                                            Type = "Hotel",
                                            HotelId = "ALNYC647",
                                            ChainCode = "AL",
                                            DupeId = "501447323",
                                            Name = "Aloft Manhattan Downtown Financial District",
                                            CityCode = "NYC",
                                            Latitude = 40.71041,
                                            Longitude = -74.00666
                                        },
                                        Offers = new List<HotelOffer>()
                                        {
                                             new HotelOffer
                                             {
                                                 Id = "ZJGJ8ZEEXQ",
                                                 RoomQuantity = 0,
                                                 CheckInDate = new DateTime(year: 2024, month: 9, day:1),
                                                 // DateTime.ParseExact("2024-09-01 00:00:00", "yyyy-MM-dd HH:mm:ss,fff",
                                      // System.Globalization.CultureInfo.InvariantCulture),
                                                 CheckOutDate = new DateTime(year: 2024, month: 9, day:6),
                                                 RateCode = "RAC",
                                                 RateFamilyEstimated = null,
                                                 BoardType = null,
                                                 Room = new Room
                                                 {
                                                     Type = "LTB",
                                                     TypeEstimated = new TypeEstimated { Category = null },
                                                     Description = new Description
                                                     {
                                                         Text = "Long Term Stay rate\nSleeps 2, Fast & free WiFi throughout the\nhotel, 1 King, 210sqft/19sqm-230sqft/21sqm,",
                                                         Lang = "EN"
                                                     }
                                                 },
                                                 Guests = new Guests{ Adults = 2 },
                                                 Price = new Price
                                                 {
                                                     Currency = "USD",
                                                     Base = "1332.50",
                                                     Total = "1546.57",
                                                     Taxes = null,
                                                     Variations = new Variations
                                                     {
                                                         Average = new Average { Base = "266.50" },
                                                         Changes = new List<Change>
                                                         {
                                                             new Change
                                                             {
                                                                  StartDate = new DateTime(year: 2024, month: 9, day:1),
                                                                  EndDate = new DateTime(year: 2024, month: 9, day:2),
                                                                  Base = "253.38"
                                                             },
                                                             new Change
                                                             {
                                                                  StartDate = new DateTime(year: 2024, month: 9, day:2),
                                                                  EndDate = new DateTime(year: 2024, month: 9, day:3),
                                                                  Base = "204.18"
                                                             },
                                                             new Change
                                                             {
                                                                  StartDate = new DateTime(year: 2024, month: 9, day:3),
                                                                  EndDate = new DateTime(year: 2024, month: 9, day:4),
                                                                  Base = "253.38"
                                                             },
                                                             new Change
                                                             {
                                                                  StartDate = new DateTime(year: 2024, month: 9, day:4),
                                                                  EndDate = new DateTime(year: 2024, month: 9, day:5),
                                                                  Base = "302.58"
                                                             },
                                                             new Change
                                                             {
                                                                  StartDate = new DateTime(year: 2024, month: 9, day:5),
                                                                  EndDate = new DateTime(year: 2024, month: 9, day:6),
                                                                  Base = "318.98"
                                                             }
                                                         }
                                                     }
                                                 },
                                                 Policies = new Policies{
                                                     Cancellations = new List<Cancellation>
                                                     {
                                                         new Cancellation { Type = null, Description = null }
                                                     },
                                                     Deposit = null,
                                                     PaymentType = "guarantee"
                                                 },
                                                 Self = "https://test.api.amadeus.com/v3/shopping/hotel-offers/ZJGJ8ZEEXQ"
                                             }
                                        },
                                        Self = "https://test.api.amadeus.com/v3/shopping/hotel-offers?hotelIds=ALNYC647&adults=2&checkInDate=2024-09-01&checkOutDate=2024-09-06&currency=EUR&roomQuantity=1"
                                    }
                                  }
                              });

           // mockSession.Setup(s => s.SetObject(It.IsAny<string>(), It.IsAny<IHotelsService>()))
           //.Callback((string key, object value) =>
           //{
           //    key = "HotelSearchResponse";
           //    value = serviceResult;
           //});

            // Act
            var result = await _controller.GetHotelsByCity(request, CancellationToken.None);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        }
        //[Test]
        //public async Task GetHotelsByCity_ValidRequest_ReturnsOkObjectResult()
        //{
        //    // Arrange
        //    var request = new HotelSearchRequestModel { CityCode = "NYC" };
        //    mockAmadeusService.Setup(x => x.FetchHotelsByCity(It.IsAny<HotelSearchRequestModel>(), It.IsAny<CancellationToken>()))
        //                      .ReturnsAsync(new HotelOffersResponse() { /* Test Hotel Data */ });

        //    // Act
        //    var result = await _controller.GetHotelsByCity(request, CancellationToken.None);

        //    // Assert
        //    Assert.That(result, Is.InstanceOf<OkObjectResult>());
        //}

        //[Test]
        //public async Task GetHotelsByCity_AmadeusApiError_ReturnsBadGateway()
        //{
        //    // Arrange
        //    var request = new HotelSearchRequestModel { CityCode = "NYC" };
        //    mockAmadeusService.Setup(x => x.FetchHotelsByCity(It.IsAny<HotelSearchRequestModel>(), It.IsAny<CancellationToken>()))
        //                      .ThrowsAsync(new HttpRequestException("Test Amadeus API Error"));

        //    // Act
        //    var result = await _controller.GetHotelsByCity(request, CancellationToken.None);

        //    // Assert
        //    Assert.IsInstanceOf<StatusCodeResult>(result);
        //    Assert.AreEqual(StatusCodes.Status502BadGateway, ((StatusCodeResult)result).StatusCode);
        //}
    }
}