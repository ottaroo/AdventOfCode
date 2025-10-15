using System.Collections.Concurrent;
using System.Xml.Schema;
using PuzzleSolverLib.Common;

namespace PuzzleSolverLib.Puzzles.Y2024;
using MapPoint = (int x, int y);
public class Day20b : PuzzleBaseClass
{
    //TODO: Get back to this - not correct yet

    public char[][] Map { get; set; }
    public char[][] EmptyMap { get; set; }
    public const int AcceptedCheatTime = 21;

    // Cheat with 20 pico seconds can any point from center up to 20 picoseconds away in any direction
    // but all points must be wall points except for the last which absolutely cannot be a wall point
    public List<(MapPoint start, MapPoint end)> GetPossibleCheatPaths(char[][] map, MapPoint start)
    {
        var list = new List<(MapPoint start, MapPoint end)>();

        //   ## ##
        //  ### ###
        // #### ####
        var n = AcceptedCheatTime - 1;
        for (var y = start.y - n; y < start.y; y++)
        {
            if (y < 0 || y >= map.Length)
                continue;

            for (var x = start.x - (AcceptedCheatTime-1) + n; x < start.x; x++)
            {
                if (x < 0 || x >= map[0].Length)
                    continue;
                list.Add((start, (x, y)));
            }

            for (var x = start.x + (AcceptedCheatTime - 1) - n; x > start.x ; x--)
            {
                if (x < 0 || x >= map[0].Length)
                    continue;
                list.Add((start, (x, y)));

            }
            n--;
        }


        // #### ####
        //  ### ###
        //   ## ##
        n = AcceptedCheatTime - 1;
        for(var y = start.y + n; y > start.y; y--)
        {
            if (y < 0 || y >= map.Length)
                continue;
            for (var x = start.x - (AcceptedCheatTime - 1) + n; x < start.x; x++)
            {
                if (x < 0 || x >= map[0].Length)
                    continue;
                list.Add((start, (x, y)));
            }
            for (var x = start.x + (AcceptedCheatTime - 1) - n; x > start.x; x--)
            {
                if (x < 0 || x >= map[0].Length)
                    continue;
                list.Add((start, (x, y)));
            }
            n--;
        }

        //    #
        //  #####
        //    #
        for (var y = start.y - AcceptedCheatTime; y < start.y + AcceptedCheatTime; y++)
            list.Add((start, (start.x, y)));
        for (var x = start.x - AcceptedCheatTime; x < start.y + AcceptedCheatTime; x++)
            list.Add((start, (x, start.y)));







        return list;
    }

    public List<(MapPoint start, MapPoint end)> GetAllPossibleCheatPaths(char[][] map, List<MapPoint> startPointForCheat)
    {
        var cheatPaths = new HashSet<(MapPoint start, MapPoint end)>(); // cheat paths with same start and same end is the same cheat regardless of route

        foreach (var point in startPointForCheat)
        {
            // Get possible cheat paths
            foreach (var possible in GetPossibleCheatPaths(map, point))
                cheatPaths.Add(possible);
        }

        return cheatPaths.ToList();
    }

    public List<List<MapPoint>> CreateMapPoints(char[][] map, MapPoint start, MapPoint end)
    {
        var allPaths = FindAllPaths(EmptyMap, start, end);

        var validCheatPaths = new List<List<MapPoint>>();

        // A valid cheat path starts on '#' and continues on '#' until it reaches the end point '.'
        //foreach (var path in allPaths)
        //{
        //    var test = path.TakeWhile(mp => map[mp.y][mp.x] == '#').ToList();
        //    if (test.Count == path.Count - 1 && map[path.Last().y][path.Last().x] == '.')
        //        validCheatPaths.Add(path);

        //}

        // uhm maybe the walls don't have to be consecutive.. only needs to end on an empty space
        foreach (var path in allPaths)
        {
            if (map[path.Last().y][path.Last().x] == '.')
                validCheatPaths.Add(path);
        }


        return validCheatPaths;
    }

