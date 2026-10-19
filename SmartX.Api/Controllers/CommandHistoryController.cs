using Microsoft.AspNetCore.Mvc;
using SmartX.Api.Services;
using SmartX.Shared.Models;

namespace SmartX.Api.Controllers
{
    [ApiController]
    [Route("api/commands/history")]
    public class CommandHistoryController : ControllerBase
    {
        private readonly CommandHistoryStore _historyStore;

        public CommandHistoryController(
            CommandHistoryStore historyStore)
        {
            _historyStore = historyStore;
        }

        // GET: api/commands/history
        [HttpGet]
        public ActionResult<List<SmartCommand>> GetCommandHistory()
        {
            return Ok(_historyStore.GetAllCommands());
        }

        // GET: api/commands/history/device/TEMP-001
        [HttpGet("device/{deviceId}")]
        public ActionResult<List<SmartCommand>> GetCommandsForDevice(
            string deviceId)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                return BadRequest("Device ID is required.");
            }

            return Ok(_historyStore.GetCommandsForDevice(deviceId));
        }

        // POST: api/commands/history
        [HttpPost]
        public IActionResult RecordProcessedCommand(
            [FromBody] SmartCommand command)
        {
            if (command == null)
            {
                return BadRequest("A command is required.");
            }

            if (command.Status != "Processed")
            {
                return BadRequest(
                    "Only processed commands can be recorded.");
            }

            if (command.ProcessedAt == null)
            {
                return BadRequest(
                    "The command processing timestamp is required.");
            }

            bool recorded = _historyStore.RecordCommand(command);

            if (!recorded)
            {
                return Conflict(
                    $"Command ID {command.Id} has already been recorded.");
            }

            return Created(
                $"/api/commands/history",
                command);
        }
    }
}