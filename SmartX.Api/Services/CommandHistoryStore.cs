using SmartX.Shared.Models;

namespace SmartX.Api.Services
{
    public class CommandHistoryStore
    {
        private readonly object _lock = new();

        private readonly Dictionary<int, SmartCommand> _commands = new();

        public bool RecordCommand(SmartCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (command.Id <= 0)
            {
                throw new ArgumentException(
                    "Command ID must be greater than zero.",
                    nameof(command));
            }

            if (string.IsNullOrWhiteSpace(command.DeviceId))
            {
                throw new ArgumentException(
                    "Device ID is required.",
                    nameof(command));
            }

            lock (_lock)
            {
                // Prevent duplicate command IDs.
                if (_commands.ContainsKey(command.Id))
                {
                    return false;
                }

                // Store a snapshot so later client-side changes
                // do not silently modify the server's history.
                SmartCommand storedCommand = new SmartCommand
                {
                    Id = command.Id,
                    DeviceId = command.DeviceId,
                    CommandType = command.CommandType,
                    Description = command.Description,
                    Priority = command.Priority,
                    CreatedAt = command.CreatedAt,
                    Status = command.Status,
                    IsUndoable = command.IsUndoable,
                    ProcessedAt = command.ProcessedAt,
                    WasSuccessful = command.WasSuccessful
                };

                _commands.Add(storedCommand.Id, storedCommand);

                return true;
            }
        }

        public List<SmartCommand> GetAllCommands()
        {
            lock (_lock)
            {
                return _commands.Values
                    .OrderBy(command => command.ProcessedAt)
                    .ThenBy(command => command.Id)
                    .ToList();
            }
        }

        public List<SmartCommand> GetCommandsForDevice(string deviceId)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                throw new ArgumentException(
                    "Device ID is required.",
                    nameof(deviceId));
            }

            lock (_lock)
            {
                return _commands.Values
                    .Where(command =>
                        command.DeviceId.Equals(
                            deviceId,
                            StringComparison.OrdinalIgnoreCase))
                    .OrderBy(command => command.ProcessedAt)
                    .ThenBy(command => command.Id)
                    .ToList();
            }
        }

        public int Count
        {
            get
            {
                lock (_lock)
                {
                    return _commands.Count;
                }
            }
        }
    }
}