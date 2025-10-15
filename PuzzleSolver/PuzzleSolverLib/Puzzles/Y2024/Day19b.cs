using System.Buffers;
using System.Collections;
using System.Collections.Concurrent;
using System.Text;
using Microsoft.CodeAnalysis.Operations;
using PuzzleSolverLib.Common;

namespace PuzzleSolverLib.Puzzles.Y2024;

public class Day19b : PuzzleBaseClass
{
    //TODO: Get back to this - not working correctly yet - or at least not fast enough to bother wait for result
    // Test this later:
    // Did some work on the patterns - do some more, maybe exclude more 2-3 letter patterns
    // Then test all patterns - find valid
    // then loop over all valid and check which of the longer patterns can be used and sum up all valid combos


    public (string text, int numberOfCombinations)[] FindAllWithSimpleSolution(string[] lines, string[] patterns)
    {
        var options = new ParallelOptions() {MaxDegreeOfParallelism = Environment.ProcessorCount};
        var solutions = new ConcurrentBag<(string text, int numberOfCombinations)>();
        var lineNo = 0;

        Console.Write($"Started: {DateTime.Now}");

        Parallel.ForEach(lines, options, line =>
        {
            var ln =Interlocked.Increment(ref lineNo);
            Console.WriteLine($"Processing line: {ln} / {lines.Length}");

            var queue = new Queue<(Range range, int combinations)>();
            queue.Enqueue((new Range(0, line.Length),0));

            while (queue.TryDequeue(out var test))
            {
                foreach (var pattern in patterns)
                {
                    if (line[test.range].EndsWith(pattern, StringComparison.OrdinalIgnoreCase))
                    {
                        if (test.range.End.Value == pattern.Length)
                        {
                            solutions.Add((line, test.combinations + 1));
                            break;
                        }

                        queue.Enqueue((new Range(test.range.Start, test.range.End.Value - pattern.Length), test.combinations + 1));
                    }
                }

            }
        });

        return solutions.Distinct().ToArray();
    }





    public override string? OnSolve(ReadOnlySpan<char> inputFile)
    {
        var lines = File.ReadAllLines(inputFile.ToString()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        var allPatterns = lines.First().Split(',').Select(t => t.Trim()).ToArray();

        var allOnePatterns = allPatterns.Where(x=>x.Length == 1).ToArray();
        var allTwoPatterns = allPatterns.Where(x => x.Length == 2).ToArray();
        var allThreePatterns = allPatterns.Where(x => x.Length == 3).ToArray();
        var allFourPatterns = allPatterns.Where(x => x.Length == 4).ToArray();
        var allFivePatterns = allPatterns.Where(x => x.Length == 5).ToArray();
        var allSixPatterns = allPatterns.Where(x => x.Length == 6).ToArray();
        var allSevenPatterns = allPatterns.Where(x => x.Length == 7).ToArray();
        var allEightPatterns = allPatterns.Where(x => x.Length == 8).ToArray();

        // var allButPatterns = allPatterns.Where(x => x.Length > 2).ToArray();
        // var combinations =  FindAllWithSimpleSolution(allButPatterns, myCombo.ToArray());
        // var withoutSolution = allButPatterns.Except(allButPatterns.Intersect(combinations)).ToList();
        //
        // did some testing
        // all patterns can be made from these (so 37 vs 420)
        var myCombo = new List<string>();
        myCombo.AddRange(allOnePatterns);
        myCombo.AddRange(allTwoPatterns);
        myCombo.AddRange(["www", "wuw", "wrw", "wgw", "wbw", "ugw", "ggw", "bgw"]);
        myCombo.Add("wggw");
        myCombo.Add("gwugw");

        var dictionary = new Dictionary<string, int>();

        var combinations =  FindAllWithSimpleSolution(lines.Skip(1).ToArray(), myCombo.ToArray());



        //var trie = new Trie();
        //foreach (var pattern in allPatterns)
        //    trie.Insert(pattern);

        //var allSolutions = 0;

        //var maxPatternSize = allPatterns.Max(x => x.Length);

        //var lineNo = 0;

        //foreach (var line in lines.Skip(1))
        //{
        //    lineNo++;
        //    Console.WriteLine($"Working on line [{lineNo}]: {line}");

        //    var possibleLines = new List<string>() {string.Empty};

        //    for (var i = 0; i < line.Length; i++)
        //    {
        //        for (var p = 1; p <= maxPatternSize; p++)
        //        {
        //            if (i + p > line.Length)
        //                break;
        //            var tries =trie.SearchByPrefix(line[i..(i+p)]);

        //            var pLines = possibleLines.Where(x => x.Length == i).ToArray();
        //            foreach (var pl in pLines)
        //            {
        //                foreach(var t in tries)
        //                    possibleLines.Add($"{pl}{t}");
        //            }

        //            possibleLines.RemoveAll(x => pLines.Any(y => y.Equals(x)));



        //            //var r = trie.SearchByPrefix(line[i..(i + sw)]);
        //            //List<string> buildLines;
        //            //List<string> moreBuildLines;
        //            //if (!possibleLines.TryGetValue(i, out buildLines))
        //            //    buildLines = new List<string>() {string.Empty};

        //            //foreach (var bl in buildLines)
        //            //{
        //            //    foreach (var a in r)
        //            //    {
        //            //        if (!possibleLines.TryGetValue(i + sw, out moreBuildLines))
        //            //        {
        //            //            moreBuildLines = new List<string>();
        //            //            possibleLines.Add(i+sw, moreBuildLines);
        //            //        }

        //            //        var ns = $"{bl}{a}";
        //            //        if (ns.Length < line.Length && line.StartsWith(ns, StringComparison.OrdinalIgnoreCase))
        //            //            moreBuildLines.Add(ns);
        //            //        if (ns.Length == line.Length && line.Equals(ns, StringComparison.OrdinalIgnoreCase))
        //            //            solution++;
        //            //    }
        //            //}

        //            //possibleLines.Remove(i);


        //        }
                


        //    }


        //    allSolutions += possibleLines.Count(x => x.Equals(line, StringComparison.OrdinalIgnoreCase));
        //}



        return "wip";

    }
}