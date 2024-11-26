namespace PuzzleSolverLib.Services
{
    public interface ILogService : IDisposable
    {
        void WriteDebug(string message);
        void WriteError(string message);
        void WriteInfo(string message);
        void WriteSuccess(string message);
        void WriteWarning(string message);

        void EmptyLine();
    }
}