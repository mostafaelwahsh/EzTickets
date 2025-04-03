using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public enum EventStatus : byte
    {
        Draft,
        Published,
        Canceled,
        Completed
    }
    public enum EventCategoryType : byte
    {
        Music,
        Sports,
        Conference,
        Theater,
        Other
    }
    public class Event
    {
        [Key]
        public int EventID { get; set; }    
        public string EventName { get; set; }
        public string Description { get; set; }
        public string VenueName { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalTickets { get; set; }
        public int AvailableTickets { get; set; }
        public decimal PricePerTicket { get; set; }
        public string? ImageUrl { get; set; }
        public EventStatus Status { get; set; } = EventStatus.Draft;

        public EventCategoryType Category { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    }
}
