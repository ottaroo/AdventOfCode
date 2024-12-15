using PuzzleSolverLib.Common;

namespace PuzzleSolverLib.Puzzles.Y2024;


public class Day12a : PuzzleBaseClass
{

    public class Region
    {
        public char SeedType { get; set; }
        public List<(int x, int y)> LandPlots { get; set; } = new();

        public int GetFencePrice(char[][] mapReference)
        {
            return GetRequiredFenceSize(mapReference) * LandPlots.Count;
        }

        public virtual int GetRequiredFenceSize(char[][] mapReference)
        {
            // Find all plots that are on the edge of the region
            var edgePlots = 0;
            foreach (var plot in LandPlots)
            {
                edgePlots += plot.x == 0 || mapReference[plot.y][plot.x - 1] != SeedType ? 1 : 0;
                edgePlots += plot.x == mapReference[plot.y].Length - 1 || mapReference[plot.y][plot.x + 1] != SeedType ? 1 : 0;
                edgePlots += plot.y == 0 || mapReference[plot.y - 1][plot.x] != SeedType ? 1 : 0;
                edgePlots += plot.y == mapReference.Length - 1 || mapReference[plot.y + 1][plot.x] != SeedType ? 1 : 0;
            }

            return edgePlots;
        }
    }



    private char[][] _map;

    public override string? OnSolve(ReadOnlySpan<char> inputFile)
    {
        var seeds = new HashSet<(char seed, int x, int y)>();
        var txt = File.ReadAllLines(inputFile.ToString()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

        _map = MapFunctions.CreateMap(txt, (c, mapPoint) => { seeds.Add((c, mapPoint.X, mapPoint.Y));});


        var listOfRegions = new List<Region>();
        var seedTypes = seeds.Select(x => x.seed).Distinct().ToList();

        foreach (var seedType in seedTypes)
        {
            var seedList = seeds.Where(s => s.seed.Equals(seedType)).Select(s => (s.x, s.y)).ToList();
            var connected = MapFunctions.FindAllConnectedPoints(seedList);
            foreach (var group in connected)
            {
                var region = new Region {LandPlots = group, SeedType = seedType};
                listOfRegions.Add(region);
            }
        }


        return listOfRegions.Sum(x=>x.GetFencePrice(_map)).ToString();
    }
}