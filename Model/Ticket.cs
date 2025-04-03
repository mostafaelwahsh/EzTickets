using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public enum TicketStatus : byte
    {
        Available, //0
        SoldOut, //1
        Expired //2
    }
    public enum TicketType : byte
    {
        Regular, //0
        VIP //1
    }
    public class Ticket
    {
        [Key]
        public string TicketID { get; set; } = Guid.NewGuid().ToString();

        [ForeignKey("Event")]
        public int EventID { get; set; }
        public Event Event { get; set; }

        [ForeignKey("User")]
        public string? UserID { get; set; } // Nullable because not all tickets may be sold
        public ApplicationUser? User { get; set; }
        public TicketType TicketType { get; set; } = TicketType.Regular;
        public decimal Price { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public TicketStatus TicketStatus { get; set; }
        public int? SeatNumber { get; set; }
        public string? QRCode { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
    }
}
