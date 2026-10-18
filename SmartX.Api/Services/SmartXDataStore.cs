using SmartX.Shared.Models;

namespace SmartX.Api.Services
{
    public class SmartXDataStore
    {
        // Existing collections used by Part 1.
        public List<Sensor> Sensors { get; } = new();

        public List<TelemetryRecord> TelemetryRecords { get; } = new();

        public List<SensorAttachment> Attachments { get; } = new();

        // Part 2: Dictionary for efficient sensor lookup.
        public Dictionary<string, Sensor> SensorsByIdentifier { get; }
            = new(StringComparer.OrdinalIgnoreCase);

        public SmartXDataStore()
        {
            SeedSensors();
        }

        private void SeedSensors()
        {
            AddSensor(new Sensor
            {
                Id = 1,
                MacAddress = "AA:BB:CC:11:22:33",
                UniqueIdentifier = "TEMP-001",
                DeploymentLocation = "Room 101",
                Zone = "Zone A",
                SensorCategory = "Environmental",
                IsConnected = true,
                LastCommunication = DateTime.UtcNow,
                Status = "Normal"
            });

            AddSensor(new Sensor
            {
                Id = 2,
                MacAddress = "AA:BB:CC:44:55:66",
                UniqueIdentifier = "POWER-001",
                DeploymentLocation = "Server Room",
                Zone = "Zone A",
                SensorCategory = "Power Consumption",
                IsConnected = true,
                LastCommunication = DateTime.UtcNow,
                Status = "Normal"
            });

            AddSensor(new Sensor
            {
                Id = 3,
                MacAddress = "AA:BB:CC:77:88:99",
                UniqueIdentifier = "ACT-001",
                DeploymentLocation = "Room 102",
                Zone = "Zone B",
                SensorCategory = "Actuator",
                IsConnected = false,
                LastCommunication = DateTime.UtcNow.AddMinutes(-10),
                Status = "Disconnected"
            });
        }

        // Adds a sensor to both the existing list and the dictionary.
        public void AddSensor(Sensor sensor)
        {
            if (sensor == null)
            {
                throw new ArgumentNullException(nameof(sensor));
            }

            if (string.IsNullOrWhiteSpace(sensor.UniqueIdentifier))
            {
                throw new ArgumentException(
                    "A sensor identifier is required.",
                    nameof(sensor));
            }

            if (SensorsByIdentifier.ContainsKey(sensor.UniqueIdentifier))
            {
                throw new InvalidOperationException(
                    $"Sensor '{sensor.UniqueIdentifier}' already exists.");
            }

            Sensors.Add(sensor);
            SensorsByIdentifier.Add(sensor.UniqueIdentifier, sensor);
        }

        // Finds a sensor by its unique identifier.
        public Sensor? GetSensorByIdentifier(string deviceId)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                return null;
            }

            return SensorsByIdentifier.TryGetValue(
                deviceId,
                out Sensor? sensor)
                    ? sensor
                    : null;
        }

        public bool UpdateSensorIdentifier(
    Sensor sensor,
    string newIdentifier)
        {
            if (sensor == null ||
                string.IsNullOrWhiteSpace(newIdentifier))
            {
                return false;
            }

            string oldIdentifier = sensor.UniqueIdentifier;

            if (oldIdentifier.Equals(
                newIdentifier,
                StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (SensorsByIdentifier.ContainsKey(newIdentifier))
            {
                return false;
            }

            if (!SensorsByIdentifier.Remove(oldIdentifier))
            {
                return false;
            }

            sensor.UniqueIdentifier = newIdentifier;
            SensorsByIdentifier.Add(newIdentifier, sensor);

            return true;
        }

        public int GetNextAttachmentId()
        {
            return Attachments.Count == 0
                ? 1
                : Attachments.Max(a => a.Id) + 1;
        }

        public void AddAttachment(SensorAttachment attachment)
        {
            Attachments.Add(attachment);
        }

        public bool SetSensorDisconnected(string deviceId)
        {
            Sensor? sensor = GetSensorByIdentifier(deviceId);

            if (sensor == null)
            {
                return false;
            }

            sensor.IsConnected = false;
            sensor.Status = "Disconnected";

            return true;
        }
    }
}