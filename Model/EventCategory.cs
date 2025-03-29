using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    [PrimaryKey(nameof(EventId), nameof(CategoryId))] // Composite primary key

    public class EventCategory
    {
        public string? EventId { get; set; } // Use only one ID property
        

        public string? CategoryId { get; set; }

        // Navigation properties
        public Event Event { get; set; }
        public Category Category { get; set; }
    }
}
