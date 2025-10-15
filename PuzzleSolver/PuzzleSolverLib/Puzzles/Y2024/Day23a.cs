using System.Text;

namespace PuzzleSolverLib.Puzzles.Y2024;

public class Day23a : PuzzleBaseClass
{

    public IEnumerable<string> SearchForConnection(string[] lines, string input)
    {
        var lanNames = input.Split(",").OrderBy(x=>x).ToList();

        var queue = new Queue<string>();
        queue.Enqueue(input);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            var c1 = string.Empty;
            var c2 = string.Empty;
            var thirdConnection = string.Empty;
            if (current.IndexOf(',') > -1)
            {
                var connections = current.Split(',');
                c1 = connections[0].Split('-')[1];
                c2 = connections[1].Split('-')[1];

                thirdConnection = string.Join("-", new string[] {c1, c2}.OrderBy(x => x));
            }

            foreach (var line in lines)
            {
                var sortLine = string.Join("-", line.Split('-').OrderBy(x => x));

                if (current.Contains(sortLine, StringComparison.OrdinalIgnoreCase))
                    continue;

                if (current.IndexOf(',') == -1 && current.StartsWith(sortLine.Substring(0, sortLine.IndexOf('-')), StringComparison.OrdinalIgnoreCase))
                {
                    queue.Enqueue($"{current},{sortLine}");
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(thirdConnection)) {

                    if (sortLine.Equals(thirdConnection, StringComparison.OrdinalIgnoreCase))
                    {
                        var totalLine = $"{current},{sortLine}";
                        var computers = totalLine.Replace("-", ",").Split(",").Distinct().OrderBy(x => x).ToList();
                        yield return string.Join(",", computers);
                    }
                }
            }
        }

    }


    public override string? OnSolve(ReadOnlySpan<char> inputFile)
    {

        var lines = File.ReadAllLines(inputFile.ToString());

        var groups = new HashSet<string>();
        foreach(var computer in lines)
        foreach (var s in SearchForConnection(lines, computer))
            groups.Add(s);

        return groups.Where(x=>x.Split(',').Any(s=>s.StartsWith("t", StringComparison.OrdinalIgnoreCase))).ToList().Count.ToString();


    }
}