using PuzzleSolverLib.Common;

namespace PuzzleSolverLib.Puzzles.Y2024;
using MapPoint = (int x, int y);

public class Day16a : PuzzleBaseClass
{
    public (int cost, List<(MapPoint mapPoint, int direction)> path) FindCheapestPath(char[][] map, MapPoint start, int startDirection, MapPoint end)
    {
        int[] dx = [-1, 1, 0, 0]; // (-1, 0) (1, 0) (0, -1) (0, 1)  // left, right, up, down
        int[] dy = [0, 0, -1, 1];
        int straightCost = 1;
        int turnCost = 1001; // 1000 for turn + 1 for step

        const int numberOfDirections = 4;
        var rows = map.Length;
        var columns = map[0].Length;
        var cost = new int[rows, columns, numberOfDirections];
        var prev = new (MapPoint point, int direction)[rows, columns, numberOfDirections];

        var queue = new PriorityQueue<(MapPoint point, int direction, int cost), int>();

        for(var y = 0; y < rows; y++)
            for(var x = 0; x < columns; x++)
            for (var d = 0; d < numberOfDirections; d++)
                cost[y, x, d] = int.MaxValue;

        cost[start.y, start.x, startDirection] = 0;
        queue.Enqueue((start, startDirection, 0), 0);


        while (queue.TryDequeue(out var element, out var priority))
        {
            if (element.point.x == end.x && element.point.y == end.y)
            {
                var path = new List<(MapPoint mapPoint, int direction)>();
                while (element.point.x != start.x || element.point.y != start.y)
                {
                    path.Add((element.point, element.direction));
                    (MapPoint point, int direction) prevElement = prev[element.point.y, element.point.x, element.direction];
                    element.point.x = prevElement.point.x;
                    element.point.y = prevElement.point.y;
                    element.direction = prevElement.direction;
                }
                path.Add((start, startDirection));
                path.Reverse();
                return (element.cost, path);
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
                if (newCost < cost[newY, newX, newDirection])
                {
                    cost[newY, newX, newDirection] = newCost;
                    prev[newY, newX, newDirection] = (element.point, element.direction);
                    queue.Enqueue(((newX, newY), newDirection, newCost), newCost);
                }
            }

        }


        return (-1, []);
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



        var cheapestPath = FindCheapestPath(Map, startPosition, 1,  endPosition);

        MapFunctions.ClearMap(Map, '.', 'X');
        foreach (var position in cheapestPath.path)
            Map[position.mapPoint.y][position.mapPoint.x] = position.direction switch
            {
                0 => '<',
                1 => '>',
                2 => '^',
                3 => 'v'
            };

        MapFunctions.PrintMapToScreen(Map);
        return cheapestPath.cost.ToString();
    }

    public char[][] Map { get; set; }
}

