using System.Text.RegularExpressions;

namespace PuzzleSolverLib.Puzzles.Y2023
{

    public partial class Day03a : PuzzleBaseClass
    {
        public override string Description => @"The engineer explains that an engine part seems to be missing from the engine, but nobody can figure out which one. If you can add up all the part numbers in the engine schematic, it should be easy to work out which part is missing.

The engine schematic (your puzzle input) consists of a visual representation of the engine. There are lots of numbers and symbols you don't really understand, but apparently any number adjacent to a symbol, even diagonally, is a ""part number"" and should be included in your sum. (Periods (.) do not count as a symbol.)

Here is an example engine schematic:

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
In this schematic, two numbers are not part numbers because they are not adjacent to a symbol: 114 (top right) and 58 (middle right). Every other number is adjacent to a symbol and so is a part number; their sum is 4361.

Of course, the actual engine schematic is much larger. What is the sum of all of the part numbers in the engine schematic?";

        [GeneratedRegex(@"[^0123456789\.]", RegexOptions.IgnoreCase)]
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

                        Log.WriteError($"[NO MATCH] {numberMatch.Value}");
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

        private bool SearchForAdjacentSymbol(ReadOnlySpan<char> checkLine, Match numberMatch, ref int sumOfParts)
        {
            foreach (var match in SchemaSymbols().EnumerateMatches(checkLine))
            {
                if (match.Index >= Math.Max(numberMatch.Index - 1, 0) && match.Index <= numberMatch.Index + numberMatch.Length)
                {
                    Log.WriteSuccess($"FOUND MATCH: Found symbol [{checkLine[match.Index]}] adjacent to number: {numberMatch.Value}");
                    sumOfParts += int.Parse(numberMatch.Value);
                    return true;
                }
            }

            return false;
        }

    }
}
