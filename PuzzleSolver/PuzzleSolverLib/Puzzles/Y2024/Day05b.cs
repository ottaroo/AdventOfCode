using System.Text.RegularExpressions;

namespace PuzzleSolverLib.Puzzles.Y2024;

public partial class Day05b : PuzzleBaseClass
{
    [GeneratedRegex(@"\d+")]
    public partial Regex Numbers();

    public (SortedList<int,HashSet<int>> Rules, List<List<int>> Pages) ParseFile(ReadOnlySpan<char> inputFile)
    {
        var rules = new SortedList<int, HashSet<int>>();
        var pages = new List<List<int>>();

        using var fs = new FileStream(inputFile.ToString(), FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan);
        using var sr = new StreamReader(fs);

        while (!sr.EndOfStream)
        {
            var line = sr.ReadLine();
            if (string.IsNullOrWhiteSpace(line)) continue;
            var index = line.IndexOf('|');
            if (index != -1)
            {
                var key = int.Parse(line[..index]);
                var value = int.Parse(line[(index+1)..]);

                if (rules.TryGetValue(key, out var list))
                    list.Add(value);
                else
                    rules.Add(key, [value]);
            }
            else
            {
                var pageOrdering = new List<int>();
                foreach (Match match in Numbers().Matches(line))
                {
                    pageOrdering.Add(int.Parse(match.Value));
                }

                pages.Add(pageOrdering);
            }
        }

        return (rules, pages);
    }

    public bool IsInCorrectOrder(ref List<int> pageOrdering)
    {
        for (var n = 1; n < pageOrdering.Count; n++)
        {
            if (!Data.Rules.TryGetValue(pageOrdering[n], out var mustAppearBeforeTheseKeys))
                continue; // no particular rules for this number
            if (pageOrdering.Take(n).Any(x => mustAppearBeforeTheseKeys.Contains(x)))
                return false; // wrong order
        }
        return true;
    }

    public class SortingRules(SortedList<int, HashSet<int>> rules) : IComparer<int>
    {

        public int Compare(int x, int y)
        {
            if (rules.TryGetValue(y, out var mustAppearBeforeAnyOfThese) && mustAppearBeforeAnyOfThese.Contains(x))
                return -1;
            return 1;
        }
    }

    public (SortedList<int,HashSet<int>> Rules, List<List<int>> Pages) Data { get; set; }


    public override string? OnSolve(ReadOnlySpan<char> inputFile)
    {
        try
        {
            var result = 0;
            Data = ParseFile(inputFile);
            var sorter = new SortingRules(Data.Rules);


            foreach (var pageOrdering in Data.Pages)
            {
                var localOrdering = pageOrdering;

                if (IsInCorrectOrder(ref localOrdering)) continue;

                // Fix order
                var correctOrder = localOrdering.OrderBy(x=>x, sorter).ToList();
                result += correctOrder[correctOrder.Count / 2];
            }


            return result.ToString();
        }
        catch (Exception ex)
        {
            LastError = ex;
            return null;
        }
    }
}