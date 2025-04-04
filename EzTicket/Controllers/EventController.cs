using AutoMapper;
using EzTickets.DTO.Admin;
using EzTickets.DTO.Pagination;
using EzTickets.DTO.Public;
using EzTickets.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace EzTickets.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;

        public EventController(IEventRepository eventRepository, IMapper mapper)
        {
            _eventRepository = eventRepository;
            _mapper = mapper;
        }

        #region Public Endpoints

        [HttpGet]
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

        [HttpGet("{id}")]
        public ActionResult<GeneralResponse> GetEventById(int id)
        {
            try
            {
                var eventItem = _eventRepository.GetByIdPublic(id);
                return new GeneralResponse
                {
                    IsPass = true,
                    Data = _mapper.Map<EventPublicDetailsDTO>(eventItem)
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

        [HttpGet("filter")]
        public ActionResult<GeneralResponse> GetFilteredEvents([FromQuery] EventPublicFilterDTO filter)
        {
            try
            {
                var result = _eventRepository.GetFilteredPublicEvents(
                    searchQuery: filter.SearchQuery,
                    category: filter.Category,
                    city: filter.City,
                    venue: filter.VenueName,
                    minPrice: filter.MinPrice,
                    maxPrice: filter.MaxPrice,
                    startDate: filter.StartDate,
                    endDate: filter.EndDate,
                    pagination: filter
                );

                return new GeneralResponse
                {
                    IsPass = true,
                    Data = new PagedResponse<EventPublicListDTO>(
                        _mapper.Map<List<EventPublicListDTO>>(result.Data),
                        result.PageNumber,
                        result.PageSize,
                        result.TotalRecords)
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

        #endregion

        #region Admin Endpoints

        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public ActionResult<GeneralResponse> GetAllEventsAdmin([FromQuery] PaginationParams pagination)
        {
            try
            {
                var pagedEvents = _eventRepository.GetAll(pagination);
                var result = new PagedResponse<EventAdminResponseDTO>(
                    _mapper.Map<List<EventAdminResponseDTO>>(pagedEvents.Data),
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

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult<GeneralResponse> CreateEvent([FromBody] EventAdminCreateDTO eventDto)
        {
            try
            {
                var eventItem = _mapper.Map<Event>(eventDto);
                _eventRepository.Insert(eventItem);
                _eventRepository.Save();

                return new GeneralResponse
                {
                    IsPass = true,
                    Data = _mapper.Map<EventAdminResponseDTO>(eventItem)
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

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/publish")]
        public async Task<IActionResult> PublishEvent(int id)
        {
            var success = await _eventRepository.PublishEvent(id);

            if (!success)
                return BadRequest("Event not found or already published");

            return Ok(new
            {
                Message = "Event published successfully",
                EventId = id
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public ActionResult<GeneralResponse> UpdateEvent(int id, [FromBody] EventAdminUpdateDTO eventDto)
        {
            try
            {
                if (id != eventDto.EventID)
                    return new GeneralResponse
                    {
                        IsPass = false,
                    };

                var existingEvent = _eventRepository.GetById(id);
                if (existingEvent == null)
                    return new GeneralResponse
                    {
                        IsPass = false,
                    };

                _mapper.Map(eventDto, existingEvent);
                _eventRepository.Update(existingEvent);
                _eventRepository.Save();

                return new GeneralResponse
                {
                    IsPass = true,
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


        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public ActionResult<GeneralResponse> DeleteEvent(int id)
        {
            try
            {
                _eventRepository.Delete(id);
                _eventRepository.Save();
                return new GeneralResponse
                {
                    IsPass = true,
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

        [Authorize(Roles = "Admin")]
        [HttpDelete("soft/{id}")]
        public ActionResult<GeneralResponse> SoftDeleteEvent(int id)
        {
            try
            {
                _eventRepository.SoftDeleteEvent(id);
                _eventRepository.Save();
                return new GeneralResponse
                {
                    IsPass = true,
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

        [Authorize(Roles = "Admin")]
        [HttpGet("count")]
        public ActionResult<GeneralResponse> GetEventsCount()
        {
            try
            {
                var count = _eventRepository.GetTotalEventsCount();
                return new GeneralResponse
                {
                    IsPass = true,
                    Data = new { Count = count }
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
        #endregion
    }
}
