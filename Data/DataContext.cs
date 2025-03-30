using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Data
{
    public class DataContext : IdentityDbContext<ApplicationUser>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        // DbSet properties for your entities
        //public DbSet<Category> Category { get; set; }
        public DbSet<Event> Event { get; set; }
        //public DbSet<EventCategory> EventCategory { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<Payment> Payment { get; set; }
        public DbSet<Ticket> Ticket { get; set; }

    }
}
