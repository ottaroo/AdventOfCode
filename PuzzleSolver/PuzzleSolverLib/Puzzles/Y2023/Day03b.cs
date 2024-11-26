using System.Text.RegularExpressions;

namespace PuzzleSolverLib.Puzzles.Y2023
{

    public partial class Day03b : PuzzleBaseClass
    {
        public override string Description => @"The missing part wasn't the only issue - one of the gears in the engine is wrong. A gear is any * symbol that is adjacent to exactly two part numbers. Its gear ratio is the result of multiplying those two numbers together.

This time, you need to find the gear ratio of every gear and add them all up so that the engineer can figure out which gear needs to be replaced.

Consider the same engine schematic again:

467..114..
...*......
..35..633.
......#...
617*......
.....+.58.
..592.....
......755.
...$.*....
.664.598..
In this schematic, there are two gears. The first is in the top left; it has part numbers 467 and 35, so its gear ratio is 16345. The second gear is in the lower right; its gear ratio is 451490. (The * adjacent to 617 is not a gear because it is only adjacent to one part number.) Adding up all of the gear ratios produces 467835.

What is the sum of all of the gear ratios in your engine schematic?";


        [GeneratedRegex(@"[*]", RegexOptions.IgnoreCase)]
        private static partial Regex SchemaSymbols();

        [GeneratedRegex(@"[0123456789]+", RegexOptions.IgnoreCase)]
        private static partial Regex Numbers();


        public override string? OnSolve(ReadOnlySpan<char> fileName)
        {
            try
            {
                using var fs = new FileStream(fileName.ToString(), FileMode.Open, FileAccess.Read);
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

                return sumOfPartNumbers.ToString();
            }
            catch (Exception ex)
            {
                LastError = ex;
                return null;
            }
        }

        private bool SearchForAdjacentNumbers(int gearPosition, ReadOnlySpan<char> line, ref int sum, ref int count)
        {
            if (count > 2)
                return false;

            if (line.Length > 0)
            {
                foreach (Match number in Numbers().Matches(line.ToString()))
                {
                    if (count > 2)
                    {
                        Log.WriteWarning($"[False positive] More than two numbers connected to gear");
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

        private bool SearchForAdjacentNumbers(int gearPosition, ReadOnlySpan<char> previous, ReadOnlySpan<char> current, ReadOnlySpan<char> next, ref int sumOfParts)
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
                Log.WriteSuccess($"[MATCH] Gear position: {gearPosition}");
                sumOfParts += partSum;
                return true;
            }


        FAILED_TO_FIND_OR_FOUND_TO_MANY:
            if (numberOfMatches < 2)
                Log.WriteInfo($"[NO MATCH] Gear position: {gearPosition}");
            else
                Log.WriteWarning($"[False positive] More than two numbers connected to gear");

            return false;
        }



    }
}
