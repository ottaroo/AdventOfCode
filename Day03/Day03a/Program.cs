using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Linq;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Attributes;

namespace Day03a
{

    [MemoryDiagnoser]
    public partial class Program
    {
        [GeneratedRegex(@"[^0123456789\.]", RegexOptions.IgnoreCase)]
        private static partial Regex SchemaSymbols();

        [GeneratedRegex(@"[0123456789]+", RegexOptions.IgnoreCase)]
        private static partial Regex Numbers();


        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Program>();
            //ExecuteMain();
        }

        [Benchmark]
        public void ExecuteMain()
        {
            using var fs = new FileStream(Path.Combine(AppContext.BaseDirectory, "input.txt"), FileMode.Open, FileAccess.Read);
            using var sr = new StreamReader(fs);


            string? previousLine = null;
            string? nextLine = null;
            var defaultColor = Console.ForegroundColor;

            var sumOfPartNumbers = 0;

            while (!sr.EndOfStream || nextLine != null)
            {
                var currentLine = nextLine ?? sr.ReadLine();
                if (currentLine == null)
                    continue;

                if (!sr.EndOfStream)
                    nextLine = sr.ReadLine();
                else
                    nextLine = null;



                var numberMatches = Numbers().Matches(currentLine);
                foreach (Match numberMatch in numberMatches)
                {
                    if (previousLine != null)
                    {
                        if (SearchForAdjacentSymbol(previousLine, numberMatch, ref sumOfPartNumbers))
                            continue;
                    }

                    if (SearchForAdjacentSymbol(currentLine, numberMatch, ref sumOfPartNumbers))
                        continue;

                    if (nextLine != null)
                    {
                        if (SearchForAdjacentSymbol(nextLine, numberMatch, ref sumOfPartNumbers))
                            continue;
                    }

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[NO MATCH] {numberMatch.Value}");
                    Console.ForegroundColor = defaultColor;
                }

                previousLine = currentLine;

            }

            Console.WriteLine($"Sum of Parts: {sumOfPartNumbers}");
        }

        private static bool SearchForAdjacentSymbol(ReadOnlySpan<char> checkLine, Match numberMatch, ref int sumOfParts)
        {
            foreach (var match in SchemaSymbols().EnumerateMatches(checkLine))
            {
                if (match.Index >= Math.Max(numberMatch.Index - 1, 0) && match.Index <= numberMatch.Index + numberMatch.Length)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("FOUND MATCH: ");
                    Console.WriteLine($"Found symbol [{checkLine[match.Index]}] adjacent to number: {numberMatch.Value}");
                    sumOfParts += int.Parse(numberMatch.Value);
                    return true;
                }
            }

            return false;
        }

    }
}
