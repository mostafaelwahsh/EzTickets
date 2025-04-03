using System.ComponentModel.DataAnnotations;

namespace EzTickets.DTO.Public
{
    public class LoginUserDTO
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
       
}
