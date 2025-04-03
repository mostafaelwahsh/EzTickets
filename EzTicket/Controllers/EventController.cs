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