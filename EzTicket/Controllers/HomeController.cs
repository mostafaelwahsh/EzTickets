using AutoMapper;
using EzTickets.DTO.Pagination;
using EzTickets.DTO.Public;
using EzTickets.Repository;
using Microsoft.AspNetCore.Mvc;

namespace EzTickets.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;

        public HomeController(IEventRepository eventRepository, IMapper mapper)
        {
            _eventRepository = eventRepository;
            _mapper = mapper;
        }

        public ActionResult<GeneralResponse> GetAllEventsPublic([FromQuery] PaginationParams pagination)
        {
            try
            {
                var pagedEvents = _eventRepository.GetAllPublic(pagination);
                var result = new PagedResponse<EventPublicListDTO>(
                    _mapper.Map<List<EventPublicListDTO>>(pagedEvents.Data),
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
    }
}
