using EzTickets.DTO.Public;
using Models;

namespace EzTickets.DTO.Public
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
        public string UserID { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal? DiscountAmount { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public List<TicketResponseDTO> Tickets { get; set; } = new List<TicketResponseDTO>();
    }
}
