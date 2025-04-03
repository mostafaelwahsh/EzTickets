using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public enum OrderStatus : byte
    {
        Pending,
        Paid,
        Canceled
    }
    public enum PaymentMethod : byte
    {
        CreditCard,
        DebitCard,
        BankTransfer,
        Fawry,
        InstaPay
    }
    public class Order
    {
        [Key]
        public int OrderId { get; set; } 

        [ForeignKey("User")]
        public string UserID { get; set; }
        public ApplicationUser User { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal? DiscountAmount { get; set; }
        public OrderStatus OrderStatus { get; set; } 
        public PaymentMethod PaymentMethod { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; }

        // Navigation property for tickets in the order
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
