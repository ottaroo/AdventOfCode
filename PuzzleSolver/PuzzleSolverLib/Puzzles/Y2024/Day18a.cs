using PuzzleSolverLib.Common;

namespace PuzzleSolverLib.Puzzles.Y2024;
using MapPoint = (int x, int y);

public class Day18a : PuzzleBaseClass
{
    public char[][] Map { get; set; }   

    public override string? OnSolve(ReadOnlySpan<char> inputFile)
    {
        Map = MapFunctions.CreateEmptyMap(71, 71);
        var graph = new Graph<MapPoint>() {IsDirected = false, IsWeighted = true};
        var lines = File.ReadAllLines(inputFile.ToString()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        

        foreach (var line in lines.Take(1024))  // First 1024 bytes
        {
            var pointMatch = RegularExpressions.Coordinates().Match(line);
            var x = int.Parse(pointMatch.Groups["x"].Value);
            var y = int.Parse(pointMatch.Groups["y"].Value);
            Map[y][x] = '#';
        }

        for(var y = 0; y < Map.Length; y++)
            for (var x = 0; x < Map[y].Length; x++)
                if (Map[y][x] != '#')
                {
                    var from = graph.AddNode((x, y));
                    if (x > 0 && Map[y][x - 1] != '#')
                    {
                        var to = graph.Nodes.Find(n => n.Data.Equals((x - 1, y)))!;
                        graph.AddEdge(from, to, 1);
                    }

                    if (y > 0 && Map[y - 1][x] != '#')
                    {
                        var to = graph.Nodes.Find(n => n.Data.Equals((x, y - 1)))!;
                        graph.AddEdge(from, to, 1);
                    }
                }

        var start = graph.Nodes.Find(n => n.Data.Equals((0, 0)))!;
        var end = graph.Nodes.Find(n => n.Data.Equals((70, 70)))!;

        var path = graph.GetShortestPath(start, end);

        return path.Count.ToString();
    }
}