namespace SmartX.Shared.Models
{
    public class SmartCommand
    {
        public int Id { get; set; }

        public string DeviceId { get; set; } = string.Empty;

        public string CommandType { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Priority { get; set; } = "Normal";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string Status { get; set; } = "Pending";

        public bool IsUndoable { get; set; }

        // Records when the command finishes processing.
        public DateTime? ProcessedAt { get; set; }

        // Records whether the command completed successfully.
        public bool? WasSuccessful { get; set; }
    }
}