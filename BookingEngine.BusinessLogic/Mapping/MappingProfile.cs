using AutoMapper;
using BookingEngine.BusinessLogic.Models;
using BookingEngine.BusinessLogic.Models.AmadeusApiCustomModels;
using BookingEngine.BusinessLogic.Models.AmadeusApiCustomModels.Hotel.HotelSearch;
using BookingEngine.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingEngine.BusinessLogic.Models.AmadeusApiCustomModels.Hotel.Booking;

namespace BookingEngine.BusinessLogic.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<HotelsSearchUserRequest, SearchRequest>();

            CreateMap<AmadeusApiHotelsSearchResponseItem, SearchRequestHotel>()
                .ForMember(dest =>
                    dest.PriceTotal,
                    opt => opt.MapFrom(src => src.BestOffer.Total))
                .ForMember(dest =>
                    dest.PriceCurrency,
                    opt => opt.MapFrom(src => src.BestOffer.Currency))
                .ForMember(dest =>
                    dest.Distance,
                    opt => opt.MapFrom(src => src.Hotel.HotelDistance.Distance))
                .ForMember(dest => dest.Hotel, opt => opt.Ignore());

            CreateMap<AmadeusApiHotelItem, Hotel>()
                .ForMember(dest =>
                    dest.Description,
                    opt => opt.MapFrom(src => src.Description.Text));

            CreateMap<SearchRequestHotel, HotelSearchItemResponse>()
                .ForMember(dest =>
                    dest.HotelId, 
                    opt => opt.MapFrom(src => src.Hotel.HotelId))
                .ForMember(dest =>
                    dest.Name,
                    opt => opt.MapFrom(src => src.Hotel.Name))
                .ForMember(dest =>
                    dest.Rating,
                    opt => opt.MapFrom(src => src.Hotel.Rating))
                .ForMember(dest =>
                    dest.Description,
                    opt => opt.MapFrom(src => src.Hotel.Description));

            CreateMap<HotelBookingItem, OrderItem>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.BookingItemId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ProviderConfirmationId, opt => opt.MapFrom(src => src.ProviderConfirmationId))
                .ForMember(dest => dest.SelfUri, opt => opt.MapFrom(src => src.SelfUri));

            CreateMap<AssociatedRecord, AssociatedRecordItem>()
                .ForMember(dest => dest.OriginSystemCode, opt => opt.MapFrom(src => src.OriginSystemCode))
                .ForMember(dest => dest.Reference, opt => opt.MapFrom(src => src.Reference));


            CreateMap<AmadeusApiHotelItem, OrderItem>()
                .ForMember(dest => dest.HotelId, opt => opt.MapFrom(src => src.HotelId))
                .ForMember(dest => dest.HotelName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.HotelCityCode, opt => opt.MapFrom(src => src.CityCode));

            CreateMap<AmadeusApiOfferItem, OrderItem>()
                .ForMember(dest => dest.OfferId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CheckInDate, opt => opt.MapFrom(src => src.CheckInDate))
                .ForMember(dest => dest.CheckOutDate, opt => opt.MapFrom(src => src.CheckOutDate))
                .ForMember(dest => dest.RoomQuantity, opt => opt.MapFrom(src => src.RoomQuantity))
                .ForMember(dest => dest.BoardType, opt => opt.MapFrom(src => src.BoardType));

            CreateMap<CommissionItem, OrderItem>()
                .ForMember(dest => dest.CommisionPercentage, opt => opt.MapFrom(src => src.Percentage))
                .ForMember(dest => dest.CommisionAmount, opt => opt.MapFrom(src => src.Amount));

            CreateMap<PriceItem, OrderItem>()
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Total))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Currency));

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
        }
    }
}
