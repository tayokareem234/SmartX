using SmartX.Shared.Models;

namespace SmartX.Api.Services
{
    public class SmartXDataStore
    {
        public List<Sensor> Sensors { get; } = new();

        public List<TelemetryRecord> TelemetryRecords { get; } = new();

        public List<SensorAttachment> Attachments { get; } = new();

        public SmartXDataStore()
        {
            SeedSensors();
        }

        private void SeedSensors()
        {
            Sensors.Add(new Sensor
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

            Sensors.Add(new Sensor
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

            Sensors.Add(new Sensor
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
            Sensor? sensor = Sensors.FirstOrDefault(s =>
                s.UniqueIdentifier.Equals(
                    deviceId,
                    StringComparison.OrdinalIgnoreCase));

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