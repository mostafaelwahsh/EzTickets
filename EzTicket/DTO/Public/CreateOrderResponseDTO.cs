namespace EzTickets.DTO.Public
{
    public class CreateOrderResponseDTO
    {
        public int OrderId { get; set; }
        public string UserID { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal? DiscountAmount { get; set; }
        public string OrderStatus { get; set; }
        public string? PaymentMethod { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TicketCount { get; set; }
        public int NumberOfTickets { get; set; }
        public List<int>? TicketIds { get; set; }

    }
}