    public List<MapPoint> FindShortestPathsUsingCheatsUpTo20Picoseconds(char[][] map, MapPoint start, MapPoint end, List<MapPoint> cheat)
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
                if (newX < 0 || newY < 0 || newX >= columns || newY >= rows || (map[newY][newX] == '#' && !cheat.Any(c=>c.Equals((newX, newY)))) || visited[newY, newX])
                    continue;

                visited[newY, newX] = true;
                prev[newY, newX] = (current.mapPoint, newDirection);
                queue.Enqueue(((newX, newY), newDirection));
            }
        }

        return new List<MapPoint>(); // Return an empty list if no path is found
    }




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

    public List<List<MapPoint>> FindAllPaths(char[][] map, MapPoint start, MapPoint end)
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
        var allPaths = new List<List<MapPoint>>();

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
                allPaths.Add(path);
                continue;
            }

            for (var newDirection = 0; newDirection < numberOfDirections; newDirection++)
            {

                var newX = current.mapPoint.x + dx[newDirection];
                var newY = current.mapPoint.y + dy[newDirection];
                if (newX < 0 || newY < 0 || newX >= columns || newY >= rows || map[newY][newX] == '#' || visited[newY, newX])
                    continue;

                visited[newY, newX] = true;
                prev[newY, newX] = (current.mapPoint, newDirection);
                queue.Enqueue(((newX, newY), newDirection));
            }
        }

        return allPaths;
    }
    
    public override string? OnSolve(ReadOnlySpan<char> inputFile)
    {
        var lines = File.ReadAllLines(inputFile.ToString());
        var results = new List<(MapPoint startCheat, MapPoint endCheat, int picoSeconds)>();
        var resultsLock = new Lock();

        MapPoint start = (0,0);
        MapPoint end = (0,0);
        var toClear = new List<MapPoint>();
        var cheatStartingPoints = new List<MapPoint>();
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
                cheatStartingPoints.Add(mp);
        });
        EmptyMap = MapFunctions.CreateEmptyMap(Map.Length, Map[0].Length);

        foreach (var mp in toClear)
            Map[mp.y][mp.x] = '.';


        var completedCheats = 0;
        var noCheat = FindShortestPath(Map, start, end, null);
        var noCheatTime = noCheat.Count - 1; // exclude starting point
        var possibleCheats = new ConcurrentQueue<List<MapPoint>>();

        var allPossibleCheatPaths  = GetAllPossibleCheatPaths(Map, cheatStartingPoints);
        foreach(var possibleCheatPath in allPossibleCheatPaths) 
            foreach(var list in CreateMapPoints(Map, possibleCheatPath.start, possibleCheatPath.end))
                possibleCheats.Enqueue(list);
        var numberOfPossibleCheats = possibleCheats.Count;


        var tasks = new List<Task>();
        for (var i = 0; i < 1; i++)
        {
            var task = new Task(() =>
            {
                var localResults = new List<(MapPoint startCheat, MapPoint endCheat, int picoSeconds)>();

                while (possibleCheats.TryDequeue(out var cheatCode))
                {
                    var r = FindShortestPathsUsingCheatsUpTo20Picoseconds(Map, start,end, cheatCode);
                    if (r.Any() && r.Count - 1 < noCheatTime)
                        localResults.Add(((cheatCode.First().x, cheatCode.First().y), (cheatCode.Last().x, cheatCode.Last().y), r.Count - 1));

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

        // Get rid of duplicates
        var cleanResult = results.Where(x=>noCheatTime -  x.picoSeconds >= 50).Distinct().ToList();

        foreach (var ps in cleanResult.Select(x => x.picoSeconds).Distinct().OrderByDescending(x => x))
        {
            Console.WriteLine($"There are {cleanResult.Count(x => x.picoSeconds == ps)} cheats which save {noCheatTime - ps} picoseconds");
        }

        return cleanResult.Count(x=>noCheatTime - x.picoSeconds >= 50).ToString();
    }
}