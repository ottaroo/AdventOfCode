using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using PuzzleSolverLib.Services;

namespace PuzzleSolverBenchmark
{
    [MemoryDiagnoser]
    public class PuzzleBenchmark()
    {

        [Benchmark]
        public void SolvePuzzle_2024_Day1b()
        {
            var p = new PuzzleFactoryService();
            var puzzle = p.CreatePuzzle(2024, 1, 2);
            puzzle.Solve();

        }

        [Benchmark]
        public void SolvePuzzle_2024_Day1c()
        {
            var p = new PuzzleFactoryService();
            var puzzle = p.CreatePuzzle(2024, 1, 3);
            puzzle.Solve();

        }

    }

    

    internal class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<PuzzleBenchmark>();
        }
    }
}
