using System.ComponentModel.DataAnnotations;

namespace EzTickets.DTO.Admin
{
    public class RoleUpdateDTO
    {
        [Required]
        public string RoleName { get; set; }
    }
}
