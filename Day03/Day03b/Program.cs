using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Linq;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Attributes;
using Microsoft.Diagnostics.Tracing.Parsers.ClrPrivate;

namespace Day03b
{

    [MemoryDiagnoser]
    public partial class Program
    {
        [GeneratedRegex(@"[*]", RegexOptions.IgnoreCase)]
        private static partial Regex SchemaSymbols();

        [GeneratedRegex(@"[0123456789]+", RegexOptions.IgnoreCase)]
        private static partial Regex Numbers();


        static void Main(string[] args)
        {
            //var summary = BenchmarkRunner.Run<Program>();
            ExecuteMain();
        }

        [Benchmark]
        public static void ExecuteMain()
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



                var gearMatches = SchemaSymbols().Matches(currentLine);
                foreach (Match match in gearMatches)
                {
                    SearchForAdjacentNumbers(match.Index, previousLine, currentLine, nextLine, ref sumOfPartNumbers);

                }

                previousLine = currentLine;

            }

            Console.WriteLine($"Sum of Parts: {sumOfPartNumbers}");
        }

        private static bool SearchForAdjacentNumbers(int gearPosition, ReadOnlySpan<char> line, ref int sum, ref int count)
        {
            if (count > 2)
                return false;

            if (line.Length > 0)
            {
                foreach (Match number in Numbers().Matches(line.ToString()))
                {
                    if (count > 2)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"[False positive] More than two numbers connected to gear");
                        return false;
                    }

                    if (gearPosition >= Math.Max(number.Index - 1, 0) && gearPosition <= number.Index + number.Length)
                    {
                        if (sum == 0)
                            sum = int.Parse(number.Value);
                        else
                            sum *= int.Parse(number.Value);
                        count++;
                    }
                }
            }

            return count <= 2;
        }

        private static bool SearchForAdjacentNumbers(int gearPosition, ReadOnlySpan<char> previous, ReadOnlySpan<char> current, ReadOnlySpan<char> next, ref int sumOfParts)
        {

            var partSum = 0;
            var numberOfMatches = 0;

            if (previous.Length > 0)
            {
                if (!SearchForAdjacentNumbers(gearPosition, previous, ref partSum, ref numberOfMatches))
                    goto FAILED_TO_FIND_OR_FOUND_TO_MANY;
            }
            if (current.Length > 0)
            {
                if (!SearchForAdjacentNumbers(gearPosition, current, ref partSum, ref numberOfMatches))
                    goto FAILED_TO_FIND_OR_FOUND_TO_MANY;
            }
            if (next.Length > 0)
            {
                if (!SearchForAdjacentNumbers(gearPosition, next, ref partSum, ref numberOfMatches))
                    goto FAILED_TO_FIND_OR_FOUND_TO_MANY;
            }


            if (numberOfMatches == 2)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[MATCH] Gear position: {gearPosition}");
                sumOfParts += partSum;
                return true;
            }


        FAILED_TO_FIND_OR_FOUND_TO_MANY: 
            Console.ForegroundColor = ConsoleColor.Red;
            if (numberOfMatches < 2)
                Console.WriteLine($"[NO MATCH] Gear position: {gearPosition}");
            else
                Console.WriteLine($"[False positive] More than two numbers connected to gear");

            return false;
        }



    }
}
