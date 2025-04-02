using Models;

public class UpdateOrderDto
{
    public decimal? TotalAmount { get; set; }
    public decimal? DiscountAmount { get; set; }
    public OrderStatus? OrderStatus { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public bool? IsDeleted { get; set; }
}
