using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class EventCategory
    {
        [Key]
        public string EventId { get; set; }
        public Event Event { get; set; }
        public string CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
