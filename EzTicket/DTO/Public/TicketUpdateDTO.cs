using Models;

namespace EzTickets.DTO.Public
{
    public class TicketUpdateDTO
    {
        public string? TicketID { get; set; }
        public string? UserID { get; set; }
        public TicketStatus TicketStatus { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public bool IsDeleted { get; set; }
        public int? SeatNumber { get; set; }
        //public string? FullName { get; set; }

    }
}
