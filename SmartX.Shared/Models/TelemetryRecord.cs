namespace SmartX.Shared.Models
{
    public class TelemetryRecord
    {
        public int Id { get; set; }

        public string DeviceId { get; set; } = string.Empty;

        public string SensorCategory { get; set; } = string.Empty;

        public double NumericValue { get; set; }

        public double Delta { get; set; }

        public string Unit { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; }

        public bool IsAnomaly { get; set; }

        public bool IsDisconnected { get; set; }

        public string Status { get; set; } = "Normal";
    }
}