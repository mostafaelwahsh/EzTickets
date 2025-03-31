using Models;

namespace EzTickets.DTO
{
    public class TicketCreateDTO
    {
        public string?TicketID { get; set; }
        public string EventID { get; set; }
        public string? UserID { get; set; }
        public TicketType TicketType { get; set; } = TicketType.Regular;
        public decimal Price { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int? SeatNumber { get; set; }
    }
}
