namespace PuzzleSolverLib.Puzzles
{
    public interface IPuzzle
    {
        string Solve(ReadOnlySpan<char> inputFile);
        string Solve();
    }
}
