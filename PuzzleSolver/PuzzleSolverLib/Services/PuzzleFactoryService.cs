using PuzzleSolverLib.Puzzles;

namespace PuzzleSolverLib.Services;

public interface IPuzzleFactoryService
{
    IPuzzle CreatePuzzle(int year, int day, int part);
}