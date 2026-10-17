using SmartX.Shared.Models;

namespace SmartX.Client.Services
{
    public class CommandPriorityService
    {
        private readonly PriorityQueue<SmartCommand, int>
            _priorityQueue = new();

        private int _nextCommandId = 1;

        public int Count => _priorityQueue.Count;

        public bool HasPendingCommands => _priorityQueue.Count > 0;

        public SmartCommand Enqueue(
            string deviceId,
            string commandType,
            string description,
            string priority = "Normal",
            bool isUndoable = false)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                throw new ArgumentException(
                    "A device ID is required.",
                    nameof(deviceId));
            }

            if (string.IsNullOrWhiteSpace(commandType))
            {
                throw new ArgumentException(
                    "A command type is required.",
                    nameof(commandType));
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException(
                    "A command description is required.",
                    nameof(description));
            }

            int priorityValue = priority switch
            {
                "Critical" => 0,
                "High" => 1,
                "Normal" => 2,
                "Low" => 3,
                _ => throw new ArgumentException(
                    "Priority must be Critical, High, Normal, or Low.",
                    nameof(priority))
            };

            SmartCommand command = new SmartCommand
            {
                Id = _nextCommandId++,
                DeviceId = deviceId,
                CommandType = commandType,
                Description = description,
                Priority = priority,
                CreatedAt = DateTime.UtcNow,
                Status = "Pending",
                IsUndoable = isUndoable
            };

            _priorityQueue.Enqueue(command, priorityValue);

            return command;
        }

        public SmartCommand? Dequeue()
        {
            if (_priorityQueue.Count == 0)
            {
                return null;
            }

            return _priorityQueue.Dequeue();
        }

        public List<SmartCommand> GetPendingCommands()
        {
            return _priorityQueue
                .UnorderedItems
                .OrderBy(item => item.Priority)
                .Select(item => item.Element)
                .ToList();
        }
    }
}