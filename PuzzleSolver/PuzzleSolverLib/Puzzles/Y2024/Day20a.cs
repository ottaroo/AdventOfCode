using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Xml.Schema;
using PuzzleSolverLib.Common;

namespace PuzzleSolverLib.Puzzles.Y2024;
using MapPoint = (int x, int y);
public class Day20a : PuzzleBaseClass
{
    public char[][] Map { get; set; }

    public List<MapPoint> FindShortestPath(char[][] map, MapPoint start, MapPoint end, MapPoint? cheat)
    {
        int[] dx = [-1, 1, 0, 0]; // left, right, up, down
        int[] dy = [0, 0, -1, 1];

        const int numberOfDirections = 4;
        var rows = map.Length;
        var columns = map[0].Length;
        var prev = new (MapPoint mapPoint, int direction)?[rows, columns];

        var queue = new Queue<(MapPoint mapPoint, int direction)>();
        var visited = new bool[rows, columns];

        queue.Enqueue((start, 0));
        visited[start.y, start.x] = true;

        while (queue.TryDequeue(out var current))
        {

            if (current.mapPoint.x == end.x && current.mapPoint.y == end.y)
            {
                var path = new List<MapPoint>();
                while (current.mapPoint.x != start.x || current.mapPoint.y != start.y)
                {
                    path.Add(current.mapPoint);
                    var prevElement = prev[current.mapPoint.y, current.mapPoint.x]!.Value;
                    current.mapPoint = prevElement.mapPoint;
                }
                path.Add(start);
                path.Reverse();
                return path;
            }

            for (var newDirection = 0; newDirection < numberOfDirections; newDirection++)
            {

                var newX = current.mapPoint.x + dx[newDirection];
                var newY = current.mapPoint.y + dy[newDirection];
                if (newX < 0 || newY < 0 || newX >= columns || newY >= rows || (map[newY][newX] == '#' && (cheat == null || (cheat.Value.x != newX || cheat.Value.y != newY))) || visited[newY, newX])
                    continue;

                visited[newY, newX] = true;
                prev[newY, newX] = (current.mapPoint, newDirection);
                queue.Enqueue(((newX, newY), newDirection));
            }
        }

        return new List<MapPoint>(); // Return an empty list if no path is found
    }

    
    public override string? OnSolve(ReadOnlySpan<char> inputFile)
    {
        var lines = File.ReadAllLines(inputFile.ToString());
        var width = lines.First().Length;
        var height = lines.Length;
        var possibleCheats = new ConcurrentQueue<MapPoint>();
        var results = new List<(MapPoint cheat, int picoSeconds)>();
        var resultsLock = new Lock();

        MapPoint start = (0,0);
        MapPoint end = (0,0);
        var toClear = new List<MapPoint>();
        Map = MapFunctions.CreateMap(lines, (c, mp) =>
        {
            if (c is 'S')
            {
                start = mp;
                toClear.Add(mp);
            }

            if (c is 'E')
            {
                end = mp;
                toClear.Add(mp);
            }

            if (c is '#')
                possibleCheats.Enqueue(mp);

        });

        foreach(var mp in toClear)
            Map[mp.y][mp.x] = '.';


        var numberOfPossibleCheats = possibleCheats.Count;
        var completedCheats = 0;
        var noCheat = FindShortestPath(Map, start, end, null);
        var noCheatTime = noCheat.Count - 1; // exclude starting point


        var tasks = new List<Task>();
        for (var i = 0; i < 1; i++)
        {
            var task = new Task(() =>
            {
                var localResults = new List<(MapPoint cheat, int picoSeconds)>();

                while (possibleCheats.TryDequeue(out var cheatCode))
                {
                    var r = FindShortestPath(Map, start,end, cheatCode);
                    if (r.Any() && r.Count - 1 < noCheatTime)
                        localResults.Add((cheatCode, r.Count - 1));

                    var c = Interlocked.Increment(ref completedCheats);
                    if (c % 10 == 0)
                        Console.WriteLine($"Completed {c} / {numberOfPossibleCheats}");
                }
                lock (resultsLock)
                {
                    results.AddRange(localResults);
                }
            });
            task.Start();
            tasks.Add(task);
        }

        Task.WaitAll(tasks);

        // Print result for example
        // var results = new List<(int picoseconds, MapPoint cheat, List<MapPoint> path)>();
        //
        //foreach (var cheatCode in possibleCheats)
        //{
        //    var race = TotalNumberOfPicosecondsToCompleteRace(lines, cheatCode);
        //    if (race.picoseconds >= noCheat.picoseconds)
        //        continue;
        //    results.Add((race.picoseconds, cheatCode, race.path));
        //}
        //
        //var picoseconds = results.OrderBy(x=>x.picoseconds).Select(x => x.picoseconds).Distinct().ToList();
        //
        //foreach (var ps in picoseconds)
        //{
        //    Console.WriteLine($"{results.Count(x=>x.picoseconds == ps)} cheats saves {noCheat.picoseconds - ps} picoseconds");
        //}

        return results.Count(x=>noCheatTime - x.picoSeconds >= 100).ToString();
    }
}