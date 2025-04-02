using AutoMapper;
using Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Models;

namespace EzTickets.Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.TicketIds, opt => opt.MapFrom(src => src.Tickets.Select(t => t.TicketID)))
                .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.OrderStatus.ToString()))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod.ToString()));

            CreateMap<CreateOrderDto, Order>();
            CreateMap<UpdateOrderDto, Order>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
