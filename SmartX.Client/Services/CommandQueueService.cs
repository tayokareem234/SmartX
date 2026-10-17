using SmartX.Shared.Models;

namespace SmartX.Client.Services
{
    public class CommandQueueService
    {
        private readonly Queue<SmartCommand> _commandQueue = new();

       

        public int Count => _commandQueue.Count;

        public bool HasPendingCommands => _commandQueue.Count > 0;

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

            SmartCommand command = new SmartCommand
            {
                Id = CommandIdGenerator.GetNextId(),
                DeviceId = deviceId,
                CommandType = commandType,
                Description = description,
                Priority = priority,
                CreatedAt = DateTime.UtcNow,
                Status = "Pending",
                IsUndoable = isUndoable
            };

            _commandQueue.Enqueue(command);

            return command;
        }

        public SmartCommand? Dequeue()
        {
            if (_commandQueue.Count == 0)
            {
                return null;
            }

            return _commandQueue.Dequeue();
        }

        public List<SmartCommand> GetPendingCommands()
        {
            return _commandQueue.ToList();
        }

        public void Clear()
        {
            _commandQueue.Clear();
        }
    }
}