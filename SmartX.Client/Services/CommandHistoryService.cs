using SmartX.Shared.Models;

namespace SmartX.Client.Services
{
    public class CommandHistoryService
    {
        private readonly Stack<SmartCommand> _undoStack = new();

        public int Count => _undoStack.Count;

        public bool CanUndo => _undoStack.Count > 0;

        public void RecordProcessedCommand(SmartCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (command.IsUndoable)
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
    }
}