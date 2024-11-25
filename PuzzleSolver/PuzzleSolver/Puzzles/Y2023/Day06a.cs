using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace PuzzleSolver.Puzzles.Y2023;

public partial class Day06a : PuzzleBaseClass
{
    [GeneratedRegex(@"(?<num>\d+)")]
    public partial Regex Numbers();


    public record BoatRace
    {
        public int Id { get; set; } 
        public int RaceTime { get; set; }
        public int DistanceToBeat { get; set; } 
    }

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
            var times = Numbers().Matches(timeLine);
            var distances = Numbers().Matches(distanceLine);
            for(var i = 1; i <= times.Count; i++)
                races.Add(new BoatRace() {Id = i, RaceTime = int.Parse(times[i-1].Groups["num"].Value), DistanceToBeat = int.Parse(distances[i-1].Groups["num"].Value)});

            var optionsToWin = 1;

            foreach (var race in races)
                optionsToWin *= FindOptionsToWin(race);




            return optionsToWin.ToString();

        }
        catch (Exception ex)
        {
            LastError = ex;
            return null;
        }
    }

    public int FindOptionsToWin(BoatRace race)
    {
        var optionsToWin = 0;

        Parallel.For(0, race.RaceTime - 1, ()=>new List<int>(), (n,s,l) =>
        {
            if ((race.RaceTime - n) * n > race.DistanceToBeat)
                l.Add(n);
            return l;
        }, intValues =>
        {
            Interlocked.Add(ref optionsToWin, intValues.Count);
        } );


        return optionsToWin;
    }


    public override string Description => @"Too long, didn't read... see http://adventToCode.com/2023/day/6";
}