using AutoMapper;
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
        private readonly IOrderRepository _repository;
        private readonly IMapper _mapper;

        public OrderController(IOrderRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

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

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<OrderDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<OrderDto>), StatusCodes.Status400BadRequest)]
        public IActionResult Create([FromBody] CreateOrderDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<OrderDto>.Fail("Invalid order data"));

            var order = _mapper.Map<Order>(dto);

            _repository.Insert(order);
            _repository.Save();

            var responseDto = _mapper.Map<OrderDto>(order);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, ApiResponse<OrderDto>.Ok(responseDto, "Order created"));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<OrderDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<OrderDto>), StatusCodes.Status404NotFound)]
        public IActionResult Update(int id, [FromBody] UpdateOrderDto dto)
        {
            var existing = _repository.GetById(id);
            if (existing == null)
                return NotFound(ApiResponse<OrderDto>.Fail("Order not found"));

            _mapper.Map(dto, existing);
            _repository.Update(existing);
            _repository.Save();

            return Ok(ApiResponse<OrderDto>.Ok(_mapper.Map<OrderDto>(existing), "Order updated"));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        public IActionResult Delete(int id)
        {
            var existing = _repository.GetById(id);
            if (existing == null)
                return NotFound(ApiResponse<string>.Fail("Order not found"));

            _repository.Delete(id);
            _repository.Save();

            return Ok(ApiResponse<string>.Ok(existing.Id.ToString(), "Order deleted"));
        }
    }
}
