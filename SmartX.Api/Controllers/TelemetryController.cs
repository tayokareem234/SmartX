using Microsoft.AspNetCore.Mvc;
using SmartX.Api.Services;
using SmartX.Shared.Models;

namespace SmartX.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TelemetryController : ControllerBase
    {
        private readonly SmartXDataStore _dataStore;

        private readonly TelemetryProcessingService
            _telemetryService;

        private readonly DeploymentValidationService
    _deploymentValidator;

        public TelemetryController(
     SmartXDataStore dataStore,
     TelemetryProcessingService telemetryService,
     DeploymentValidationService deploymentValidator)
        {
            _dataStore = dataStore;
            _telemetryService = telemetryService;
            _deploymentValidator = deploymentValidator;
        }


        // GET: api/telemetry
        [HttpGet]
        public ActionResult<IEnumerable<TelemetryRecord>>
            GetTelemetry()
        {
            return Ok(_dataStore.TelemetryRecords);
        }

        // GET: api/telemetry/{deviceId}
        [HttpGet("{deviceId}")]
        public ActionResult<IEnumerable<TelemetryRecord>>
            GetDeviceTelemetry(string deviceId)
        {
            var records =
                _dataStore.TelemetryRecords
                    .Where(t =>
                        t.DeviceId.Equals(
                            deviceId,
                            StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(
                        t => t.Timestamp)
                    .ToList();

            if (records.Count == 0)
            {
                return NotFound(new
                {
                    message =
                        $"No telemetry found for device '{deviceId}'."
                });
            }

            return Ok(records);
        }

        // POST: api/telemetry/temperature
        [HttpPost("temperature")]
        public ActionResult<TelemetryRecord>
            ReceiveTemperature(
                [FromBody]
                TelemetryPacket<float> packet)
        {
            if (packet == null)
            {
                return BadRequest(new
                {
                    message =
                        "Telemetry packet is required."
                });
            }

            try
            {
                TelemetryRecord record =
                    _telemetryService
                        .ProcessNumericTelemetry(
                            packet.DeviceId,
                            packet.Value,
                            "Temperature",
                            packet.Unit,
                            0,
                            50);

                return Ok(record);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // POST: api/telemetry/power
        [HttpPost("power")]
        public ActionResult<TelemetryRecord>
            ReceivePower(
                [FromBody]
                TelemetryPacket<int> packet)
        {
            if (packet == null)
            {
                return BadRequest(new
                {
                    message =
                        "Telemetry packet is required."
                });
            }

            try
            {
                TelemetryRecord record =
                    _telemetryService
                        .ProcessNumericTelemetry(
                            packet.DeviceId,
                            packet.Value,
                            "Power Consumption",
                            packet.Unit,
                            0,
                            5000);

                return Ok(record);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // POST: api/telemetry/switch
        [HttpPost("switch")]
        public ActionResult<TelemetryRecord>
            ReceiveSwitch(
                [FromBody]
                TelemetryPacket<bool> packet)
        {
            if (packet == null)
            {
                return BadRequest(new
                {
                    message =
                        "Telemetry packet is required."
                });
            }

            try
            {
                TelemetryRecord record =
                    _telemetryService
                        .ProcessSwitchTelemetry(packet);

                return Ok(record);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }

        // POST: api/telemetry/validate-deployment
        [HttpPost("validate-deployment")]
        public ActionResult ValidateDeployment(
            [FromBody] DeploymentNode root)
        {
            if (root == null)
            {
                return BadRequest(new
                {
                    message = "Deployment structure is required."
                });
            }

            List<string> errors =
                _deploymentValidator
                    .ValidateDeployment(root);

            if (errors.Count > 0)
            {
                return BadRequest(new
                {
                    valid = false,
                    errors
                });
            }

            return Ok(new
            {
                valid = true,
                message =
                    "Deployment structure is valid."
            });
        }
    }
}