using System.Text;
using System.Text.RegularExpressions;

namespace PuzzleSolverLib.Puzzles.Y2023;

public partial class Day06b : Day06a
{

    [GeneratedRegex(@"\s+")]
    public partial Regex Whitespace();

    public override string? OnSolve(ReadOnlySpan<char> inputFile)
    {
        try
        {
            using var fs = new FileStream(inputFile.ToString(), FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan);
            using var sr = new StreamReader(fs, Encoding.UTF8, true, 4096, true);

            var timeLine = sr.ReadLine();
            if (timeLine == null)
            {
                LastError = new Exception("Missing input data!");
                return null;
            }

            var distanceLine = sr.ReadLine();
            if (distanceLine == null)
            {
                LastError = new Exception("Missing input data!");
                return null;
            }

            var races = new List<BoatRace>();
            var times = Numbers().Match(Whitespace().Replace(timeLine, string.Empty));
            var distances = Numbers().Match(Whitespace().Replace(distanceLine, string.Empty));

            var optionsToWin = FindOptionsToWin(new BoatRace() {Id = 1, RaceTime = long.Parse(times.Groups["num"].Value), DistanceToBeat = long.Parse(distances.Groups["num"].Value)});

            return optionsToWin.ToString();

        }
        catch (Exception ex)
        {
            LastError = ex;
            return null;
        }

    }

    
}