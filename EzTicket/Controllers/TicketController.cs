using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Models;
using EzTickets.Repository;
using EzTickets.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace EzTickets.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IMapper _mapper;

        public TicketController(ITicketRepository ticketRepository, IMapper mapper)
        {
            _ticketRepository = ticketRepository;
            _mapper = mapper;
        }

        // GET: api/Ticket
        [HttpGet]
        public ActionResult<IEnumerable<TicketResponseDTO>> GetAllTickets()
        {
            var tickets = _ticketRepository.GetAll();
            return Ok(_mapper.Map<List<TicketResponseDTO>>(tickets));
        }

        // GET: api/Ticket/5
        [HttpGet("{id}")]
        public ActionResult<TicketResponseDTO> GetTicket(string id)
        {
            var ticket = _ticketRepository.GetById(id);

            if (ticket == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<TicketResponseDTO>(ticket));
        }

        // GET: api/Ticket/event/5
        [HttpGet("event/{eventId}")]
        public ActionResult<IEnumerable<TicketResponseDTO>> GetTicketsByEvent(string eventId)
        {
            var tickets = _ticketRepository.GetTicketsByEventId(eventId);
            return Ok(_mapper.Map<List<TicketResponseDTO>>(tickets));
        }

        // GET: api/Ticket/user/userid
        [HttpGet("user/{userId}")]
        public ActionResult<IEnumerable<TicketWithEventDTO>> GetTicketsByUser(string userId)
        {
            //if (userId != User.Identity?.Name && !User.IsInRole("Admin"))
            //{
            //    return Forbid();
            //}

            var tickets = _ticketRepository.GetTicketsByUserId(userId);
            return Ok(_mapper.Map<List<TicketWithEventDTO>>(tickets));
        }

        // POST: api/Ticket
        [HttpPost]
        public ActionResult<TicketResponseDTO> CreateTicket(TicketCreateDTO ticketDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ticket = _mapper.Map<Ticket>(ticketDTO);

            // Ensure purchase date is set to today if user is assigned
            if (!string.IsNullOrEmpty(ticket.UserID))
            {
                ticket.PurchaseDate = DateTime.Today;
            }

            _ticketRepository.Insert(ticket);
            _ticketRepository.Save();

            return CreatedAtAction(
                nameof(GetTicket),
                new { id = ticket.TicketID },
                _mapper.Map<TicketResponseDTO>(ticket));
        }

        // POST: api/Ticket/bulk
        [HttpPost("bulk")]
        public ActionResult<IEnumerable<TicketResponseDTO>> CreateBulkTickets(
            [FromQuery] string eventId,
            [FromQuery] int count,
            [FromQuery] decimal price,
            [FromQuery] TicketType ticketType = TicketType.Regular)
        {
            if (count <= 0 || count > 1000)
            {
                return BadRequest("Count must be between 1 and 1000");
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

            return CreatedAtAction(
                nameof(GetTicketsByEvent),
                new { eventId = eventId },
                _mapper.Map<List<TicketResponseDTO>>(tickets));
        }

        // PUT: api/Ticket/5
        [HttpPut("{id}")]
        public IActionResult UpdateTicket(string id, TicketUpdateDTO ticketDTO)
        {
            var ticket = _ticketRepository.GetById(id);
            if (ticket == null)
            {
                return NotFound();
            }

            //if (!string.IsNullOrEmpty(ticket.UserID) &&
            //    ticket.UserID != User.Identity?.Name &&
            //    !User.IsInRole("Admin"))
            //{
            //    return Forbid();
            //}

            _mapper.Map(ticketDTO, ticket);
            _ticketRepository.Update(ticket);
            _ticketRepository.Save();

            return NoContent();
        }

        // Other actions remain the same...
        // (GET status/type/count/sales, PUT status/assign, GET qrcode, DELETE)
    }
}