using System.Xml.Schema;
using PuzzleSolverLib.Common;

namespace PuzzleSolverLib.Puzzles.Y2024;
using MapPoint = (int x, int y);

public class Day16b : PuzzleBaseClass
{
    public List<(int cost, List<(MapPoint mapPoint, int direction)>)> FindAllTheCheapestPaths(char[][] map, MapPoint start, int startDirection, MapPoint end)
    {
        int[] dx = { -1, 1, 0, 0 }; // (-1, 0) (1, 0) (0, -1) (0, 1)  // left, right, up, down
        int[] dy = { 0, 0, -1, 1 };
        int straightCost = 1;
        int turnCost = 1001; // 1000 for turn + 1 for step

        var allTheCheapPaths = new List<(int cost, List<(MapPoint mapPoint, int direction)>)>();

        const int numberOfDirections = 4;
        var rows = map.Length;
        var columns = map[0].Length;
        var cost = new int[rows, columns, numberOfDirections];
        var prev = new List<(MapPoint point, int direction)>[rows, columns, numberOfDirections];

        var queue = new PriorityQueue<(MapPoint point, int direction, int cost), int>();

        for (var y = 0; y < rows; y++)
            for (var x = 0; x < columns; x++)
                for (var d = 0; d < numberOfDirections; d++)
                {
                    cost[y, x, d] = int.MaxValue;
                    prev[y, x, d] = new List<(MapPoint point, int direction)>();
                }

        cost[start.y, start.x, startDirection] = 0;
        queue.Enqueue((start, startDirection, 0), 0);

        while (queue.TryDequeue(out var element, out var priority))
        {
            if (element.point.x == end.x && element.point.y == end.y)
            {
                var paths = new List<List<(MapPoint mapPoint, int direction)>>();
                var stack = new Stack<(MapPoint point, int direction, List<(MapPoint mapPoint, int direction)> path)>();
                stack.Push((element.point, element.direction, new List<(MapPoint mapPoint, int direction)>()));

                while (stack.Count > 0)
                {
                    var (currentPoint, currentDirection, currentPath) = stack.Pop();
                    currentPath.Add((currentPoint, currentDirection));

                    if (currentPoint.x == start.x && currentPoint.y == start.y)
                    {
                        currentPath.Reverse();
                        paths.Add(currentPath);
                        continue;
                    }

                    foreach (var prevElement in prev[currentPoint.y, currentPoint.x, currentDirection].Distinct())
                    {
                        var newPath = new List<(MapPoint mapPoint, int direction)>(currentPath);
                        stack.Push((prevElement.point, prevElement.direction, newPath));
                    }
                }

                foreach (var path in paths)
                {
                    allTheCheapPaths.Add((element.cost, path));
                }

                continue;
            }

            if (element.cost > cost[element.point.y, element.point.x, element.direction])
                continue;

            for (var newDirection = 0; newDirection < numberOfDirections; newDirection++)
            {
                var newX = element.point.x + dx[newDirection];
                var newY = element.point.y + dy[newDirection];
                var extraCost = element.direction != newDirection ? turnCost : straightCost;
                if (newX < 0 || newY < 0 || newX >= columns || newY >= rows || map[newY][newX] == '#')
                    continue;
                var newCost = element.cost + extraCost;
                if (newCost <= cost[newY, newX, newDirection])
                {
                    if (newCost < cost[newY, newX, newDirection])
                    {
                        prev[newY, newX, newDirection].Clear();
                    }
                    cost[newY, newX, newDirection] = newCost;
                    prev[newY, newX, newDirection].Add((element.point, element.direction));
                    queue.Enqueue(((newX, newY), newDirection, newCost), newCost);
                }
            }
        }

        return allTheCheapPaths;
    }
    

    public override string? OnSolve(ReadOnlySpan<char> inputFile)
    {
        MapPoint startPosition = (0, 0);
        MapPoint endPosition = (0, 0);
        var lines = File.ReadAllLines(inputFile.ToString());
        Map = MapFunctions.CreateMap(lines, (c, mp) =>
        {
            if (c == 'S')
                startPosition = mp;

            if (c == 'E')
                endPosition = mp;
        });
        Map[startPosition.y][startPosition.x] = '.';



        var allThePaths = FindAllTheCheapestPaths(Map, startPosition, 1,  endPosition);

        var cheapestPaths = allThePaths.Where(x => x.cost == allThePaths.Min(c => c.cost)).ToList();
        foreach(var cheapPath in cheapestPaths.SelectMany(c=>c.Item2).Select(c=>c.mapPoint))
            Map[cheapPath.y][cheapPath.x] = 'O';

        MapFunctions.PrintMapToScreen(Map);

        return cheapestPaths.SelectMany(c=>c.Item2).Select(c=>c.mapPoint).Distinct().Count().ToString();
    }

    public char[][] Map { get; set; }
}

