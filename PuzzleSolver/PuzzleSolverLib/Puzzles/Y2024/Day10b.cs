namespace PuzzleSolverLib.Puzzles.Y2024;

public class Day10b : PuzzleBaseClass
{
    private int[][] _topographicMap;

    
    public enum Direction
    {
        None,
        Up,
        Down,
        Left,
        Right,
        Done
    }

    public Direction[] GetAvailableDirectionsToMove(int x, int y)
    {
        var currentNodeValue = _topographicMap[y][x];
        if (currentNodeValue == 9)
            return [Direction.Done];

        var nextAcceptableValue = currentNodeValue + 1;

        var directions = new List<Direction>();
        if (y > 0 && _topographicMap[y - 1][x] == nextAcceptableValue)
            directions.Add(Direction.Up);
        if (y < _topographicMap.Length - 1 && _topographicMap[y + 1][x] == nextAcceptableValue)
            directions.Add(Direction.Down);
        if (x > 0 && _topographicMap[y][x - 1] == nextAcceptableValue)
            directions.Add(Direction.Left);
        if (x < _topographicMap[y].Length - 1 && _topographicMap[y][x + 1] == nextAcceptableValue)
            directions.Add(Direction.Right);


        return directions.Any() ? directions.ToArray() : [Direction.None];
    }

    public HashSet<List<(int x, int y)>> VerifiedTrails = new();
    public List<(int x, int y, int score, int rating)> TrailScore = new();

    public void TrailBlazer(int x, int y)
    {
        
        var forkedPaths = new Queue<List<(int x, int y)>>();
        var pathConclusion = new HashSet<(int x, int y)>();
        var rating = 0;

        // Get all possible start directions for trailhead
        foreach (var direction in GetAvailableDirectionsToMove(x, y))
        {
            switch (direction)
            {
                case Direction.Up:
                    forkedPaths.Enqueue(new List<(int x, int y)>([(x, y), (x, y - 1)]));
                    break;
                case Direction.Down:
                    forkedPaths.Enqueue(new List<(int x, int y)>([(x, y), (x, y + 1)]));
                    break;
                case Direction.Left:
                    forkedPaths.Enqueue(new List<(int x, int y)>([(x, y), (x - 1, y)]));
                    break;
                case Direction.Right:
                    forkedPaths.Enqueue(new List<(int x, int y)>([(x, y), (x + 1, y)]));
                    break;
            }
        }

        // Walk the trails
        while (true)
        {
            if (!forkedPaths.TryDequeue(out var path))
                break;

            var currentPoint = path.Last();
            foreach (var direction in GetAvailableDirectionsToMove(currentPoint.x, currentPoint.y))
            {
                switch (direction)
                {
                    case Direction.Up:
                        var newForkUp = new List<(int x, int y)>(path.ToArray()) {(currentPoint.x, currentPoint.y - 1)};
                        forkedPaths.Enqueue(newForkUp);
                        break;
                    case Direction.Down:
                        var newForkDown = new List<(int x, int y)>(path.ToArray()) {(currentPoint.x, currentPoint.y + 1)};
                        forkedPaths.Enqueue(newForkDown);
                        break;
                    case Direction.Left:
                        var newForkLeft = new List<(int x, int y)>(path.ToArray()) {(currentPoint.x - 1, currentPoint.y)};
                        forkedPaths.Enqueue(newForkLeft);
                        break;
                    case Direction.Right:
                        var newForkRight = new List<(int x, int y)>(path.ToArray()) {(currentPoint.x + 1, currentPoint.y)};
                        forkedPaths.Enqueue(newForkRight);
                        break;
                    case Direction.Done:
                        VerifiedTrails.Add(path);
                        rating++;
                        pathConclusion.Add((currentPoint.x, currentPoint.y));
                        break;
                }
            }
        }

        TrailScore.Add((x, y, pathConclusion.Count, rating));

    }



    public override string? OnSolve(ReadOnlySpan<char> inputFile)
    {
        var lines = File.ReadAllLines(inputFile.ToString()).Where(x=>!string.IsNullOrWhiteSpace(x)).ToList();
        _topographicMap = new int[lines.Count][];
        var y = 0;
        foreach (var line in lines)
            _topographicMap[y++] = line.ToCharArray().Select(x => (int) (x - '0')).ToArray();
        var trailHeaders = new List<(int x, int y, int score)>();
        for(y = 0; y < _topographicMap.Length; y++)
            for (var x = 0; x < _topographicMap[y].Length; x++)
                if (_topographicMap[y][x] == 0)
                    trailHeaders.Add((x,y, -1));
      
        foreach (var trailHead in trailHeaders)
        {
            TrailBlazer(trailHead.x, trailHead.y);


        }


        return TrailScore.Sum(x => x.rating).ToString();

    }
}