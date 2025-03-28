using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Models;

namespace Data
{
    public class DataContext : IdentityDbContext<ApplicationUser>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        // DbSet properties for your entities
        public DbSet<Category> Courses { get; set; }
        public DbSet<Event> Plugins { get; set; }
        public DbSet<EventCategory> CourseContents { get; set; }
        public DbSet<Order> Enrollments { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Ticket> ShoppingCarts { get; set; }
    }
    
}