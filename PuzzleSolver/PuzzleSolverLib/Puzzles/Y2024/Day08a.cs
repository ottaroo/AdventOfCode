using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.JavaScript;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;

namespace PuzzleSolverLib.Puzzles.Y2024;

public record Location(int X, int Y);

public class Antenna : IEquatable<Antenna>
{
    public int Id { get; set; }
    public char Symbol { get; set; }
    public Location Location { get; set; }

    public bool Equals(Antenna? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Location.Equals(other.Location);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Antenna) obj);
    }

    public override int GetHashCode()
    {
        return Location.GetHashCode();
    }
}

public class FrequencyPair
{
    public int GridSize { get; }

    public FrequencyPair(int gridSize)
    {
        GridSize = gridSize;
    }
    public Antenna A { get; set; }
    public Antenna B { get; set; }

    public virtual Antenna[] GetAntiFrequencies(Func<char> symbol = null)
    {
        var antiFrequencyA = new Antenna() {Symbol = symbol?.Invoke() ?? '#' };
        var antiFrequencyB = new Antenna() {Symbol = symbol?.Invoke() ?? '#'};

        var (left, right) = (A.Location.X < B.Location.X) ? (A, B) : (B, A);
        var (top, bottom) = (A.Location.Y < B.Location.Y) ? (A, B) : (B, A);

        var distance = right.Location.X - left.Location.X;
        var height = bottom.Location.Y - top.Location.Y;

        if (ReferenceEquals(left, top))
        {
            antiFrequencyA.Location = new Location(left.Location.X - distance, left.Location.Y - height);
            antiFrequencyB.Location = new Location(right.Location.X + distance, right.Location.Y + height);
        }
        else
        {
            antiFrequencyA.Location = new Location(left.Location.X - distance, left.Location.Y + height);
            antiFrequencyB.Location = new Location(right.Location.X + distance, right.Location.Y - height);

        }

        var list = new List<Antenna>();
        if (antiFrequencyA.Location.X >= 0 && antiFrequencyA.Location.X < GridSize && antiFrequencyA.Location.Y >= 0 && antiFrequencyA.Location.Y < GridSize)
            list.Add(antiFrequencyA);
        if (antiFrequencyB.Location.X >= 0 && antiFrequencyB.Location.X < GridSize && antiFrequencyB.Location.Y >= 0 && antiFrequencyB.Location.Y < GridSize)
            list.Add(antiFrequencyB);

        return list.ToArray();
    }

}


public class Day08a : PuzzleBaseClass
{
    public override string? OnSolve(ReadOnlySpan<char> inputFile)
    {
        var antennaList = new List<Antenna>();
        var frequencyPairs = new List<FrequencyPair>();
        var gridSize = -1;

        using var fs = new FileStream(inputFile.ToString(), FileMode.Open, FileAccess.Read, FileShare.Read);
        using var sr = new StreamReader(fs);

        var y = 0;
        while (!sr.EndOfStream)
        {
            var line = sr.ReadLine();
            if (string.IsNullOrWhiteSpace(line)) continue;
            if (gridSize < 0)
                gridSize = line.Length;

            for (var x = 0; x < line.Length; x++)
            {
                var ch = line[x];

                if (line[x] != '.')
                {
                    var id = antennaList.Count(x => x.Symbol.Equals(ch)) + 1;
                    antennaList.Add(new Antenna
                    {
                        Id = id,
                        Symbol = ch,
                        Location = new Location(x, y)
                    });
                }
            }

            y++;
        }

        foreach (var ch in antennaList.Select(x => x.Symbol).Distinct())
        {
            var max = antennaList.Where(x => x.Symbol == ch).Max(x => x.Id);
            for (var i = 1; i <= max; i++)
            {
                var antennaA = antennaList.FirstOrDefault(x => x.Symbol == ch && x.Id == i);
                for (var j = i + 1; j <= max; j++)
                {
                    var antennaB = antennaList.FirstOrDefault(x => x.Symbol == ch && x.Id == j);
                    if (antennaA != null && antennaB != null && !ReferenceEquals(antennaA, antennaB))
                    {
                        frequencyPairs.Add(new FrequencyPair(gridSize)
                        {
                            A = antennaA,
                            B = antennaB
                        });
                    }
                }
            }
        }

        //var anti = frequencyPairs.SelectMany(x => x.GetAntiFrequencies(GetId)).ToList();
        //using var fss = new FileStream("d:\\temp\\frequencymap.txt", FileMode.Create, FileAccess.Write);
        //using var sw = new StreamWriter(fss);
        //for (var yn = 0; yn < 12; yn++)
        //{
        //    var sb = new StringBuilder();
        //    for (var x = 0; x < 12; x++)
        //    {
        //        var antenna = antennaList.FirstOrDefault(a => a.Location.X == x && a.Location.Y == yn);
        //        var af = anti.FirstOrDefault(a => a.Location.X == x && a.Location.Y == yn);
        //        if (af != null)
        //        {
        //            sb.Append(af.Symbol);
        //        }
        //        //else
        //        //if (antenna != null)
        //        //{
        //        //    sb.Append(antenna.Symbol);
        //        //}
        //        else
        //        {
        //            sb.Append('.');
        //        }



        //    }
        //    sw.WriteLine(sb.ToString());
        //}

        //sw.WriteLine();

        //foreach (var pair in frequencyPairs.OrderBy(x => x.A.Symbol).ThenBy(f => Math.Min(f.A.Id, f.B.Id)))
        //{
        //    sw.WriteLine($"{pair.A.Symbol} [{pair.A.Id} <=> {pair.B.Id}] ({pair.A.Location.X},{pair.A.Location.Y})x({pair.B.Location.X},{pair.B.Location.Y})");
        //}

        //sw.WriteLine();
        //foreach (var a in anti)
        //{
        //    sw.WriteLine($"{a.Symbol} ({a.Location.X},{a.Location.Y})");
        //}





        return frequencyPairs.SelectMany(x => x.GetAntiFrequencies()).Distinct().Count().ToString();

    }

}