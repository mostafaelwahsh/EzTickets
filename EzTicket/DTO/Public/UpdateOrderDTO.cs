using Models;
using EzTickets.DTO.Public;


public class UpdateOrderDTO
{
    public OrderStatus? OrderStatus { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public bool? IsDeleted { get; set; }
}
