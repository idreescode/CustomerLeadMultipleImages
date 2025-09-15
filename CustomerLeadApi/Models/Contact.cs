using System.ComponentModel.DataAnnotations;

namespace CustomerLeadApi.Models
{
    public enum ContactType
    {
        Customer,
        Lead
    }

    public class Contact
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        [Required]
        public ContactType Type { get; set; }

        // Navigation property for images
        public ICollection<ContactImage> Images { get; set; } = new List<ContactImage>();
    }
}
