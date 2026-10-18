using System;
using System.Collections.Generic;
using System.Linq;
using SmartX.Shared.Models;

namespace SmartX.Api.Services
{
    public class TelemetryProcessingService
    {
        private readonly SmartXDataStore _dataStore;
        private readonly TelemetryHistoryService _historyService;

        // Protects access to the tracking collections.
        private readonly object _trackingLock = new();

        // Tracks devices currently reporting anomalies.
        // HashSet prevents duplicate device identifiers.
        private readonly HashSet<string> _activeAnomalies =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // Tracks devices currently disconnected.
        private readonly HashSet<string> _disconnectedDevices =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // Multidimensional array for sequential raw telemetry batches.
        // Columns: value, anomaly flag, timestamp in milliseconds.
        private readonly double[,] _rawTelemetryBatch = new double[50, 3];

        private int _batchPosition = 0;

        // List used to store processed raw telemetry batches.
        private readonly List<TelemetryRecord> _processedTelemetry =
            new List<TelemetryRecord>();

        // Jagged array for variable-length sensor histories.
        private readonly double[][] _sensorHistory = new double[20][];

        public TelemetryProcessingService(
            SmartXDataStore dataStore,
            TelemetryHistoryService historyService)
        {
            _dataStore = dataStore;
            _historyService = historyService;
        }

        // Processes numeric telemetry such as temperature and power.
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

            bool isAnomaly = value < minimum || value > maximum;

            double delta = CalculateDelta(deviceId, value, unit);

            StoreRawTelemetry(value, isAnomaly);

            // Update the sensor's connection and status.
            UpdateSensorStatus(sensor, isAnomaly);

            // Update active anomaly tracking.
            UpdateAnomalyTracking(deviceId, isAnomaly);

            // Receiving valid telemetry confirms the device is connected.
            UpdateDisconnectedTracking(deviceId, false);

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
                Status = isAnomaly ? "Anomaly" : "Normal"
            };

            _dataStore.TelemetryRecords.Add(record);

            // Preserve the SortedDictionary history implementation.
            _historyService.AddRecord(record);

            UpdateJaggedSensorHistory(sensor.Id, value);

