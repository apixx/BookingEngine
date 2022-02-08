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
                    dest.Name,
                    opt => opt.MapFrom(src => src.Hotel.Name))
                .ForMember(dest =>
                    dest.Rating,
                    opt => opt.MapFrom(src => src.Hotel.Rating))
                .ForMember(dest =>
                    dest.Description,
                    opt => opt.MapFrom(src => src.Hotel.Description));
        }
    }
}
