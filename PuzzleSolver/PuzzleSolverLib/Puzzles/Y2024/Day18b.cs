using PuzzleSolverLib.Common;

namespace PuzzleSolverLib.Puzzles.Y2024;
using MapPoint = (int x, int y);

public class Day18b : PuzzleBaseClass
{
    public char[][] Map { get; set; }


    public bool HasSolution(int n, string[] lines)
    {
        Map = MapFunctions.CreateEmptyMap(71, 71);
        var graph = new Graph<MapPoint>() {IsDirected = false, IsWeighted = true};
        foreach (var line in lines.Take(n))  
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

        try
        {
            var path = graph.GetShortestPath(start, end);
            return path.Any();
        }
        catch
        {
            return false;
        }
    }

    public override string? OnSolve(ReadOnlySpan<char> inputFile)
    {
        var lines = File.ReadAllLines(inputFile.ToString()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

        var n = 1024;
        var nMax = lines.Length - 1;
        var l = 0;
        while (true)
        {
            if (nMax - n == 1)
            {
                if (HasSolution(n, lines))
                    l = n;
                else
                    l = nMax;
                break;
            }

            var t = ((nMax - n) / 2) + n;



            if (HasSolution(t, lines))
            {
                n = t;
            }
            else
            {
                nMax = t;
            }
        }




        return lines[l].ToString();
    }
}