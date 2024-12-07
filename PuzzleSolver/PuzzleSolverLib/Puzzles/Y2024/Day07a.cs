using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PuzzleSolverLib.Puzzles.Y2024;

public partial class Day07a : PuzzleBaseClass
{
    [GeneratedRegex(@"\d+")]
    public partial Regex Numbers();

    public override string? OnSolve(ReadOnlySpan<char> inputFile)
    {
        using var fs = new FileStream(inputFile.ToString(), FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan);
        using var sr = new StreamReader(fs);

        var numberOfCorrectCalibrations = 0L;

        while (!sr.EndOfStream)
        {
            var line = sr.ReadLine();
            if (string.IsNullOrWhiteSpace(line)) continue;

            var matches = Numbers().Matches(line);
            if (!matches.Any()) continue;

            var numbers = new List<long>();
            foreach(Match match in matches)
                numbers.Add(long.Parse(match.Value));

            var expectedAnswer = numbers.First();

            var options = (int)Math.Pow(2, numbers.Count - 2);
            for (var o = 0; o < options; o++)
            {
                var evaluator = o; // 0 multiply / 1 add
                var sum = numbers[1];
                for (var n = 2; n < numbers.Count; n++)
                {
                    if ((evaluator & 0x1) == 0)
                        sum *= numbers[n];
                    else
                        sum += numbers[n];

                    evaluator >>= 1;
                }

                
                if (sum == expectedAnswer)
                {
                    // found at least one valid combination
                    numberOfCorrectCalibrations += expectedAnswer;
                    break;
                }
            }
        }

        return numberOfCorrectCalibrations.ToString();
    }
}