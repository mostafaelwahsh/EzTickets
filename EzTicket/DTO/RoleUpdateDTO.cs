using System.ComponentModel.DataAnnotations;

namespace EzTickets.DTO
{
    public class RoleUpdateDTO
    {
        [Required]
        public string RoleName { get; set; }
    }
}
