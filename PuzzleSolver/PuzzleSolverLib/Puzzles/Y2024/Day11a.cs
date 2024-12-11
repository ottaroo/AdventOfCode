using System.Text.RegularExpressions;
using PuzzleSolverLib.Common;

namespace PuzzleSolverLib.Puzzles.Y2024;

public class Day11a : PuzzleBaseClass
{
    public override string? OnSolve(ReadOnlySpan<char> inputFile)
    {
        var txt = File.ReadAllText(inputFile.ToString());

        var stones = new LinkedList<long>();

        // 3028 78 973951 5146801 5 0 23533 857
        foreach (Match match in RegularExpressions.Numbers().Matches(txt))
            stones.AddLast(long.Parse(match.Value));


        for (var n = 0; n < 25; n++)
        {
            var stone = stones.First;
            while (stone != null)
            {
                if (stone.Value == 0)
                {
                    stone.Value = 1;
                    goto RULES_PROCESSED;
                }

                var s = stone.Value.ToString();
                if (s.Length % 2 == 0)
                {
                    var half = s.Length / 2;
                    var firstHalf = s.Substring(0, half);
                    var secondHalf = s.Substring(half);
                    stone.Value = long.Parse(firstHalf);
                    stone = stones.AddAfter(stone, long.Parse(secondHalf));
                    goto RULES_PROCESSED;
                }

                stone.Value *= 2024;

                RULES_PROCESSED: ;
                stone = stone.Next;
            }
        }

        return stones.Count.ToString();

    }
}