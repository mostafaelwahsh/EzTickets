using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public enum TicketStatus : byte
    {
        Available, 
        SoldOut, 
        Expired 
    }
    public enum TicketType : byte
    {
        Regular, 
        VIP 
    }
    public class Ticket
    {
        [Key]
        public string TicketID { get; set; } = Guid.NewGuid().ToString();

        [ForeignKey("Event")]
        public int EventID { get; set; }
        public Event Event { get; set; }

        [ForeignKey("User")]
        public string? UserID { get; set; } 
        public ApplicationUser? User { get; set; }

        [ForeignKey("Order")]
        public int? OrderID { get; set; }
        public Order? Order { get; set; }
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
