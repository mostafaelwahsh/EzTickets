using AutoMapper;
using EzTickets.DTO.Public;
using EzTickets.Repository;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace EzTickets.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly IMapper _mapper;

        public OrderController(IOrderRepository orderRepository, ITicketRepository ticketRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _ticketRepository = ticketRepository;
            _mapper = mapper;
        }

        #region GET Methods

        [HttpGet]
        public IActionResult GetAll()
        {
            var orders = _orderRepository.GetAll();
            var result = _mapper.Map<List<OrderDTO>>(orders);
            return Ok(ApiResponse<List<OrderDTO>>.Ok(result));
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var order = _orderRepository.GetById(id);
            if (order == null)
                return NotFound(ApiResponse<OrderDTO>.Fail("Order not found"));

            var dto = _mapper.Map<OrderDTO>(order);
            return Ok(ApiResponse<OrderDTO>.Ok(dto));
        }

        [HttpGet("user/{userId}")]
        public IActionResult GetByUserId(string userId)
        {
            var orders = _orderRepository.GetByUserId(userId);
            var result = _mapper.Map<List<OrderDTO>>(orders);
            return Ok(ApiResponse<List<OrderDTO>>.Ok(result));
        }

        [HttpGet("expiring")]
        public IActionResult GetExpiring([FromQuery] DateTime targetDate)
        {
            var orders = _orderRepository.GetExpiringOrders(targetDate);
            var result = _mapper.Map<List<OrderDTO>>(orders);
            return Ok(ApiResponse<List<OrderDTO>>.Ok(result));
        }

        #endregion

        #region POST

        [HttpPost]
        public IActionResult Create([FromBody] CreateOrderDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<OrderDTO>.Fail("Invalid data"));

            var order = _mapper.Map<Order>(dto);

            // Load Ticket objects
            var tickets = _ticketRepository.GetAll()
                .Where(t => dto.TicketIds.Contains(t.TicketID))
                .ToList();

            order.Tickets = tickets;

            _orderRepository.Insert(order);
            _orderRepository.Save();

            var resultDto = _mapper.Map<OrderDTO>(order);
            return CreatedAtAction(nameof(GetById), new { id = order.OrderId }, ApiResponse<OrderDTO>.Ok(resultDto, "Order created"));
        }

        #endregion

        #region PUT

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateOrderDto dto)
        {
            var existing = _orderRepository.GetById(id);
            if (existing == null)
                return NotFound(ApiResponse<OrderDTO>.Fail("Order not found"));

            _mapper.Map(dto, existing);
            _orderRepository.Update(existing);
            _orderRepository.Save();

            var resultDto = _mapper.Map<OrderDTO>(existing);
            return Ok(ApiResponse<OrderDTO>.Ok(resultDto, "Order updated"));
        }

        #endregion

        #region DELETE

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var existing = _orderRepository.GetById(id);
            if (existing == null)
                return NotFound(ApiResponse<string>.Fail("Order not found"));

            _orderRepository.Delete(id);
            _orderRepository.Save();

            return Ok(ApiResponse<string>.Ok(existing.OrderId.ToString(), "Order deleted"));
        }

        #endregion
    }
}
