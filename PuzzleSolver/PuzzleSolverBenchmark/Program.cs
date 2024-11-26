using System.Buffers;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using PuzzleSolverLib.Puzzles;
using PuzzleSolverLib.Services;

namespace PuzzleSolverBenchmark
{
    [MemoryDiagnoser]
    public class PuzzleBenchmark()
    {

        public IPuzzle Puzzle { get; set; }
        public string Path { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Benchmarking PuzzleSolver");

            this.Puzzle = Program.Puzzle;
            this.Path = Program.Path;
        }


        [Benchmark]
        public void SolvePuzzle()
        {
            if (string.IsNullOrWhiteSpace(Path))
                Puzzle.Solve();
            else
                Puzzle.Solve(Path);
        }
    }

    

    internal class Program
    {
        public static IPuzzle Puzzle { get; set; }
        public static string Path { get; set; }

        private static bool TryGetArgumentWithValue(ReadOnlySpan<string> args, ReadOnlySpan<string> argumentSwithWithAliases, bool isRequired, out ReadOnlySpan<char> argValue)
        {
            argValue = default;
            var index = args.IndexOfAny(SearchValues.Create(argumentSwithWithAliases, StringComparison.OrdinalIgnoreCase));
            if (index == -1 || index + 1 >= args.Length)
            {
                if (isRequired)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Value for argument [{string.Join(" | ", argumentSwithWithAliases.ToArray())}] is required");
                    Environment.Exit(-1);
                }

                return false;
            }

            argValue = args[index + 1];

            return true;
        }

        private static bool TryGetArgumentSwitch(ReadOnlySpan<string> args, ReadOnlySpan<string> argumentSwithWithAliases)
        {
            return args.IndexOfAny(SearchValues.Create(argumentSwithWithAliases, StringComparison.OrdinalIgnoreCase)) != -1;
        }

        static void Main(string[] args)
        {
            _ = TryGetArgumentWithValue(args, ["-y", "--year"], true, out var year);
            _ = TryGetArgumentWithValue(args, ["-d", "--day"], true, out var day);
            _ = TryGetArgumentWithValue(args, ["-p", "--part"], true, out var part);
            _ = TryGetArgumentWithValue(args, ["--path"], false, out var path);

            var p = new PuzzleFactoryService();
            Puzzle = p.CreatePuzzle(int.Parse(year), int.Parse(day), int.Parse(part));
            Path = path.ToString();


            BenchmarkRunner.Run<PuzzleBenchmark>();
        }
    }
}
