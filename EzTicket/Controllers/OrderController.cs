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
        public IActionResult GetById(int id)
        {
            var order = _repository.GetById(id);
            if (order == null) return NotFound();
            return Ok(_mapper.Map<OrderDto>(order));
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateOrderDto dto)
        {
            var order = _mapper.Map<Order>(dto);

            _repository.Insert(order);
            _repository.Save();

            return Ok();
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateOrderDto dto)
        {
            var existing = _repository.GetAll().FirstOrDefault(o => o.Id == id);
            if (existing == null) return NotFound();

            _mapper.Map(dto, existing);
            _repository.Update(existing);
            _repository.Save();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var order = _repository.GetAll().FirstOrDefault(o => o.Id == id);
            if (order == null) return NotFound();

            _repository.Delete(order.Id);
            _repository.Save();

            return NoContent();
        }
    }
}
