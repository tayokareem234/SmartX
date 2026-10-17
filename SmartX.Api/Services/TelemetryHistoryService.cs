using SmartX.Shared.Models;

namespace SmartX.Api.Services
{
    public class TelemetryHistoryService
    {
        private readonly SortedDictionary<
            DateTime,
            List<TelemetryRecord>> _history = new();

        private readonly object _lock = new();

        public void AddRecord(TelemetryRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            lock (_lock)
            {
                if (!_history.TryGetValue(
                    record.Timestamp,
                    out List<TelemetryRecord>? records))
                {
                    records = new List<TelemetryRecord>();

                    _history.Add(record.Timestamp, records);
                }

                records.Add(record);
            }
        }

        public List<TelemetryRecord> GetAllRecords()
        {
            lock (_lock)
            {
                return _history.Values
                    .SelectMany(records => records)
                    .ToList();
            }
        }

        public List<TelemetryRecord> GetRecordsForDevice(
            string deviceId)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                throw new ArgumentException(
                    "Device ID is required.",
                    nameof(deviceId));
            }

            lock (_lock)
            {
                return _history.Values
                    .SelectMany(records => records)
                    .Where(record =>
                        record.DeviceId.Equals(
                            deviceId,
                            StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
        }

        public int Count
        {
            get
            {
                lock (_lock)
                {
                    return _history.Values.Sum(
                        records => records.Count);
                }
            }
        }
    }
}