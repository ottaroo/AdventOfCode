using PuzzleSolverLib.Services;

namespace PuzzleSolverLib.Puzzles
{
    public abstract class PuzzleBaseClass : IPuzzle
    {
        protected ILogService Log { get; } = new LogService();
        protected Exception? LastError { get; set; } = null;

        public string Solve(ReadOnlySpan<char> inputFile)
        {
            var puzzleName = GetType().Name;
            var puzzleNamespace = GetType().Namespace;
            var puzzleYear = puzzleNamespace!.Split('.').Last();

            Log.WriteInfo($"Solving puzzle {puzzleName} [{puzzleYear.Substring(1)}]");
            Log.WriteInfo($"Input file: {inputFile}");

            var solution = OnSolve(inputFile);


            if (solution != null)
            {
                Log.EmptyLine();
                Log.WriteInfo(Description);
                Log.EmptyLine();


                Log.WriteSuccess($"Puzzle solution  = {solution}");
            }
            else
                Log.WriteError($"Puzzle solution not found - Exception: {(LastError?.ToString() ?? "N/A")}");

            return solution ?? "Failed to come up with an solution";
        }

        public string Solve()
        {
            var puzzleName = GetType().Name;
            var puzzleNamespace = GetType().Namespace;
            var puzzleYear = puzzleNamespace!.Split('.').Last();

            return Solve(Path.Combine(AppContext.BaseDirectory, "Puzzles", puzzleYear, "InputFiles", $"{puzzleName.TrimEnd('a', 'A', 'b', 'B')}.txt"));
        }

        public abstract string? OnSolve(ReadOnlySpan<char> inputFile);


        public abstract string Description { get; }
    }
}
