using AutoMapper;
using Data;
using EzTickets.DTO;
using Microsoft.AspNetCore.Http.HttpResults;
using Models;

namespace EzTickets.Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region order
            CreateMap<CreateOrderDto, Order>()
           .ForMember(dest => dest.Tickets, opt => opt.Ignore())
           .ForMember(dest => dest.Id, opt => opt.Ignore())
           .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
           .ForMember(dest => dest.User, opt => opt.Ignore())
           .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
            #endregion

        }
    }
}
