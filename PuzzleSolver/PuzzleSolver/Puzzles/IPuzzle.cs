namespace PuzzleSolver.Puzzles
{
    public interface IPuzzle
    {
        int Solve(ReadOnlySpan<char> inputFile);
        int Solve();
    }
}
