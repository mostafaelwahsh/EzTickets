using Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace EzTickets.DTO.Admin
{
    public class EventAdminUpdateDTO
    {
        public string EventName { get; set; }

        public string Description { get; set; }

        public string VenueName { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        [Range(1, 250)]
        public int TotalTickets { get; set; }

        [Range(0, 1000)]
        public decimal PricePerTicket { get; set; }

        public string? ImageUrl { get; set; }

        public EventStatus Status { get; set; }

        public EventCategoryType Category { get; set; }

    }
}