            return record;
        }

        // Processes Boolean telemetry from an actuator or switch.
        public TelemetryRecord ProcessSwitchTelemetry(
            TelemetryPacket<bool> packet)
        {
            if (packet == null)
            {
                throw new ArgumentNullException(nameof(packet));
            }

            Sensor? sensor = FindSensor(packet.DeviceId);

            if (sensor == null)
            {
                throw new KeyNotFoundException(
                    $"Sensor '{packet.DeviceId}' is not registered.");
            }

            // A received packet confirms the device is connected.
            sensor.IsConnected = true;
            sensor.LastCommunication = DateTime.UtcNow;
            sensor.Status = "Normal";

            UpdateAnomalyTracking(packet.DeviceId, false);
            UpdateDisconnectedTracking(packet.DeviceId, false);

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
                Status = packet.Value ? "ON" : "OFF"
            };

            _dataStore.TelemetryRecords.Add(record);
            _historyService.AddRecord(record);

            return record;
        }

        // Calculate the difference between the current and previous readings.
        private double CalculateDelta(
            string deviceId,
            double currentValue,
            string unit)
        {
            TelemetryRecord? previous = _dataStore.TelemetryRecords
                .Where(t => t.DeviceId.Equals(
                    deviceId,
                    StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(t => t.Timestamp)
                .FirstOrDefault();

            if (previous == null)
            {
                return 0;
            }

            SensorValue current = new SensorValue(currentValue, unit);
            SensorValue previousValue = new SensorValue(
                previous.NumericValue,
                previous.Unit);

            return current - previousValue;
        }

        // Store raw telemetry in the multidimensional array.
        private void StoreRawTelemetry(double value, bool isAnomaly)
        {
            if (_batchPosition >= _rawTelemetryBatch.GetLength(0))
            {
                TransferBatchToList();
            }

            _rawTelemetryBatch[_batchPosition, 0] = value;
            _rawTelemetryBatch[_batchPosition, 1] = isAnomaly ? 1 : 0;
            _rawTelemetryBatch[_batchPosition, 2] =
                DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            _batchPosition++;
        }

        // Transfer the current raw batch to the processed telemetry list.
        private void TransferBatchToList()
        {
            for (int row = 0; row < _batchPosition; row++)
            {
                double value = _rawTelemetryBatch[row, 0];

                bool isAnomaly = _rawTelemetryBatch[row, 1] == 1;

                long timestampMilliseconds = Convert.ToInt64(
                    _rawTelemetryBatch[row, 2]);

                TelemetryRecord batchRecord = new TelemetryRecord
                {
                    Id = _processedTelemetry.Count + 1,
                    DeviceId = "BATCH",
                    SensorCategory = "Raw Telemetry",
                    NumericValue = value,
                    Delta = 0,
                    Unit = "Raw",
                    Timestamp = DateTimeOffset
                        .FromUnixTimeMilliseconds(timestampMilliseconds)
                        .UtcDateTime,
                    IsAnomaly = isAnomaly,
                    IsDisconnected = false,
                    Status = isAnomaly ? "Anomaly" : "Normal"
                };

                _processedTelemetry.Add(batchRecord);
            }

            _batchPosition = 0;
        }

        // Update the numeric history for a sensor.
        private void UpdateJaggedSensorHistory(int sensorId, double value)
        {
            int index = sensorId - 1;

            if (index < 0 || index >= _sensorHistory.Length)
            {
                return;
            }

            double[]? existingHistory = _sensorHistory[index];

            if (existingHistory == null)
            {
                _sensorHistory[index] = new[] { value };
                return;
            }

            double[] updatedHistory =
                new double[existingHistory.Length + 1];

            for (int i = 0; i < existingHistory.Length; i++)
            {
                updatedHistory[i] = existingHistory[i];
            }

            updatedHistory[^1] = value;
            _sensorHistory[index] = updatedHistory;
        }

        // Update the sensor's status after receiving numeric telemetry.
        private void UpdateSensorStatus(Sensor sensor, bool isAnomaly)
        {
            sensor.IsConnected = true;
            sensor.LastCommunication = DateTime.UtcNow;
            sensor.Status = isAnomaly ? "Anomaly" : "Normal";
        }

        // Look up a sensor by its unique identifier.
        private Sensor? FindSensor(string deviceId)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                return null;
            }

            return _dataStore.GetSensorByIdentifier(deviceId);
        }

        // Generate the next telemetry record ID.
        private int GetNextTelemetryId()
        {
            return _dataStore.TelemetryRecords.Count == 0
                ? 1
                : _dataStore.TelemetryRecords.Max(t => t.Id) + 1;
        }

        // Add or remove a device from active anomaly tracking.
        public void UpdateAnomalyTracking(
            string deviceId,
            bool isAnomaly)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                throw new ArgumentException(
                    "Device identifier is required.",
                    nameof(deviceId));
            }

            lock (_trackingLock)
            {
                if (isAnomaly)
                {
                    _activeAnomalies.Add(deviceId);
                }
                else
                {
                    _activeAnomalies.Remove(deviceId);
                }
            }
        }

        // Add or remove a device from disconnected-device tracking.
        public void UpdateDisconnectedTracking(
            string deviceId,
            bool isDisconnected)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                throw new ArgumentException(
                    "Device identifier is required.",
                    nameof(deviceId));
            }

            lock (_trackingLock)
            {
                if (isDisconnected)
                {
                    _disconnectedDevices.Add(deviceId);
                }
                else
                {
                    _disconnectedDevices.Remove(deviceId);
                }
            }
        }

        // Return a snapshot of devices with active anomalies.
        public List<string> GetActiveAnomalies()
        {
            lock (_trackingLock)
            {
                return _activeAnomalies
                    .OrderBy(deviceId => deviceId)
                    .ToList();
            }
        }

        // Return a snapshot of disconnected devices.
        public List<string> GetDisconnectedDevices()
        {
            lock (_trackingLock)
            {
                return _disconnectedDevices
                    .OrderBy(deviceId => deviceId)
                    .ToList();
            }
        }
    }
}