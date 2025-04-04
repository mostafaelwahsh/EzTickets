using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Models;
using EzTickets.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using EzTickets.DTO.Public;
using Microsoft.EntityFrameworkCore;
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
        private readonly IOrderRepository orderRepository;

        public TicketController(ITicketRepository ticketRepository, IMapper mapper, IEventRepository eventRepository, IOrderRepository orderRepository, DataContext dataContext)
        {
            _ticketRepository = ticketRepository;
            _mapper = mapper;
            _eventRepository = eventRepository;
            this.orderRepository = orderRepository;
        }

        #region Public Actions

        // GET: api/Ticket
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
        public ActionResult<GeneralResponse> GetTicketsByEvent(int eventId, [FromQuery] TicketStatus ticketStatus)
        {
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

        #endregion

        #region Admin Actions

        // POST: api/Ticket/bulk
        [HttpPost("bulk")]
        [Authorize(Roles = "Admin")]
        public ActionResult<GeneralResponse> CreateBulkTickets([FromQuery] int eventId, [FromQuery] int count, [FromQuery] decimal price, [FromQuery] TicketType ticketType = TicketType.Regular)
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

        // DELETE: api/Ticket/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
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

            _ticketRepository.DeleteById(id);
            _ticketRepository.Save();

            var response = new GeneralResponse
            {
                IsPass = true,
                Data = null
            };

            return Ok(response);
        }

        #endregion

        #region User Actions
        #region GetMyTickets

        [HttpGet("mytickets")]
        [Authorize(Roles = "User")]
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

            return Ok(response);
        }

        #endregion
        #region Purchase Actions

        [HttpPost("purchase/event/{eventId}")]
        [Authorize(Roles = "User")]
        public ActionResult<GeneralResponse> PurchaseTickets(int eventId, int numOfTickets)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var availableTickets = _ticketRepository.GetTicketsByEventId(eventId)
                                      .Where(t => t.TicketStatus == TicketStatus.Available)
                                      .Take(numOfTickets)
                                      .ToList();

            if (availableTickets.Count < numOfTickets)
            {
                return NotFound(new GeneralResponse
                {
                    IsPass = false,
                    Data = null
                });
            }

            var order = new Order
            {
                UserID = userId,
                CreatedAt = DateTime.UtcNow,
                TotalAmount = availableTickets.Sum(t => t.Price), 
                OrderStatus = OrderStatus.Pending 
            };

            orderRepository.Insert(order);
            orderRepository.Save(); 

            foreach (var ticket in availableTickets)
            {
                ticket.UserID = userId;
                ticket.PurchaseDate = DateTime.UtcNow;
                ticket.TicketStatus = TicketStatus.SoldOut;
                ticket.OrderID = order.OrderId; // Assign the generated OrderID

                _ticketRepository.Update(ticket);
            }

            _ticketRepository.Save();

          
            var ticketDTOs = _mapper.Map<List<TicketResponseDTO>>(availableTickets);
            return Ok(new GeneralResponse
            {
                IsPass = true,
                Data = new
                {
                    OrderId = order.OrderId,
                    Tickets = ticketDTOs
                }
            });
        }

        #endregion


        // GET: api/Ticket/user/userid
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "User, Admin")]
        public ActionResult<GeneralResponse> GetTicketsByUser(string userId)
        {
            var tickets = _ticketRepository.GetTicketsByUserId(userId);
            var ticketDTOs = _mapper.Map<List<TicketWithEventDTO>>(tickets);

            var response = new GeneralResponse
            {
                IsPass = true,
                Data = ticketDTOs
            };

            return Ok(response);
        }

       
        [HttpPut("{id}")]
        [Authorize(Roles = "User, Admin")]
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

            _mapper.Map(ticketDTO, ticket);
            _ticketRepository.Update(ticket);
            _ticketRepository.Save();

            var response = new GeneralResponse
            {
                IsPass = true,
                Data = ticket
            };

            return Ok(response);
        }

        #endregion

       

    }
}
