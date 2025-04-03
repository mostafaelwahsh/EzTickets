using Models;
using System;

namespace EzTickets.DTO.Public
{
    public class EventPublicFilterDTO
    {
        public string? SearchQuery { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? VenueName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public EventCategoryType? Category { get; set; }
    }
}