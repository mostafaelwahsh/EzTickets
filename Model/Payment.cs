using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public enum PaymentStatus : byte
    {
        Success,
        Failed,
        Pending
    }
    public class Payment
    {
        [Key]
        public string PaymentID { get; set; } = Guid.NewGuid().ToString();

        public string CardHolderName { get; set; }

        public string MaskedCardNumber { get; set; }

        public string ExpiryDate { get; set; }

        public decimal Amount { get; set; }
        public string Currency { get; set; } = "EGP";

        public Guid TransactionID { get; set; } = Guid.NewGuid();

        public PaymentStatus Status { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Now;

        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [ForeignKey("Order")]
        public int? OrderID { get; set; } // Nullable if payment is pending
        public Order? Order { get; set; }
    }
}
