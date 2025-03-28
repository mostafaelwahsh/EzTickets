using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Category
    {
        [Key]
        public string CategoryId { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } // e.g., "Concert", "Sports"
        public ICollection<EventCategory> EventCategories { get; set; } = new List<EventCategory>();
    }
}
