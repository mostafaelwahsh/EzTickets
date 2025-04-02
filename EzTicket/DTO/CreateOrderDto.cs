using System.ComponentModel.DataAnnotations;
using Models;

public class CreateOrderDto
{
    [Required]
    public string UserID { get; set; }

    [Required]
    public decimal TotalAmount { get; set; }

    public decimal? DiscountAmount { get; set; }

    [Required]
    public OrderStatus OrderStatus { get; set; }

    [Required]
    public PaymentMethod PaymentMethod { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public List<string> TicketIds { get; set; } = new();
}
