namespace PuzzleSolverLib.Puzzles.Y2024;

public class FrequencyChaos : FrequencyPair
{
    public FrequencyChaos(int gridSize) : base(gridSize)
    {
    }

    public override Antenna[] GetAntiFrequencies(Func<char> symbol = null)
    {
        var (left, right) = (A.Location.X < B.Location.X) ? (A, B) : (B, A);
        var (top, bottom) = (A.Location.Y < B.Location.Y) ? (A, B) : (B, A);

        var distance = right.Location.X - left.Location.X;
        var height = bottom.Location.Y - top.Location.Y;

        var list = new List<Antenna>();

        int aX = left.Location.X;
        int aY = left.Location.Y;
        int bX = right.Location.X;
        int bY = right.Location.Y;

        if (ReferenceEquals(left, top))
        {
            while (true)
            {
                aX -= distance;
                aY -= height;
                if (aX < 0 || aX >= GridSize || aY < 0 || aY >= GridSize)
                    break;

                var antiFrequencyA = new Antenna() {Symbol = symbol?.Invoke() ?? '#' };
                antiFrequencyA.Location = new Location(aX, aY);
                list.Add(antiFrequencyA);
            }
            while (true)
            {
                bX += distance;
                bY += height;
                if (bX < 0 || bX >= GridSize || bY < 0 || bY >= GridSize)
                    break;

                var antiFrequencyA = new Antenna() {Symbol = symbol?.Invoke() ?? '#' };
                antiFrequencyA.Location = new Location(bX, bY);
                list.Add(antiFrequencyA);
            }
        }
        else
        {
            while (true)
            {
                aX -= distance;
                aY += height;
                if (aX < 0 || aX >= GridSize || aY < 0 || aY >= GridSize)
                    break;

                var antiFrequencyA = new Antenna() {Symbol = symbol?.Invoke() ?? '#' };
                antiFrequencyA.Location = new Location(aX, aY);
                list.Add(antiFrequencyA);
            }
            while (true)
            {
                bX += distance;
                bY -= height;
                if (bX < 0 || bX >= GridSize || bY < 0 || bY >= GridSize)
                    break;

                var antiFrequencyA = new Antenna() {Symbol = symbol?.Invoke() ?? '#' };
                antiFrequencyA.Location = new Location(bX, bY);
                list.Add(antiFrequencyA);
            }

        }

        list.Add(new Antenna(){Symbol = symbol?.Invoke() ?? '#',  Location = new Location(A.Location.X, A.Location.Y)});
        list.Add(new Antenna(){Symbol = symbol?.Invoke() ?? '#',  Location = new Location(B.Location.X, B.Location.Y)});

        return list.ToArray();

    }
}



public class Day08b : PuzzleBaseClass
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
                        frequencyPairs.Add(new FrequencyChaos(gridSize)
                        {
                            A = antennaA,
                            B = antennaB
                        });
                    }
                }
            }
        }





        return frequencyPairs.SelectMany(x => x.GetAntiFrequencies()).Distinct().Count().ToString();

    }

}