using Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace EzTickets.DTO.Admin
{
    public class EventAdminUpdateDTO
    {
        [Required]
        public int EventID { get; set; }

        [Required]
        public string EventName { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string VenueName { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int TotalTickets { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal PricePerTicket { get; set; }

        public string? ImageUrl { get; set; }

        [Required]
        public EventStatus Status { get; set; }

        [Required]
        public EventCategoryType Category { get; set; }
    }
}