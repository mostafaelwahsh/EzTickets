using System.Runtime.Serialization;

namespace EzTickets.DTO
{
    public class UserProfileDTO
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string BloodType { get; set; }
        public string City { get; set; }
    }
}
