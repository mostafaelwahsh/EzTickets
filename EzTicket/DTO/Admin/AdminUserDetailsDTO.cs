namespace EzTickets.DTO.Admin
{
    public class AdminUserDetailsDTO
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string BloodType { get; set; }
        public string City { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}
