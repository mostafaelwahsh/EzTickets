﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Models;
using EzTickets.Repository;
using EzTickets.DTO.Admin;
using EzTickets.DTO.Public;
using System;
using System.Collections.Generic;
using AutoMapper;
using System.Linq;

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

        // GET: api/event
        [HttpGet]
        public IActionResult GetAllEvents()
        {
            try
            {
                var events = _eventRepository.GetAllPublic();
                return Ok(_mapper.Map<List<EventPublicListDTO>>(events));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        #endregion
    }
}