using AutoMapper;
using EzTickets.DTO.Pagination;
using EzTickets.DTO.Public;
using EzTickets.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace EzTickets.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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

        [HttpGet("all-orders")]
        public ActionResult<GeneralResponse> GetAllOrderPaginated([FromQuery] PaginationParams pagination)
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
        public ActionResult<GeneralResponse> GetById(int id)
        {
            var order = _orderRepository.GetById(id);
            if (order == null)
            {
                return new GeneralResponse
                {
                    Data = "There is no order with this ID",
                    IsPass = false
                };
            }
            var dto = _mapper.Map<OrderDTO>(order);
            return new GeneralResponse
            {
                Data = dto,
                IsPass = true,
            };
        }

        [HttpGet("user/{userId}")]
        public ActionResult<GeneralResponse> GetByUserId(string userId)
        {
            var orders = _orderRepository.GetByUserId(userId);
            var result = _mapper.Map<List<OrderDTO>>(orders);
            return new GeneralResponse
            {
                Data = result,
                IsPass = true,
            };
        }

        [Authorize]
        [HttpPost]
        public ActionResult<GeneralResponse> CreateOrder(int eventId, int ticketCount, string userId, decimal discount = 0)
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

            Order orderdone = _orderRepository.LastOrder();

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
                Data = "Order Successfully Created"
            };

            return respone;
        }

        [HttpPut("{id}")]
        public ActionResult<GeneralResponse> Update(int id, [FromBody] UpdateOrderDTO dto)
        {
            var existing = _orderRepository.GetById(id);
            if (existing == null)
            {
                return new GeneralResponse
                {
                    Data = "There is no order with this ID",
                    IsPass = false
                };
            }

            _mapper.Map(dto, existing); 

            _orderRepository.Update(existing);
            _orderRepository.Save();

            var resultDto = _mapper.Map<OrderDTO>(existing);

            return new GeneralResponse
            {
                Data = resultDto,
                IsPass = true,
            };
        }

        [Authorize]
        [HttpDelete("{id}")]
        public ActionResult<GeneralResponse> Delete(int id)
        {
            var existing = _orderRepository.GetById(id);
            if (existing == null)
            {
                return new GeneralResponse
                {
                    IsPass = false,
                    Data = "Order Not Found"
                };
            }

            var ticketsinorder = _ticketRepository.GetAllTicketsByOrderID(id);
            foreach (Ticket ticket in ticketsinorder)
            {
                ticket.TicketStatus = TicketStatus.Available;
            }
            _ticketRepository.UpdateRangeofTickets(ticketsinorder);
            _ticketRepository.Save();

            // Update event
            var _event = _eventRepository.GetById(ticketsinorder[0].EventID);
            _event.AvailableTickets += ticketsinorder.Count;

            _eventRepository.Update(_event);
            _eventRepository.Save();

            _orderRepository.IsDeletedTrue(id);
            _orderRepository.Update(existing);
            _orderRepository.Save();

            return new GeneralResponse
            {
                IsPass = true,
                Data = "Order Is Deleted Successfully"
            };
        }
    }
}
