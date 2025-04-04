using AutoMapper;
using EzTickets.DTO.Pagination;
using EzTickets.DTO.Public;
using EzTickets.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        // GET: api/event (for public view)
        [HttpGet]
        public ActionResult<GeneralResponse> GetAllEventsPublic([FromQuery] PaginationParams pagination)
        {
            try
            {
                var pagedEvents = _orderRepository.GetAll(pagination);
                var result = new PagedResponse<OrderDTO>(
                    _mapper.Map<List<OrderDTO>>(pagedEvents.Data),
                    pagedEvents.PageNumber,
                    pagedEvents.PageSize,
                    pagedEvents.TotalRecords);

                return new GeneralResponse
                {
                    IsPass = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse
                {
                    IsPass = false,
                };
            }
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
        //public Order CreateOrder(int eventId, int ticketCount, string userId)
        //{
        //    // Get available tickets
        //    var availableTickets = _context.Tickets
        //        .Where(t => t.EventID == eventId && t.TicketStatus == TicketStatus.Available)
        //        .Take(ticketCount)
        //        .ToList();

        //    // Create order
        //    var order = new Order
        //    {
        //        UserID = userId,
        //        OrderStatus = OrderStatus.Pending,
        //        Tickets = availableTickets,
        //        TotalAmount = availableTickets.Sum(t => t.Price),
        //        CreatedAt = DateTime.UtcNow
        //    };

        //    // Update tickets
        //    foreach (var ticket in availableTickets)
        //    {
        //        ticket.TicketStatus = TicketStatus.SoldOut;
        //        ticket.OrderID = order.OrderId;
        //        ticket.PurchaseDate = DateTime.UtcNow;
        //    }

        //    // Update event
        //    var @event = _context.Events.Find(eventId);
        //    @event.AvailableTickets -= ticketCount;

        //    _context.SaveChanges();
        //    return order;
        //}

        //#endregion

        //#region PUT

        //[HttpPut("{id}")]
        //public IActionResult Update(int id, [FromBody] UpdateOrderDto dto)
        //{
        //    var existing = _orderRepository.GetById(id);
        //    if (existing == null)
        //        return NotFound(ApiResponse<OrderDTO>.Fail("Order not found"));

        //    _mapper.Map(dto, existing);
        //    _orderRepository.Update(existing);
        //    _orderRepository.Save();

        //    var resultDto = _mapper.Map<OrderDTO>(existing);
        //    return Ok(ApiResponse<OrderDTO>.Ok(resultDto, "Order updated"));
        //}

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
