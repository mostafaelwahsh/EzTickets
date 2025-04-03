using AutoMapper;
using EzTickets.DTO.Pagination;
using EzTickets.DTO.Public;
using EzTickets.Repository;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace EzTickets.Controllers
{

    [Route("api/[controller]")]
    [Produces("application/json")]
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
        [ProducesResponseType(typeof(ApiResponse<List<OrderDto>>), StatusCodes.Status200OK)]
        public IActionResult GetAll()
        {
            var orders = _repository.GetAll();
            var result = _mapper.Map<List<OrderDto>>(orders);
            return Ok(ApiResponse<List<OrderDto>>.Ok(result));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<OrderDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<OrderDto>), StatusCodes.Status404NotFound)]
        public IActionResult GetById(int id)
        {
            var order = _repository.GetById(id);
            if (order == null)
                return NotFound(ApiResponse<OrderDto>.Fail("Order not found"));

            var dto = _mapper.Map<OrderDto>(order);
            return Ok(ApiResponse<OrderDto>.Ok(dto));
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
        [ProducesResponseType(typeof(ApiResponse<OrderDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<OrderDto>), StatusCodes.Status400BadRequest)]
        public IActionResult Create([FromBody] CreateOrderDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<OrderDto>.Fail("Invalid order data"));

            var order = _mapper.Map<Order>(dto);

            // Map the DTO to the Order entity
            var order = _mapper.Map<Order>(orderDTO);
            _orderRepository.Insert(order);
            _orderRepository.Save();

            var responseDto = _mapper.Map<OrderDto>(order);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, ApiResponse<OrderDto>.Ok(responseDto, "Order created"));
        }

        #endregion

        #region PUT Method

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateOrderDto dto)
        {
            var existing = _repository.GetAll().FirstOrDefault(o => o.Id == id);
            if (existing == null) return NotFound();

            // Map the updated DTO back to the Order entity
            _mapper.Map(orderDTO, order);
            _orderRepository.Update(order);
            _orderRepository.Save();

            return Ok(ApiResponse<OrderDto>.Ok(_mapper.Map<OrderDto>(existing), "Order updated"));
        }

        #endregion

        #region DELETE Method

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        public IActionResult Delete(int id)
        {
            var order = _repository.GetAll().FirstOrDefault(o => o.Id == id);
            if (order == null) return NotFound();

            _repository.Delete(order.Id);
            _repository.Save();

            return Ok(ApiResponse<string>.Ok(existing.Id.ToString(), "Order deleted"));
        }

        #endregion
    }

}
