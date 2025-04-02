using AutoMapper;
using EzTickets.Repository;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace EzTickets.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
        public IActionResult GetAll()
        {
            var orders = _repository.GetAll();
            var result = _mapper.Map<List<OrderDto>>(orders);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            var order = _repository.GetAll().FirstOrDefault(o => o.OrderID == id);
            if (order == null) return NotFound();
            return Ok(_mapper.Map<OrderDto>(order));
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateOrderDto dto)
        {
            var order = _mapper.Map<Order>(dto);
            order.OrderID = Guid.NewGuid().ToString(); // Ensure OrderID is set

            _repository.Insert(order);
            _repository.Save();

            return CreatedAtAction(nameof(GetById), new { id = order.OrderID }, _mapper.Map<OrderDto>(order));
        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody] UpdateOrderDto dto)
        {
            var existing = _repository.GetAll().FirstOrDefault(o => o.OrderID == id);
            if (existing == null) return NotFound();

            _mapper.Map(dto, existing);
            _repository.Update(existing);
            _repository.Save();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var order = _repository.GetAll().FirstOrDefault(o => o.OrderID == id);
            if (order == null) return NotFound();

            _repository.Delete(int.Parse(order.OrderID));
            _repository.Save();

            return NoContent();
        }
    }
}
