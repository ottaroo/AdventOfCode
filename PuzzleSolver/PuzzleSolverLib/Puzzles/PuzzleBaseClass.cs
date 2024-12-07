using PuzzleSolverLib.Services;

namespace PuzzleSolverLib.Puzzles
{
    public abstract class PuzzleBaseClass : IPuzzle
    {
        protected ILogService Log { get; } = new LogService();
        protected Exception? LastError { get; set; } = null;

        public string Solve(ReadOnlySpan<char> inputFile)
        {
            try
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
            catch(Exception ex)
            {
                Log.WriteError($"An error occurred while solving the puzzle: {ex.Message}");
                LastError = ex;
                return "An error occurred while solving the puzzle";
            }
        }

        public string Solve()
        {
            var puzzleName = GetType().Name;
            var puzzleNamespace = GetType().Namespace;
            var puzzleYear = puzzleNamespace!.Split('.').Last();

            return Solve(Path.Combine(AppContext.BaseDirectory, "Puzzles", puzzleYear, "InputFiles", $"{puzzleName.TrimEnd('a', 'A', 'b', 'B', 'c', 'C', 'd', 'D', 'e', 'E')}.txt"));
        }

        public abstract string? OnSolve(ReadOnlySpan<char> inputFile);


        public virtual string Description
        {
            get
            {
                var day = GetType().Name[^3..^1];
                var year = GetType().Namespace![^4..];

                return $"Too long, didn't read... see http://adventToCode.com/{int.Parse(year)}/{int.Parse(day)}";
            }
        }
    }
}
