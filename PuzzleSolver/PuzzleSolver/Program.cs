using System.Buffers;
using PuzzleSolver.Services;

namespace PuzzleSolver
{
    internal class Program
    {

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
            var puzzle = p.CreatePuzzle(int.Parse(year), int.Parse(day), int.Parse(part));
            puzzle.Solve();

            //if (!path.IsEmpty)
            //{
            //    puzzle.Solve(path);
            //}
            //else
            //{
            //    puzzle.Solve();
            //}


        }
    }
}
