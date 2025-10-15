using System.Buffers;
using System.Collections.Concurrent;
using System.Text;

namespace PuzzleSolverLib.Puzzles.Y2024;

public class Day19a : PuzzleBaseClass
{

    public string[] FindAllWithSimpleSolution(string[] lines, string[] patterns)
    {
        var options = new ParallelOptions() {MaxDegreeOfParallelism = Environment.ProcessorCount};
        var solutions = new ConcurrentBag<string>();

        Parallel.ForEach(lines, options, line =>
        {
            var queue = new Queue<Range>();
            queue.Enqueue(new Range(0, line.Length));

            while (queue.TryDequeue(out var test))
            {
                foreach (var pattern in patterns)
                {
                    if (line[test].EndsWith(pattern, StringComparison.OrdinalIgnoreCase))
                    {
                        if (test.End.Value == pattern.Length)
                        {
                            solutions.Add(line);
                            queue.Clear();
                            break;
                        }

                        queue.Enqueue(new Range(test.Start, test.End.Value - pattern.Length));
                    }
                }

            }
        });

        return solutions.Distinct().ToArray();
    }


    public override string? OnSolve(ReadOnlySpan<char> inputFile)
    {
        var lines = File.ReadAllLines(inputFile.ToString()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

        var allPatterns = lines.First().Split(',').Select(t=>t.Trim()).ToArray();

        var longestPattern = allPatterns.Max(x => x.Length);

        var foundSolution = new List<string>();

        for (var i = 1; i <= longestPattern; i++)
        {
            var linesToSearch = lines.Skip(1).Except(foundSolution).ToArray();
            var simpleSolution =FindAllWithSimpleSolution(linesToSearch, allPatterns.Where(x => x.Length <= i).OrderBy(x => x.Length).ToArray());
            foundSolution.AddRange(simpleSolution);
        }



        return foundSolution.Count.ToString();

    }
}