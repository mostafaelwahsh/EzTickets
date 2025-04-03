using AutoMapper;
using EzTickets.DTO.Admin;
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
        public ActionResult<GeneralResponse> GetAllEvents()
        {
            try
            {
                var events = _eventRepository.GetAll()
                    .Where(e => e.Status == EventStatus.Published);
                return new GeneralResponse
                {
                    IsPass = true,
                    Data = _mapper.Map<List<EventPublicListDTO>>(events)
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

        // GET: api/event/5
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
                IEnumerable<Event> events = _eventRepository.GetAllPublic();

                if (!string.IsNullOrEmpty(filter.SearchQuery))
                {
                    events = _eventRepository.GetEventsByName(filter.SearchQuery);
                }

                if (filter.Category.HasValue)
                {
                    events = events.Intersect(_eventRepository.GetEventsByCategory(filter.Category.Value));
                }

                if (!string.IsNullOrEmpty(filter.City))
                {
                    events = events.Intersect(_eventRepository.GetEventsByCity(filter.City));
                }

                if (!string.IsNullOrEmpty(filter.VenueName))
                {
                    events = events.Intersect(_eventRepository.GetEventsByVenue(filter.VenueName));
                }

                if (filter.MinPrice.HasValue || filter.MaxPrice.HasValue)
                {
                    events = events.Intersect(_eventRepository.GetEventsByPriceRange(
                        filter.MinPrice ?? 0,
                        filter.MaxPrice ?? decimal.MaxValue));
                }

                if (filter.StartDate.HasValue && filter.EndDate.HasValue)
                {
                    events = events.Intersect(_eventRepository.GetEventsByDateRange(
                        filter.StartDate.Value,
                        filter.EndDate.Value));
                }
                else if (filter.StartDate.HasValue)
                {
                    events = events.Intersect(_eventRepository.GetEventsByDate(filter.StartDate.Value));
                }

                var result = events.ToList();
                var mappedResults = _mapper.Map<List<EventPublicListDTO>>(result);

                return new GeneralResponse
                {
                    IsPass = true,
                    Data = mappedResults
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

        // POST: api/event
        //[Authorize(Roles = "Admin")]
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

        // PUT: api/event/5
        //[Authorize(Roles = "Admin")]
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

        // DELETE: api/event/5
        //[Authorize(Roles = "Admin")]
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
        
        //// DELETE: api/event/5
        ////[Authorize(Roles = "Admin")]
        //[HttpDelete("{id}")]
        //public ActionResult<GeneralResponse> SoftDeleteEvent(int id)
        //{
        //    try
        //    {
        //        _eventRepository.SoftDeleteEvent(id);
        //        _eventRepository.Save();
        //        return new GeneralResponse
        //        {
        //            IsPass = true,
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new GeneralResponse
        //        {
        //            IsPass = false,
        //        };
        //    }
        //}

        #endregion

        #region Utility Endpoints

        // GET: api/event/count
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