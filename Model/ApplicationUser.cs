using Microsoft.AspNetCore.Identity;

namespace Models;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public string? BloodType { get; set; }
    public string? City { get; set; }
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();

}
