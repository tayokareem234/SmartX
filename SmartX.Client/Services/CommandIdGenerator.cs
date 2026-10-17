namespace SmartX.Client.Services
{
    public static class CommandIdGenerator
    {
        private static int _nextId = 0;

        public static int GetNextId()
        {
            return Interlocked.Increment(ref _nextId);
        }
    }
}