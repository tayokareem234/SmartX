using SmartX.Shared.Models;

namespace SmartX.Api.Services
{
    public class TelemetryProcessingService
    {
        private readonly SmartXDataStore _dataStore;

        // Multi-dimensional array for sequential raw telemetry batches.
        // Rows represent telemetry entries.
        // Columns represent:
        // 0 = telemetry value
        // 1 = anomaly flag
        // 2 = timestamp value
        private readonly double[,] _rawTelemetryBatch =
            new double[50, 3];

        private int _batchPosition = 0;

        // Optimized collection used after raw telemetry
        // has been collected into the temporary batch array.
        private readonly List<TelemetryRecord> _processedTelemetry =
            new List<TelemetryRecord>();

        // Jagged array for variable-length sensor histories.
        // Each row represents the numeric history of one sensor.
        private readonly double[][] _sensorHistory =
            new double[20][];

        public TelemetryProcessingService(
            SmartXDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public TelemetryRecord ProcessNumericTelemetry(
            string deviceId,
            double value,
            string category,
            string unit,
            double minimum,
            double maximum)
        {
            Sensor? sensor = FindSensor(deviceId);

            if (sensor == null)
            {
                throw new KeyNotFoundException(
                    $"Sensor '{deviceId}' is not registered.");
            }

            if (string.IsNullOrWhiteSpace(unit))
            {
                throw new ArgumentException(
                    "Telemetry unit is required.");
            }

            bool isAnomaly =
                value < minimum ||
                value > maximum;

            double delta = CalculateDelta(
                deviceId,
                value,
                unit);

            StoreRawTelemetry(
                value,
                isAnomaly);

            UpdateSensorStatus(
                sensor,
                isAnomaly);

            TelemetryRecord record = new TelemetryRecord
            {
                Id = GetNextTelemetryId(),
                DeviceId = deviceId,
                SensorCategory = category,
                NumericValue = value,
                Delta = delta,
                Unit = unit,
                Timestamp = DateTime.UtcNow,
                IsAnomaly = isAnomaly,
                IsDisconnected = false,
                Status = isAnomaly
                    ? "Anomaly"
                    : "Normal"
            };

            _dataStore.TelemetryRecords.Add(record);

            UpdateJaggedSensorHistory(
                sensor.Id,
                value);

            return record;
        }

        public TelemetryRecord ProcessSwitchTelemetry(
            TelemetryPacket<bool> packet)
        {
            Sensor? sensor = FindSensor(packet.DeviceId);

            if (sensor == null)
            {
                throw new KeyNotFoundException(
                    $"Sensor '{packet.DeviceId}' is not registered.");
            }

            sensor.IsConnected = true;
            sensor.LastCommunication = DateTime.UtcNow;
            sensor.Status = "Normal";

            TelemetryRecord record = new TelemetryRecord
            {
                Id = GetNextTelemetryId(),
                DeviceId = packet.DeviceId,
                SensorCategory = "Actuator",
                NumericValue = packet.Value ? 1 : 0,
                Delta = 0,
                Unit = "Boolean",
                Timestamp = packet.Timestamp == default
                    ? DateTime.UtcNow
                    : packet.Timestamp,
                IsAnomaly = false,
                IsDisconnected = false,
                Status = packet.Value
                    ? "ON"
                    : "OFF"
            };

            _dataStore.TelemetryRecords.Add(record);

            return record;
        }

        private double CalculateDelta(
            string deviceId,
            double currentValue,
            string unit)
        {
            TelemetryRecord? previous =
                _dataStore.TelemetryRecords
                    .Where(t =>
                        t.DeviceId.Equals(
                            deviceId,
                            StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(t => t.Timestamp)
                    .FirstOrDefault();

            if (previous == null)
            {
                return 0;
            }

            SensorValue current = new SensorValue(
                currentValue,
                unit);

            SensorValue previousValue = new SensorValue(
                previous.NumericValue,
                previous.Unit);

            // Operator overloading is used here to
            // calculate the telemetry delta directly.
            return current - previousValue;
        }

        private void StoreRawTelemetry(
            double value,
            bool isAnomaly)
        {
            if (_batchPosition >=
                _rawTelemetryBatch.GetLength(0))
            {
                TransferBatchToList();
            }

            _rawTelemetryBatch[_batchPosition, 0] =
                value;

            _rawTelemetryBatch[_batchPosition, 1] =
                isAnomaly ? 1 : 0;

            _rawTelemetryBatch[_batchPosition, 2] =
                DateTimeOffset.UtcNow
                    .ToUnixTimeMilliseconds();

            _batchPosition++;
        }

        private void TransferBatchToList()
        {
            // Transfer the sequential raw telemetry data
            // from the multi-dimensional array into the
            // optimized List<T> collection.

            for (int row = 0;
                 row < _batchPosition;
                 row++)
            {
                double value =
                    _rawTelemetryBatch[row, 0];

                bool isAnomaly =
                    _rawTelemetryBatch[row, 1] == 1;

                long timestampMilliseconds =
                    Convert.ToInt64(
                        _rawTelemetryBatch[row, 2]);

                TelemetryRecord batchRecord =
                    new TelemetryRecord
                    {
                        Id = _processedTelemetry.Count + 1,
                        DeviceId = "BATCH",
                        SensorCategory = "Raw Telemetry",
                        NumericValue = value,
                        Delta = 0,
                        Unit = "Raw",
                        Timestamp =
                            DateTimeOffset
                                .FromUnixTimeMilliseconds(
                                    timestampMilliseconds)
                                .UtcDateTime,
                        IsAnomaly = isAnomaly,
                        IsDisconnected = false,
                        Status = isAnomaly
                            ? "Anomaly"
                            : "Normal"
                    };

                _processedTelemetry.Add(
                    batchRecord);
            }

            // The temporary array batch is cleared logically
            // by resetting its current position.
            _batchPosition = 0;
        }

        private void UpdateJaggedSensorHistory(
            int sensorId,
            double value)
        {
            int index = sensorId - 1;

            if (index < 0 ||
                index >= _sensorHistory.Length)
            {
                return;
            }

            double[]? existingHistory =
                _sensorHistory[index];

            if (existingHistory == null)
            {
                _sensorHistory[index] =
                    new[] { value };

                return;
            }

            double[] updatedHistory =
                new double[existingHistory.Length + 1];

            for (int i = 0;
                 i < existingHistory.Length;
                 i++)
            {
                updatedHistory[i] =
                    existingHistory[i];
            }

            updatedHistory[^1] = value;

            _sensorHistory[index] =
                updatedHistory;
        }

        private void UpdateSensorStatus(
            Sensor sensor,
            bool isAnomaly)
        {
            sensor.IsConnected = true;

            sensor.LastCommunication =
                DateTime.UtcNow;

            sensor.Status =
                isAnomaly
                    ? "Anomaly"
                    : "Normal";
        }

        private Sensor? FindSensor(
            string deviceId)
        {
            return _dataStore.Sensors
                .FirstOrDefault(s =>
                    s.UniqueIdentifier.Equals(
                        deviceId,
                        StringComparison.OrdinalIgnoreCase));
        }

        private int GetNextTelemetryId()
        {
            return _dataStore.TelemetryRecords.Count == 0
                ? 1
                : _dataStore.TelemetryRecords.Max(
                    t => t.Id) + 1;
        }
    }
}