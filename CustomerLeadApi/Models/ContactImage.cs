using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerLeadApi.Models
{
    public class ContactImage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ContactId { get; set; }

        [Required]
        public string ImageData { get; set; } = string.Empty; // Base64 encoded string

        [StringLength(100)]
        public string? FileName { get; set; }

        [StringLength(50)]
        public string? ContentType { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        [ForeignKey("ContactId")]
        public Contact? Contact { get; set; }
    }
}
