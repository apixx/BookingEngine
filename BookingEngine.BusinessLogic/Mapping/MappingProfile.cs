using AutoMapper;
using BookingEngine.BusinessLogic.Models;
using BookingEngine.Entities.Models;
using BookingEngine.BusinessLogic.Models.AmadeusApiModels.Hotel.Booking;

namespace BookingEngine.BusinessLogic.Mapping
{
    public class MappingProfile : Profile
    {

        public MappingProfile()
        {

            CreateMap<HotelBookingItem, OrderItem>(MemberList.None)
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.BookingItemId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.AssociatedRecords, opt => opt.MapFrom(src => src.AssociatedRecords));

            CreateMap<AssociatedRecord, AssociatedRecordItem>()
                .ForMember(dest => dest.OriginSystemCode, opt => opt.MapFrom(src => src.OriginSystemCode))
                .ForMember(dest => dest.Reference, opt => opt.MapFrom(src => src.Reference));

            CreateMap<StakeHolder, Guest>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id));

            CreateMap<NameItem, Guest>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName));

            CreateMap<Contact, Guest>()
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

            CreateMap<PaymentItem, Guest>()
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.Method));

            //check the mapper
            CreateMap<HotelSearchRequest, HotelSearchRequestModel>()
                .ForMember(dest => dest.CheckInDate, opt => opt.Ignore())
                .ForMember(dest => dest.CheckOutDate, opt => opt.Ignore())
                .ForMember(dest => dest.Adults, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<HotelSearchRequestModel, HotelOffersRequest>()
                .ForMember(dest => dest.HotelIds, opt => opt.Ignore())
                .ForMember(dest => dest.Adults, opt => opt.MapFrom(src => src.Adults))
                .ForMember(dest => dest.CheckInDate, opt => opt.MapFrom(src => src.CheckInDate))
                .ForMember(dest => dest.CheckOutDate, opt => opt.MapFrom(src => src.CheckOutDate));

            CreateMap<Order, OrderDTO>();
            CreateMap<OrderDTO, Order>();
            CreateMap<HotelBookingRequest, HotelBookingRequestDTO>();
            CreateMap<HotelBookingRequestDTO, HotelBookingRequest>();
            CreateMap<HotelBookingRequestDTO, AmadeusApiHotelBookingRequestItem>(); // Maps DTO -> Amadeus Model Item

            CreateMap<GuestDTO, StakeHolder>();
            CreateMap<PaymentDTO, PaymentItem>();
            CreateMap<CardDTO, CardItem>();
            CreateMap<ContactDTO, Contact>();
            CreateMap<NameDTO, NameItem>();
            CreateMap<RoomDTO, Models.AmadeusApiModels.Hotel.Booking.Room>();

            CreateMap<Order, HotelBookingResultDTO>()
                .ForMember(dest => dest.Bookings, opt => opt.MapFrom(src => src.OrderItems))
                .ForMember(dest => dest.BookingStatus, opt => opt.MapFrom(src => src.OrderStatus.StatusValue))
                .ForMember(dest => dest.NumberOfGuests, opt => opt.MapFrom(src => src.Adults));
            CreateMap<AssociatedRecordItem, AssociatedRecordDTO>();
            CreateMap<OrderItem, HotelBookingItemDTO>();
        }
    }
}
