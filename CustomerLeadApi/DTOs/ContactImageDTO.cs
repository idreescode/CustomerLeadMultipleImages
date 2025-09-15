namespace CustomerLeadApi.DTOs
{
    public class ContactImageDTO
    {
        public int Id { get; set; }
        public int ContactId { get; set; }
        public string ImageData { get; set; } = string.Empty;
        public string? FileName { get; set; }
        public string? ContentType { get; set; }
        public DateTime UploadedAt { get; set; }
    }

    public class UploadImageDTO
    {
        public string ImageData { get; set; } = string.Empty; // Base64 encoded string
        public string? FileName { get; set; }
        public string? ContentType { get; set; }
    }
}
