using Models;

namespace EzTickets.DTO
{
   
        public class TicketWithEventDTO
        {
            public string TicketID { get; set; }
            public string EventID { get; set; }
            public string EventName { get; set; }
            public string VenueName { get; set; }
            public string City { get; set; }
            public DateTime StartDate { get; set; }
            public TicketType TicketType { get; set; }
            public decimal Price { get; set; }
            public DateTime? PurchaseDate { get; set; }
            public TicketStatus TicketStatus { get; set; }
            public int? SeatNumber { get; set; }
            public string? QRCode { get; set; }
        }
    
}
