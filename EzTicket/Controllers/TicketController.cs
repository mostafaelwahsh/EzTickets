using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Models;
using EzTickets.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using EzTickets.DTO.Public;

namespace EzTickets.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IMapper _mapper;
        private readonly IEventRepository _eventRepository;

        public TicketController(ITicketRepository ticketRepository, IMapper mapper, IEventRepository eventRepository)
        {
            _ticketRepository = ticketRepository;
            _mapper = mapper;
            _eventRepository = eventRepository;
        }

        // GET: api/Ticket
        [HttpGet]
        public ActionResult<GeneralResponse> GetAllTickets([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var tickets = _ticketRepository.GetAll()
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
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

                return StatusCode(500, response);
            }
        }

        // GET: api/Ticket/5
        [HttpGet("{id}")]
        public ActionResult<GeneralResponse> GetTicket(string id)
        {
            var ticket = _ticketRepository.GetById(id);

            if (ticket == null)
            {
                var response = new GeneralResponse
                {
                    IsPass = false,
                    Data = null
                };
                return NotFound(response);
            }

            var responseData = new GeneralResponse
            {
                IsPass = true,
                Data = _mapper.Map<TicketResponseDTO>(ticket)
            };

            return Ok(responseData);
        }

        // GET: api/Ticket/event/5
        [HttpGet("event/{eventId}")]
        public ActionResult<GeneralResponse> GetTicketsByEvent(string eventId, [FromQuery] TicketStatus ticketStatus)
        {
            //if (!_eventRepository.EventExists(eventId))
            //{
            //    var response = new GeneralResponse
            //    {
            //        IsPass = false,
            //        Message = "Event not found",
            //        Data = null
            //    };
            //    return NotFound(response);
            //}

            var tickets = _ticketRepository.GetTicketsByEventId(eventId);
            if (ticketStatus == TicketStatus.Available)
            {
                tickets = tickets.Where(t => t.TicketStatus == TicketStatus.Available).ToList();
            }

            var ticketDTOs = _mapper.Map<List<TicketResponseDTO>>(tickets);
            var response = new GeneralResponse
            {
                IsPass = true,
                Data = ticketDTOs
            };

            return Ok(response);
        }

        // GET: api/Ticket/user/userid
        [HttpGet("user/{userId}")]
        public ActionResult<GeneralResponse> GetTicketsByUser(string userId)
        {
            //if (userId != User.Identity?.Name && !User.IsInRole("Admin"))
            //{
            //    var response = new GeneralResponse
            //    {
            //        IsPass = false,
            //        Message = "Access denied",
            //        Data = null
            //    };
            //    return Forbid();
            //}

            var tickets = _ticketRepository.GetTicketsByUserId(userId);
            var ticketDTOs = _mapper.Map<List<TicketWithEventDTO>>(tickets);

            var response = new GeneralResponse
            {
                IsPass = true,
                Data = ticketDTOs
            };

            return Ok(response);
        }

        // POST: api/Ticket
        [HttpPost]
        public ActionResult<GeneralResponse> CreateTicket(TicketCreateDTO ticketDTO)
        {
            if (!ModelState.IsValid)
            {
                var generalresponse = new GeneralResponse
                {
                    IsPass = false,
                    Data = ModelState
                };
                return BadRequest(generalresponse);
            }

            var ticket = _mapper.Map<Ticket>(ticketDTO);

            // Ensure purchase date is set to today if user is assigned
            if (!string.IsNullOrEmpty(ticket.UserID))
            {
                ticket.PurchaseDate = DateTime.Today;
            }

            _ticketRepository.Insert(ticket);
            _ticketRepository.Save();

            var createdTicket = _mapper.Map<TicketResponseDTO>(ticket);
            var response = new GeneralResponse
            {
                IsPass = true,
                Data = createdTicket
            };

            return CreatedAtAction(nameof(GetTicket), new { id = ticket.TicketID }, response);
        }

        // POST: api/Ticket/bulk 
        [HttpPost("bulk")]
        public ActionResult<GeneralResponse> CreateBulkTickets(
            [FromQuery] string eventId,
            [FromQuery] int count,
            [FromQuery] decimal price,
            [FromQuery] TicketType ticketType = TicketType.Regular)
        {
            if (count <= 0 || count > 1000)
            {
                var generalresponse = new GeneralResponse
                {
                    IsPass = false,
                    Data = null
                };
                return BadRequest(generalresponse);
            }

            var tickets = Enumerable.Range(0, count)
                .Select(_ => new TicketCreateDTO
                {
                    EventID = eventId,
                    Price = price,
                    TicketType = ticketType
                })
                .Select(dto => _mapper.Map<Ticket>(dto))
                .ToList();

            tickets.ForEach(t => _ticketRepository.Insert(t));
            _ticketRepository.Save();

            var ticketDTOs = _mapper.Map<List<TicketResponseDTO>>(tickets);
            var response = new GeneralResponse
            {
                IsPass = true,
                Data = ticketDTOs
            };

            return CreatedAtAction(nameof(GetTicketsByEvent), new { eventId = eventId }, response);
            
        }

        // PUT: api/Ticket/5
        [HttpPut("{id}")]
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
                return NotFound(generalresponse);
            }

            //if (!string.IsNullOrEmpty(ticket.UserID) &&
            //    ticket.UserID != User.Identity?.Name &&
            //    !User.IsInRole("Admin"))
            //{
            //    var response = new GeneralResponse
            //    {
            //        IsPass = false,
            //        Message = "Access denied",
            //        Data = null
            //    };
            //    return Forbid();
            //}

            _mapper.Map(ticketDTO, ticket);
            _ticketRepository.Update(ticket);
            _ticketRepository.Save();

            var response = new GeneralResponse
            {
                IsPass = true,
                Data = null
            };

            return Ok(response);
        }

        //[Authorize]
        [HttpPost("purchase/event/{eventId}")]
        public ActionResult<GeneralResponse> PurchaseNextAvailableTicket(string userId, string eventId, int numOftickets)
        {
            var ticket = _ticketRepository.GetTicketsByEventId(eventId)
                            .FirstOrDefault(t => t.TicketStatus == TicketStatus.Available);

            if (ticket == null)
            {
                var generalresponse = new GeneralResponse
                {
                    IsPass = false,
                    
                    Data = null
                };
                return NotFound(generalresponse);
            }

            //var userId = User.Identity?.Name;
            //if (string.IsNullOrEmpty(userId))
            //{
            //    var response = new GeneralResponse
            //    {
            //        IsPass = false,
            //        Message = "User must be logged in to purchase a ticket.",
            //        Data = null
            //    };
            //    return Unauthorized(response);
            //}

            ticket.UserID = userId;
            ticket.PurchaseDate = DateTime.UtcNow;
            ticket.SeatNumber = numOftickets;
            ticket.TicketStatus = TicketStatus.SoldOut;
            //var eventinfo /*=/* _eventRepository.GetById(/*eventId*//*)*/;//----Add this to eventRepo*/*/
            //eventinfo.AvailableTickets -= numOftickets;
            //_eventRepository.Update(eventinfo);
            //_eventRepository.Save();

            _ticketRepository.Update(ticket);
            _ticketRepository.Save();

            var ticketDTO = _mapper.Map<TicketResponseDTO>(ticket);
            var response = new GeneralResponse
            {
                IsPass = true,
               
                Data = ticketDTO
            };

            return Ok(response);
        }

        [HttpGet("type/{type}")]
        public ActionResult<GeneralResponse> GetTicketsByType(TicketType type)
        {
            var tickets = _ticketRepository.GetTicketsByType(type);
            var ticketDTOs = _mapper.Map<List<TicketResponseDTO>>(tickets);

            var response = new GeneralResponse
            {
                IsPass = true,
               
                Data = ticketDTOs
            };

            return Ok(response);
        }

        [HttpDelete]
        public ActionResult<GeneralResponse> DeleteTicket(string id)
        {
            var ticket = _ticketRepository.GetById(id);
            if (ticket == null)
            {
                var generalresponse = new GeneralResponse
                {
                    IsPass = false,
                   
                    Data = null
                };
                return NotFound(generalresponse);
            }

            //if (!string.IsNullOrEmpty(ticket.UserID) &&
            //    ticket.UserID != User.Identity?.Name &&
            //    !User.IsInRole("Admin"))
            //{
            //    var response = new GeneralResponse
            //    {
            //        IsPass = false,
            //        Message = "Access denied",
            //        Data = null
            //    };
            //    return Forbid();
            //}

            _ticketRepository.DeleteById(id);
            _ticketRepository.Save();

            var response = new GeneralResponse
            {
                IsPass = true,
               
                Data = null
            };

            return Ok(response);
        }
    }
}