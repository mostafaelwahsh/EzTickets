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
        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;

        public OrderController(IOrderRepository orderRepository, ITicketRepository ticketRepository,
                                                             IEventRepository eventRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _ticketRepository = ticketRepository;
            _eventRepository = eventRepository;
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
        public ActionResult<GeneralResponse> CreateOrder(int eventId, int ticketCount, string userId,decimal discount=0)
        {
            // Get available tickets
            var availableTickets = _ticketRepository.GetTicketsByEventId(eventId);

            if (availableTickets == null || availableTickets.Count == 0)
                throw new InvalidOperationException("No available tickets for this event.");

            decimal ticketPrice = availableTickets[0].Price;
            decimal totalAmount = ticketPrice * ticketCount;
            decimal discountAmount = 0;

            if (discount > 0)
            {
                discountAmount = (totalAmount * discount) / 100;
                totalAmount -= discountAmount;
            }

            // Create order
            var order = new Order
            {
                UserID = userId,
                OrderStatus = OrderStatus.Pending,                
                TotalAmount = totalAmount,
                DiscountAmount = discountAmount,
                CreatedAt = DateTime.UtcNow,
                NumberOfTickets = ticketCount,
            };

            _orderRepository.Insert(order);
            _orderRepository.Save();

            Order orderdone =_orderRepository.LastOrder();
            
            // Update tickets
            foreach (var ticket in availableTickets)
            {
                ticket.TicketStatus = TicketStatus.SoldOut;
                ticket.OrderID = orderdone.OrderId;
                ticket.PurchaseDate = DateTime.UtcNow;
            }

            _ticketRepository.UpdateRangeofTickets(availableTickets);
            _ticketRepository.Save();

            // Update event
            var _event = _eventRepository.GetById(eventId);
            _event.AvailableTickets -= ticketCount;

            _eventRepository.Update(_event);
            _eventRepository.Save();

            var respone = new GeneralResponse
            {
                IsPass = true,
                Data = _mapper.Map<CreateOrderResponseDTO>(order)
            };

            return respone;
        }

        #endregion

        #region PUT

        //[HttpPut("{id}")]
        //public IActionResult Update(int id, [FromBody] UpdateOrderDTO dto)
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
