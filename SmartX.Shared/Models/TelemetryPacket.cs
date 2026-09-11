namespace SmartX.Shared.Models
{
    public class TelemetryPacket<T>
    {
        public string DeviceId { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; }

        public string DataType { get; set; } = string.Empty;

        public T Value { get; set; } = default!;

        public string Unit { get; set; } = string.Empty;

        public TelemetryPacket()
        {
        }

        public TelemetryPacket(
            string deviceId,
            T value,
            string dataType,
            string unit)
        {
            DeviceId = deviceId;
            Value = value;
            DataType = dataType;
            Unit = unit;
            Timestamp = DateTime.UtcNow;
        }
    }
}