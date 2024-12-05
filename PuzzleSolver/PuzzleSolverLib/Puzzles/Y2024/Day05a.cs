using System.Text.RegularExpressions;

namespace PuzzleSolverLib.Puzzles.Y2024;

public partial class Day05a : PuzzleBaseClass
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



    public override string? OnSolve(ReadOnlySpan<char> inputFile)
    {
        try
        {
            var result = 0;
            var data = ParseFile(inputFile);

            foreach (var pageOrdering in data.Pages)
            {

                for (var n = 1; n < pageOrdering.Count; n++)
                {
                    if (!data.Rules.TryGetValue(pageOrdering[n], out var mustAppearBeforeTheseKeys))
                        continue; // no particular rules for this number
                    if (pageOrdering.Take(n).Any(x => mustAppearBeforeTheseKeys.Contains(x)))
                        goto WRONG_ORDER; // wrong order

                }

                // get middle number of correct order
                result += pageOrdering[pageOrdering.Count / 2];
                WRONG_ORDER: ;
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