using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Models;
using EzTickets.Repository;
using AutoMapper;
using EzTickets.DTO.Public;
using System.Security.Claims;
using Data;
using EzTickets.DTO.Pagination;

namespace EzTickets.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IMapper _mapper;
        private readonly IEventRepository _eventRepository;
        private readonly IOrderRepository _orderRepository;

        public TicketController(ITicketRepository ticketRepository, IMapper mapper,
                                IEventRepository eventRepository, IOrderRepository orderRepository)
        {
            _ticketRepository = ticketRepository;
            _mapper = mapper;
            _eventRepository = eventRepository;
            _orderRepository = orderRepository;
        }

        [Authorize(Roles ="Admin")]
        [HttpGet]
        public ActionResult<GeneralResponse> GetAllTickets([FromQuery] PaginationParams pagination)
        {
            try
            {
                var tickets = _ticketRepository.GetAll(pagination);
                var ticketDTOs = _mapper.Map<List<TicketResponseDTO>>(tickets);
                var response = new GeneralResponse
                {
                    IsPass = true,
                    Data = ticketDTOs
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new GeneralResponse
                {
                    IsPass = false,
                    Data = null
                };
                return response;
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public ActionResult<GeneralResponse> GetTicket(string id)
        {
            var ticket = _ticketRepository.GetById(id);

            if (ticket == null)
            {
                GeneralResponse response = new GeneralResponse
                {
                    IsPass = false,
                    Data = null
                };
                return (response);
            }

            GeneralResponse responseData = new GeneralResponse
            {
                IsPass = true,
                Data = _mapper.Map<TicketResponseDTO>(ticket)
            };

            return responseData;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("event/{eventId}")]
        public ActionResult<GeneralResponse> GetTicketsByEvent(int eventId, TicketStatus ticketStatus)
        {
            var tickets = _ticketRepository.GetTicketsByEventId(eventId);

            if (ticketStatus == TicketStatus.Available)
            {
                tickets = tickets.Where(t => t.TicketStatus == TicketStatus.Available).ToList();
            }

            var ticketDTOs = _mapper.Map<List<TicketResponseDTO>>(tickets);

            GeneralResponse response = new GeneralResponse
            {
                IsPass = true,
                Data = ticketDTOs
            };
            return response;
        }

        [HttpGet("my-tickets")]
        [Authorize]
        public ActionResult<GeneralResponse> GetMyTickets()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new GeneralResponse
                {
                    IsPass = false,
                    Data = null
                });
            }

            var tickets = _ticketRepository.GetTicketsByUserId(userId);
            var ticketDTOs = _mapper.Map<List<TicketWithEventDTO>>(tickets);

            var response = new GeneralResponse
            {
                IsPass = true,
                Data = ticketDTOs
            };

            return response;
        }

        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Admin")]
        public ActionResult<GeneralResponse> GetTicketsByUser(string userId)
        {
            var tickets = _ticketRepository.GetTicketsByUserId(userId);
            var ticketDTOs = _mapper.Map<List<TicketWithEventDTO>>(tickets);

            var response = new GeneralResponse
            {
                IsPass = true,
                Data = ticketDTOs
            };

            return response;
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult<GeneralResponse> UpdateTicket(string id, TicketUpdateDTO ticketDTO)
        {
            var ticket = _ticketRepository.GetById(id);
            if (ticket == null)
            {
                var generalresponse = new GeneralResponse
                {
                    IsPass = false,
                    Data = null
                };
                return generalresponse;
            }

            _mapper.Map(ticketDTO, ticket);
            _ticketRepository.Update(ticket);
            _ticketRepository.Save();

            var response = new GeneralResponse
            {
                IsPass = true,
                Data = ticket
            };

            return response;
        }

    }
}
