namespace SmartX.Shared.Models
{
    public class SensorAttachment
    {
        public int Id { get; set; }

        public string DeviceId { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;

        public string ContentType { get; set; } = string.Empty;

        public long FileSize { get; set; }

        public string AttachmentType { get; set; } = string.Empty;

        public string StoredFileName { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; }
    }
}