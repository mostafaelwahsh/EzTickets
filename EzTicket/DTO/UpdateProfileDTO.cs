using System.ComponentModel.DataAnnotations;

namespace EzTickets.DTO
{
    public class UpdateProfileDTO
    {

        [StringLength(100)]
        public string FullName { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        [StringLength(3)]
        public string BloodType { get; set; }

        [StringLength(50)]
        public string City { get; set; }
    }
}
