using System.ComponentModel.DataAnnotations;

namespace EzTickets.DTO.Public
{
    public class RegisterUserDTO
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required,Phone]
        public string PhoneNumber { get; set; }

    }
}
