using System.Text.RegularExpressions;
using PuzzleSolverLib.Common;

namespace PuzzleSolverLib.Puzzles.Y2024;

public class Day11b : PuzzleBaseClass
{
    public override string? OnSolve(ReadOnlySpan<char> inputFile)
    {
        var txt = File.ReadAllText(inputFile.ToString());


        var currentStateOfStones = new Dictionary<long, long>();
        // 3028 78 973951 5146801 5 0 23533 857
        foreach (Match match in RegularExpressions.Numbers().Matches(txt))
            currentStateOfStones.Add(long.Parse(match.Value), 1);

        var initialCount = (long)currentStateOfStones.Count;


        for (var n = 0; n < 75; n++)
        {
            var newStateOfStones = new Dictionary<long, long>();

            foreach (var stone in currentStateOfStones.Keys)
            {
                if (stone == 0)
                {
                    if (newStateOfStones.ContainsKey(1))
                        newStateOfStones[1] += currentStateOfStones[stone];
                    else
                        newStateOfStones.Add(1, currentStateOfStones[stone]);
                    goto RULES_PROCESSED;
                }

                var s = stone.ToString();
                if (s.Length % 2 == 0)
                {
                    var count = currentStateOfStones[stone];

                    var half = s.Length / 2;
                    var firstHalf = s.Substring(0, half);
                    var secondHalf = s.Substring(half);
                    var fh = long.Parse(firstHalf);
                    var sh = long.Parse(secondHalf);
                    if (newStateOfStones.ContainsKey(fh))
                        newStateOfStones[fh] += count;
                    else
                        newStateOfStones.Add(fh, count);
                    if (newStateOfStones.ContainsKey(sh))
                        newStateOfStones[sh] += count;
                    else
                        newStateOfStones.Add(sh, count);

                    initialCount += count;

                    goto RULES_PROCESSED;
                }

                var newKey = stone * 2024;
                var currentCount = currentStateOfStones[stone];
                if (newStateOfStones.ContainsKey(newKey))
                    newStateOfStones[newKey] += currentCount;
                else
                    newStateOfStones.Add(newKey, currentCount);

                RULES_PROCESSED: ;
            }

            currentStateOfStones = newStateOfStones;
        }

        Log.WriteWarning(currentStateOfStones.Sum(x=>x.Value).ToString());

        return initialCount.ToString();

    }
}