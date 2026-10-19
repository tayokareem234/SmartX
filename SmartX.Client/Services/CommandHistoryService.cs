using SmartX.Shared.Models;

namespace SmartX.Client.Services
{
    public class CommandHistoryService
    {
        // Stack used to undo the most recent undoable command.
        private readonly Stack<SmartCommand> _undoStack = new();

        // Retains processed commands for historical analysis.
        private readonly List<SmartCommand> _commandHistory = new();

        public int Count => _undoStack.Count;

        public bool CanUndo => _undoStack.Count > 0;

        public int HistoryCount => _commandHistory.Count;

        public void RecordProcessedCommand(SmartCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            // Avoid recording the same command twice.
            if (!_commandHistory.Any(existing => existing.Id == command.Id))
            {
                _commandHistory.Add(command);
            }

            // Preserve existing undo functionality.
            if (command.IsUndoable &&
                !_undoStack.Contains(command))
            {
                _undoStack.Push(command);
            }
        }

        public SmartCommand? UndoLastCommand()
        {
            if (_undoStack.Count == 0)
            {
                return null;
            }

            return _undoStack.Pop();
        }

        public List<SmartCommand> GetCommandHistory()
        {
            return _commandHistory
                .OrderBy(command => command.CreatedAt)
                .ToList();
        }

        public List<SmartCommand> GetCommandsForDevice(string deviceId)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                throw new ArgumentException(
                    "Device ID is required.",
                    nameof(deviceId));
            }

            return _commandHistory
                .Where(command =>
                    command.DeviceId.Equals(
                        deviceId,
                        StringComparison.OrdinalIgnoreCase))
                .OrderBy(command => command.CreatedAt)
                .ToList();
        }

        // Retrieve commands grouped by their command type.
        public Dictionary<string, List<SmartCommand>>
            GetCommandsGroupedByType()
        {
            return _commandHistory
                .GroupBy(
                    command => command.CommandType,
                    StringComparer.OrdinalIgnoreCase)
                .ToDictionary(
                    group => group.Key,
                    group => group
                        .OrderBy(command => command.CreatedAt)
                        .ToList(),
                    StringComparer.OrdinalIgnoreCase);
        }

        // Count how often a particular command was processed.
        public int GetCommandFrequency(string commandType)
        {
            if (string.IsNullOrWhiteSpace(commandType))
            {
                throw new ArgumentException(
                    "Command type is required.",
                    nameof(commandType));
            }

            return _commandHistory.Count(command =>
                command.CommandType.Equals(
                    commandType,
                    StringComparison.OrdinalIgnoreCase));
        }

        // Identify the most frequently processed command type.
        public string? GetMostFrequentCommandType()
        {
            return _commandHistory
                .GroupBy(
                    command => command.CommandType,
                    StringComparer.OrdinalIgnoreCase)
                .OrderByDescending(group => group.Count())
                .ThenBy(group => group.Key)
                .Select(group => group.Key)
                .FirstOrDefault();
        }
    }
}