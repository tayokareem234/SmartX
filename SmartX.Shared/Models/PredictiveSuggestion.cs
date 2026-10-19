namespace SmartX.Shared.Models
{
    public class PredictiveSuggestion
    {
        public int Id { get; set; }

        public string DeviceId { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string SuggestedAction { get; set; } = string.Empty;

        public string Severity { get; set; } = "Information";

        public string Source { get; set; } = string.Empty;

        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

        public bool IsActionable { get; set; }
    }
}