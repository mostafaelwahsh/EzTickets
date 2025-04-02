public class OrderDto
{
    public string OrderID { get; set; }
    public string UserID { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal? DiscountAmount { get; set; }
    public string OrderStatus { get; set; }
    public string PaymentMethod { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<string> TicketIds { get; set; }
}
