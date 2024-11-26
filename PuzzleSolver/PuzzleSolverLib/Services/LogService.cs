namespace PuzzleSolverLib.Services
{
    public class LogService : ILogService
    {
        private ConsoleColor _defaultColor;

        public LogService()
        {
            _defaultColor = Console.ForegroundColor;
        }

        public void WriteDebug(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"DEBUG: {message}");
        }
        public void WriteError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"ERROR: {message}");
        }
        public void WriteInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"INFO: {message}");
        }
        public void WriteSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"SUCCESS: {message}");
        }
        public void WriteWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"WARNING: {message}");
        }

        public void Dispose()
        {
            Console.ForegroundColor = _defaultColor;
            Console.WriteLine();
        }

        public void EmptyLine()
        {
            Console.ForegroundColor = _defaultColor;
            Console.WriteLine();
        }
    }
}
