using AutoMapper;
using EzTickets.DTO.Pagination;
using EzTickets.DTO.Public;
using EzTickets.Repository;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace EzTickets.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public OrderController(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        #region GET Methods

        [HttpGet]
        public IActionResult GetAllOrders([FromQuery] PaginationParams paginationParams)
        {
            var pagedOrders = _orderRepository.GetPagedOrders(paginationParams);
            var pagedOrdersDTO = _mapper.Map<PagedResponse<OrderDTO>>(pagedOrders);
            return Ok(pagedOrdersDTO);
        }

        [HttpGet("{id}")]
        public IActionResult GetOrderById(int id)
        {
            var order = _orderRepository.GetById(id);
            if (order == null)
            {
                return NotFound();
            }

            var orderDTO = _mapper.Map<OrderDTO>(order);
            return Ok(orderDTO);
        }

        [HttpGet("user/{userId}")]
        public IActionResult GetOrdersByUserId(string userId)
        {
            var orders = _orderRepository.GetByUserId(userId);
            if (orders == null || orders.Count == 0)
            {
                return NotFound();
            }

            var ordersDTO = _mapper.Map<List<OrderDTO>>(orders);
            return Ok(ordersDTO);
        }

        [HttpGet("expiring")]
        public IActionResult GetExpiringOrders([FromQuery] DateTime targetDate)
        {
            var orders = _orderRepository.GetExpiringOrders(targetDate);
            if (orders == null || orders.Count == 0)
            {
                return NotFound();
            }

            var ordersDTO = _mapper.Map<List<OrderDTO>>(orders);
            return Ok(ordersDTO);
        }

        #endregion

        #region POST Method

        [HttpPost]
        public IActionResult CreateOrder([FromBody] OrderDTO orderDTO)
        {
            if (orderDTO == null)
            {
                return BadRequest("Order data is null.");
            }

            // Map the DTO to the Order entity
            var order = _mapper.Map<Order>(orderDTO);
            _orderRepository.Insert(order);
            _orderRepository.Save();

            return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderId }, orderDTO);
        }

        #endregion

        #region PUT Method

        [HttpPut("{id}")]
        public IActionResult UpdateOrder(int id, [FromBody] OrderDTO orderDTO)
        {
            if (orderDTO == null || id != orderDTO.OrderId)
            {
                return BadRequest("Order data is invalid.");
            }

            var order = _orderRepository.GetById(id);
            if (order == null)
            {
                return NotFound();
            }

            // Map the updated DTO back to the Order entity
            _mapper.Map(orderDTO, order);
            _orderRepository.Update(order);
            _orderRepository.Save();

            return NoContent(); // Return 204 No Content to indicate success
        }

        #endregion

        #region DELETE Method

        [HttpDelete("{id}")]
        public IActionResult DeleteOrder(int id)
        {
            var order = _orderRepository.GetById(id);
            if (order == null)
            {
                return NotFound();
            }

            _orderRepository.Delete(id);
            _orderRepository.Save();

            return NoContent(); // Return 204 No Content to indicate success
        }

        #endregion
    }

}
