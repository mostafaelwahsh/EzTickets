using Models;

namespace EzTickets.DTO.Public
{
    public class EventPublicListDTO
    {
        public int EventID { get; set; }
        public string EventName { get; set; }
        public string VenueName { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public DateTime StartDate { get; set; }
        public decimal PricePerTicket { get; set; }
        public string? ImageUrl { get; set; }
        public EventCategoryType Category { get; set; }
    }
}