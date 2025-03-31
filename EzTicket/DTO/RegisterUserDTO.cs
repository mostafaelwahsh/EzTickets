using System.ComponentModel.DataAnnotations;

namespace EzTickets.DTO
{
    public class RegisterUserDTO
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public String FullName { get; set; }

        [Required,Phone]
        public string PhoneNumber { get; set; }

    }
}
