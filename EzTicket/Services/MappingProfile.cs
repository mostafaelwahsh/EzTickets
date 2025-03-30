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
            #region TicketsAutoMapper
            CreateMap<Ticket, TicketResponseDTO>()
               .ForMember(dest => dest.EventName, opt => opt.MapFrom(src => src.Event.EventName))
               .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User.FullName));

            // Mapping from Ticket entity to TicketWithEventDTO
            CreateMap<Ticket, TicketWithEventDTO>()
                .ForMember(dest => dest.EventName, opt => opt.MapFrom(src => src.Event.EventName))
                .ForMember(dest => dest.VenueName, opt => opt.MapFrom(src => src.Event.VenueName))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Event.City))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.Event.StartDate));

            // Mapping from TicketCreateDTO to Ticket entity
            CreateMap<TicketCreateDTO, Ticket>()
                .ForMember(dest => dest.TicketID, opt => opt.Ignore())
                .ForMember(dest => dest.TicketStatus, opt => opt.MapFrom(src =>
                    string.IsNullOrEmpty(src.UserID) ? TicketStatus.Available : TicketStatus.SoldOut))
                .ForMember(dest => dest.PurchaseDate, opt => opt.MapFrom(src =>
                    string.IsNullOrEmpty(src.UserID) ? null : (DateTime?)DateTime.UtcNow))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false));

            // Mapping from TicketUpdateDTO to Ticket entity
            CreateMap<TicketUpdateDTO, Ticket>()
                .ForMember(dest => dest.EventID, opt => opt.Ignore())
                .ForMember(dest => dest.Event, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.TicketType, opt => opt.Ignore())
                .ForMember(dest => dest.Price, opt => opt.Ignore())
                .ForMember(dest => dest.ExpirationDate, opt => opt.Ignore())
                .ForMember(dest => dest.SeatNumber, opt => opt.Ignore())
                .ForMember(dest => dest.QRCode, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
            #endregion
        }
    }
}
