using CustomerLeadApi.Models;

namespace CustomerLeadApi.DTOs
{
    public class ContactDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public ContactType Type { get; set; }
        public int ImageCount { get; set; }
    }

    public class CreateContactDTO
    {
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public ContactType Type { get; set; }
    }

    public class UpdateContactDTO
    {
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }
}
