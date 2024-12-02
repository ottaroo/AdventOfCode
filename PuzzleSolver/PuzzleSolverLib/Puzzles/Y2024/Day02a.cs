using System.Globalization;
using System.Text.RegularExpressions;

namespace PuzzleSolverLib.Puzzles.Y2024;

public partial class Day02a : PuzzleBaseClass
{

    [GeneratedRegex("\\d+")]
    public partial Regex Numbers();


    public bool IsSafeReport(ReadOnlySpan<int> numbers)
    {
        var increasing = 0;
        for (var n = 1; n < numbers.Length; n++)
        {
            if (increasing == 0)
                increasing = numbers[n] > numbers[n - 1] ? 1 : -1;

            if (Math.Abs(numbers[n] - numbers[n - 1]) > 3 || numbers[n] - numbers[n-1] == 0 || (numbers[n] - numbers[n - 1]) * increasing < 0)
                return false;

        }

        return true;
    }


    public override string? OnSolve(ReadOnlySpan<char> inputFile)
    {
        try
        {
            var safeReports = 0;
            using var fs = new FileStream(inputFile.ToString(), FileMode.Open, FileAccess.Read, FileShare.Read);
            using var sr = new StreamReader(fs);

            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;
                var matches = Numbers().Matches(line);
                if (!matches.Any()) continue;

                var numbers = matches.Select(x => int.Parse(x.Value)).ToArray();

                if (IsSafeReport(numbers))
                    safeReports++;
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