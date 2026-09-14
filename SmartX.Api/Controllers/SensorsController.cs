using Microsoft.AspNetCore.Mvc;
using SmartX.Api.Services;
using SmartX.Shared.Models;

namespace SmartX.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SensorsController : ControllerBase
    {
        private readonly SmartXDataStore _dataStore;

        public SensorsController(SmartXDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        // GET: api/sensors
        [HttpGet]
        public ActionResult<IEnumerable<Sensor>> GetSensors()
        {
            return Ok(_dataStore.Sensors);
        }

        // GET: api/sensors/1
        [HttpGet("{id}")]
        public ActionResult<Sensor> GetSensor(int id)
        {
            Sensor? sensor = _dataStore.Sensors
                .FirstOrDefault(s => s.Id == id);

            if (sensor == null)
            {
                return NotFound(new
                {
                    message = $"Sensor with ID {id} was not found."
                });
            }

            return Ok(sensor);
        }

        // POST: api/sensors
        [HttpPost]
        public ActionResult<Sensor> RegisterSensor([FromBody] Sensor sensor)
        {
            if (sensor == null)
            {
                return BadRequest(new
                {
                    message = "Sensor data is required."
                });
            }

            if (string.IsNullOrWhiteSpace(sensor.MacAddress))
            {
                return BadRequest(new
                {
                    message = "MAC address is required."
                });
            }

            if (string.IsNullOrWhiteSpace(sensor.UniqueIdentifier))
            {
                return BadRequest(new
                {
                    message = "Unique identifier is required."
                });
            }

            if (string.IsNullOrWhiteSpace(sensor.DeploymentLocation))
            {
                return BadRequest(new
                {
                    message = "Deployment location is required."
                });
            }

            if (string.IsNullOrWhiteSpace(sensor.SensorCategory))
            {
                return BadRequest(new
                {
                    message = "Sensor category is required."
                });
            }

            bool duplicateIdentifier = _dataStore.Sensors
                .Any(s => s.UniqueIdentifier.Equals(
                    sensor.UniqueIdentifier,
                    StringComparison.OrdinalIgnoreCase));

            if (duplicateIdentifier)
            {
                return Conflict(new
                {
                    message = "A sensor with this unique identifier already exists."
                });
            }

            sensor.Id = _dataStore.Sensors.Count == 0
                ? 1
                : _dataStore.Sensors.Max(s => s.Id) + 1;

            sensor.IsConnected = true;
            sensor.LastCommunication = DateTime.UtcNow;
            sensor.Status = "Normal";

            _dataStore.Sensors.Add(sensor);

            return CreatedAtAction(
                nameof(GetSensor),
                new { id = sensor.Id },
                sensor);
        }

        // PUT: api/sensors/1
        [HttpPut("{id}")]
        public IActionResult UpdateSensor(
     int id,
     [FromBody] Sensor updatedSensor)
        {
            Sensor? existingSensor = _dataStore.Sensors
                .FirstOrDefault(s => s.Id == id);

            if (existingSensor == null)
            {
                return NotFound(new
                {
                    message = $"Sensor with ID {id} was not found."
                });
            }

            if (updatedSensor == null)
            {
                return BadRequest(new
                {
                    message = "Sensor data is required."
                });
            }

            if (string.IsNullOrWhiteSpace(updatedSensor.MacAddress))
            {
                return BadRequest(new
                {
                    message = "MAC address is required."
                });
            }

            if (string.IsNullOrWhiteSpace(updatedSensor.UniqueIdentifier))
            {
                return BadRequest(new
                {
                    message = "Unique identifier is required."
                });
            }

            if (string.IsNullOrWhiteSpace(updatedSensor.DeploymentLocation))
            {
                return BadRequest(new
                {
                    message = "Deployment location is required."
                });
            }

            if (string.IsNullOrWhiteSpace(updatedSensor.SensorCategory))
            {
                return BadRequest(new
                {
                    message = "Sensor category is required."
                });
            }

            bool duplicateIdentifier = _dataStore.Sensors
                .Any(s =>
                    s.Id != id &&
                    s.UniqueIdentifier.Equals(
                        updatedSensor.UniqueIdentifier,
                        StringComparison.OrdinalIgnoreCase));

            if (duplicateIdentifier)
            {
                return Conflict(new
                {
                    message =
                        "A sensor with this unique identifier already exists."
                });
            }

            existingSensor.MacAddress = updatedSensor.MacAddress;
            existingSensor.UniqueIdentifier =
                updatedSensor.UniqueIdentifier;
            existingSensor.DeploymentLocation =
                updatedSensor.DeploymentLocation;
            existingSensor.Zone = updatedSensor.Zone;
            existingSensor.SensorCategory =
                updatedSensor.SensorCategory;
            existingSensor.IsConnected =
                updatedSensor.IsConnected;
            existingSensor.LastCommunication =
                updatedSensor.LastCommunication;
            existingSensor.Status =
                updatedSensor.Status;

            return NoContent();
        }

        // POST: api/sensors/TEMP-001/disconnect
        [HttpPost("{deviceId}/disconnect")]
        public IActionResult SimulateDisconnect(string deviceId)
        {
            bool disconnected =
                _dataStore.SetSensorDisconnected(deviceId);

            if (!disconnected)
            {
                return NotFound(new
                {
                    message = $"Sensor '{deviceId}' was not found."
                });
            }

            Sensor sensor = _dataStore.Sensors
                .First(s => s.UniqueIdentifier.Equals(
                    deviceId,
                    StringComparison.OrdinalIgnoreCase));

            return Ok(new
            {
                message = $"Sensor '{sensor.UniqueIdentifier}' is now disconnected.",
                deviceId = sensor.UniqueIdentifier,
                status = sensor.Status,
                isConnected = sensor.IsConnected
            });
        }
    }
}