using System.Text.RegularExpressions;

namespace PuzzleSolverLib.Puzzles.Y2024;

public partial class Day02b : PuzzleBaseClass
{

    [GeneratedRegex("\\d+")]
    public partial Regex Numbers();


    private int[] GetDiff(ReadOnlySpan<int> numbers)
    {
        var diffs = new int[numbers.Length - 1];
        for (var n = 1; n < numbers.Length; n++)
        {
            diffs[n - 1] = numbers[n] - numbers[n - 1];
        }
        return diffs;
    }

    private bool IsSafelyIncreasingOrDecreasing(ReadOnlySpan<int> numbers)
    {
        var diff = GetDiff(numbers);
        if (diff.All(x=> x > 0 && x <= 3) || diff.All(x=>x < 0 && x >= -3))
            return true;
        return false;
    }

    public bool IsSafeReport(ReadOnlySpan<int> numbers)
    {
        if (IsSafelyIncreasingOrDecreasing(numbers))
            return true;

        var tmp = new int[numbers.Length - 1];

        // We accept one error - test if we have a good report if we skip one number
        var indexToSkip = 0;
        for (var i = 0; i < numbers.Length; i++)
        {
            for (var n = 0; n < numbers.Length; n++)
            {
                if (indexToSkip == n) continue;

                tmp[n > indexToSkip ? n - 1 : n] = numbers[n];
            }
            indexToSkip++;
            if (IsSafelyIncreasingOrDecreasing(tmp))
                return true;
        }
        return false;
    }


    public override string? OnSolve(ReadOnlySpan<char> inputFile)
    {
        try
        {
            var safeReports = 0;
            using var fs = new FileStream(inputFile.ToString(), FileMode.Open, FileAccess.Read, FileShare.Read);
            using var sr = new StreamReader(fs);

            var row = 0;
            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;
                var matches = Numbers().Matches(line);
                if (!matches.Any()) continue;

                var numbers = matches.Select(x => int.Parse(x.Value)).ToArray();


                if (IsSafeReport(numbers))
                    safeReports++;

                row++;
            }

            return safeReports.ToString();
        }
        catch (Exception ex)
        {
            LastError = ex;
            return null;
        }

    }
}