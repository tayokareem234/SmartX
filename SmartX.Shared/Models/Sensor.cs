namespace SmartX.Shared.Models
{
    public class Sensor
    {
        public int Id { get; set; }

        public string MacAddress { get; set; } = string.Empty;

        public string UniqueIdentifier { get; set; } = string.Empty;

        public string DeploymentLocation { get; set; } = string.Empty;

        public string Zone { get; set; } = string.Empty;

        public string SensorCategory { get; set; } = string.Empty;

        public bool IsConnected { get; set; }

        public DateTime LastCommunication { get; set; }

        public string Status { get; set; } = "Normal";
    }
}