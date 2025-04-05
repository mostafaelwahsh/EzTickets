using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Models;
using EzTickets.Repository;
using AutoMapper;
using EzTickets.DTO.Public;
using System.Security.Claims;
using Data;
using EzTickets.DTO.Pagination;
using System.Text.Json.Serialization;
using System.Text.Json;

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

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public ActionResult<GeneralResponse> GetTicket(string id)
        {
            var ticket = _ticketRepository.GetById(id);
            var dto = _mapper.Map<TicketResponseDTO>(ticket);

            if (ticket == null)
            {
                GeneralResponse response = new GeneralResponse
                {
                    IsPass = false,
                    Data = "Ticket Not Found"
                };
             
            }

            return (new GeneralResponse
            {
                IsPass = true,
                Data = dto
            });
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet("event/{eventId}")]
        public ActionResult<GeneralResponse> GetTicketsByEvent(int eventId, TicketStatus? ticketStatus = null)
        {
            // Get tickets filtered directly from the database
            var tickets = _ticketRepository.GetTicketsByEventIdAndStatus(eventId);

            // Map to DTOs
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
        //[Authorize(Roles = "Admin")]
        public ActionResult<GeneralResponse> UpdateTicket(string id, TicketUpdateDTO ticketDTO)
        {
            var ticket = _ticketRepository.GetById(id);
            if (ticket == null)
            {
                var generalResponse = new GeneralResponse
                {
                    IsPass = false,
                    Data = null
                };
                return NotFound(generalResponse);
            }


            string originalUserId = ticket.UserID;

            _mapper.Map(ticketDTO, ticket);

           
            if (string.IsNullOrEmpty(ticketDTO.UserID))
            {
                ticket.UserID = originalUserId;
            }

            _ticketRepository.Update(ticket);
            _ticketRepository.Save();

            var responseDTO = _mapper.Map<TicketResponseDTO>(ticket);

            if (ticket.User != null)
            {
                responseDTO.UserFullName = ticket.User.FullName;
            }

            var response = new GeneralResponse
            {
                IsPass = true,
                Data = responseDTO
            };

            return Ok(response);
        }


    }
}
