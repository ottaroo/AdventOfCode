using System.Text.RegularExpressions;
using PuzzleSolverLib.Common;

namespace PuzzleSolverLib.Puzzles.Y2024;

public class Day11b : PuzzleBaseClass
{
    public override string? OnSolve(ReadOnlySpan<char> inputFile)
    {
        var txt = File.ReadAllText(inputFile.ToString());

        var stones = new LinkedList<long>();

        // 3028 78 973951 5146801 5 0 23533 857
        //foreach (Match match in RegularExpressions.Numbers().Matches(txt))
        //    stones.AddLast(long.Parse(match.Value));

        // Notes:
        //
        // 3028: After 12 iterations, it always produce 55 unique stones
        // 78: After 14 iterations, it always produce 54 unique stones
        // 973951: ???
        // 5146801: ???
        // 5: After 14 iterations, it always produce 54 unique stones
        // 0: After 15 iterations, it always produce 54 unique stones
        // 23533: ???
        // 857: ???



        stones.AddLast(857);
        var uniqueStones = new HashSet<long>();

        using var fs = new FileStream(@"d:\temp\day11_file1.txt", FileMode.Create, FileAccess.Write, FileShare.Read);
        using var sw = new StreamWriter(fs);

        for (var n = 0; n < 25; n++)
        {
            var countNewStones = 0;

            var stone = stones.First;
            while (stone != null)
            {
                if (stone.Value == 0)
                {
                    stone.Value = 1;
                    sw.WriteLine($"{n}, 0, 1");
                    goto RULES_PROCESSED;
                }

                var s = stone.Value.ToString();
                if (s.Length % 2 == 0)
                {
                    var half = s.Length / 2;
                    var firstHalf = s.Substring(0, half);
                    var secondHalf = s.Substring(half);
                    stone.Value = long.Parse(firstHalf);
                    sw.WriteLine($"({n}), {s}, {firstHalf}");
                    sw.WriteLine($"({n}), {s}, {secondHalf}");
                    uniqueStones.Add(stone.Value);
                    stone = stones.AddAfter(stone, long.Parse(secondHalf));
                    uniqueStones.Add(stone.Value);
                    countNewStones++;
                    goto RULES_PROCESSED;
                }

                var prev = stone.Value;
                stone.Value *= 2024;
                sw.WriteLine($"{n}, {prev}, {stone.Value}");
                uniqueStones.Add(stone.Value);

                RULES_PROCESSED: ;
                stone = stone.Next;
            }

           // sw.WriteLine($"{n+1}, {countNewStones}, {uniqueStones.Count}");

        }

        return stones.Count.ToString();

    }
}