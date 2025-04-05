using System.ComponentModel.DataAnnotations;

namespace EzTickets.DTO.Public
{
    public class ForgotPasswordDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
