using System.Transactions;
using System.Xml.Schema;
using PuzzleSolverLib.Puzzles;
using PuzzleSolverLib.Puzzles.Y2024;

namespace PuzzleSolverLib.Puzzles.Y2024;

public class Point(int x, int y)
{
    public int X { get; set; } = x;
    public int Y { get; set; } = y;

    public Direction Direction { get; set; } = Direction.Unchanged;
}

public class Triangle(Point a, Point b, Point c)
{
    public Point A { get; } = a;
    public Point B { get; } = b;
    public Point C { get; } = c;
}

public class Square(Point topLeft, Point topRight, Point bottomLeft, Point bottomRight, int order)
{
    public Square() : this(new Point(0, 0), new Point(0, 0), new Point(0, 0), new Point(0, 0), 0)
    {
        
    }
    public Point TopLeft { get; set; } = topLeft;
    public Point TopRight { get; set; } = topRight;
    public Point BottomLeft { get;set; } = bottomLeft;
    public Point BottomRight { get; set; } = bottomRight;

    public int Order { get; set; } = order;

}


public class Day06b : PuzzleBaseClass
{
    private char[][]? _map;
    private HashSet<Point> _blockedPaths = new();


    private HashSet<Point> _canCreateALoopPoint = new();
    private HashSet<Triangle> _triangles = new();

    private List<Square> _squares = new();


    private static int DeterminateValue(Point a, Point b, Point c) => (a.X * (b.Y - c.Y) - a.Y * (b.X - c.X) + 1 * (b.X * c.Y - b.Y * c.X));

    private void FindAllTriangles()
    {
        var blockedPaths = _blockedPaths.ToArray();

        for (var i = 0; i < _blockedPaths.Count; i++)
            for (var j = i + 1; j < _blockedPaths.Count; j++)
                for (var k = j + 1; k < _blockedPaths.Count; k++)
                    if (DeterminateValue(blockedPaths[i], blockedPaths[j], blockedPaths[k])>=0)
                    {
                        _triangles.Add(new Triangle(blockedPaths[i], blockedPaths[j], blockedPaths[k]));
                    }
    }




    public override string? OnSolve(ReadOnlySpan<char> inputFile)
    {
        var startPosition = new Point(0, 0);
        var map = File.ReadAllLines(inputFile.ToString());


        _map = new char[map.Length][];

        for (var y = 0; y < map.Length; y++)
        {
            _map[y] = new char[map[y].Length];
            for (var x = 0; x < map[y].Length; x++)
            {
                _map[y][x] = map[y][x];
                if (map[y][x] == '^' || map[y][x] == '<' || map[y][x] == '>' || map[y][x] == 'v')
                {
                    startPosition.X = x;
                    startPosition.Y = y;
                }
                if (map[y][x] == '#')
                {
                    _blockedPaths.Add(new Point(x, y));
                }
            }
        }

        //FindAllTriangles();

        //FindPotentialBlocks();

//        FindLoopPoints();


        

        // Maybe revisit...
        // Missing the intersect for crossing squares where you can loop it into two squares

        var tmp = WalkTheMaze(startPosition, GetDirection(startPosition)).ToString();

        foreach(var potential in _canCreateALoopPoint)
            _map[potential.Y][potential.X] = 'O';


        for (var n = 0; n < _map.Length; n++)
        {
            for (var x = 0; x < _map[n].Length; x++)
                Console.Write(_map[n][x]);

            Console.WriteLine();
        }


        return tmp;
    }

    private Direction GetDirection(Point p)
    {
        if (_map == null) throw new ArgumentException("Map has not been initialized");

        if (p.Y < 0 || p.Y > _map.Length - 1)
            return Direction.Done;
        if (p.X < 0 || p.X > _map[p.Y].Length -1)
            return Direction.Done;

        return _map[p.Y][p.X] switch
        {
            '^' => Direction.Up,
            '<' => Direction.Left,
            '>' => Direction.Right,
            'v' => Direction.Down,
            '#' => Direction.Blocked,
            _ => Direction.Unchanged
        };
    }

    public void FindLoopPoints()
    {
        foreach (var path in _blockedPaths)
        {
            //  #
            //  +.....+#
            //  .
            // #+
            //       x
            var bottomLeft = path;
            var topLeft = _blockedPaths.OrderByDescending(b=>b.Y).FirstOrDefault(b => b.X == bottomLeft.X + 1 && b.Y < bottomLeft.Y);
            if (topLeft == null)
                continue;
            var topRight = _blockedPaths.OrderBy(b => b.X).FirstOrDefault(b => b.X > bottomLeft.X && b.Y == topLeft.Y + 1);
            if (topRight == null)
                continue;
            _canCreateALoopPoint.Add(new Point(topRight.X - 1, bottomLeft.Y + 1) {Direction = Direction.Down});
        }

        foreach (var path in _blockedPaths)
        {
            //  #
            //  +.....+#
            //        .
            // x      +
            //       #
            var topLeft = path;
            var topRight = _blockedPaths.OrderBy(b=>b.X).FirstOrDefault(b => b.X > topLeft.X && b.Y == topLeft.Y + 1);
            if (topRight == null)
                continue;
            var bottomRight = _blockedPaths.OrderBy(b => b.Y).FirstOrDefault(b => b.X == topRight.X - 1 && b.Y > topRight.Y);
            if (bottomRight == null)
                continue;
            _canCreateALoopPoint.Add(new Point(topLeft.X - 1, bottomRight.Y - 1) {Direction = Direction.Left});
        }

        foreach (var path in _blockedPaths)
        {
            //  x
            //        +#
            //        .
            // #+.....+
            //        #
            var topRight = path;
            var bottomRight = _blockedPaths.OrderBy(b=>b.Y).FirstOrDefault(b => b.X == topRight.X - 1 && b.Y > topRight.Y);
            if (bottomRight == null)
                continue;
            var bottomLeft = _blockedPaths.OrderByDescending(b => b.X).FirstOrDefault(b => b.X < bottomRight.X && b.Y == bottomRight.Y - 1);
            if (bottomLeft== null)
                continue;
            _canCreateALoopPoint.Add(new Point(bottomLeft.X + 1, topRight.Y - 1) {Direction = Direction.Up});
        }

        foreach (var path in _blockedPaths)
        {
            //  #
            //  '       x
            //  '       
            // #+.....+
            //        #
            var bottomRight = path;
            var bottomLeft = _blockedPaths.OrderByDescending(b => b.X).FirstOrDefault(b => b.X < bottomRight.X && b.Y == bottomRight.Y - 1);
            if (bottomLeft== null)
                continue;
            var topLeft = _blockedPaths.OrderByDescending(b => b.Y).FirstOrDefault(b => b.X == bottomLeft.X+1 && b.Y < bottomRight.Y);
            if (topLeft== null)
                continue;
            _canCreateALoopPoint.Add(new Point(bottomRight.X + 1, topLeft.Y + 1) {Direction = Direction.Right});
        }

    }




    private bool IsALoopWalk(Point start, Direction direction)
    {
        if (_map == null) throw new ArgumentException("Map is not initialized");
        var visitedPositions = new HashSet<Point>();
        var currentPosition = start;
        var currentDirection = direction;
        while (true)
        {
            var nextPosition = currentPosition;
            switch (currentDirection)
            {
                case Direction.Up:
                    nextPosition.Y -= 1;
                    break;
                case Direction.Down:
                    nextPosition.Y += 1;
                    break;
                case Direction.Left:
                    nextPosition.X -= 1;
                    break;
                case Direction.Right:
                    nextPosition.X += 1;
                    break;
            }
            if (nextPosition.Y < 0 || nextPosition.Y > _map.Length - 1 || nextPosition.X < 0 || nextPosition.X > _map[0].Length - 1)
            {
                return false;
            }
            var dir = GetDirection(nextPosition);
            if (dir == Direction.Blocked)
            {
                currentDirection = currentDirection switch
                {
                    Direction.Up => Direction.Right,
                    Direction.Right => Direction.Down,
                    Direction.Down => Direction.Left,
                    Direction.Left => Direction.Up,
                    _ => throw new InvalidOperationException("Invalid direction")
                };
                continue;
            }
            if (visitedPositions.Contains(nextPosition))
                return true;
            visitedPositions.Add(nextPosition);
            currentPosition = nextPosition;
        }


    }

  


    public int WalkTheMaze(Point startPosition, Direction direction)
    {
        if (_map == null) throw new ArgumentException("Map is not initialized");

        var visitedPositions = new HashSet<Point>();
        var currentPosition = startPosition;
        var currentDirection = direction;
        var blockOptions = 0;
        var switchedDirection = 0;
        var square = new Square();
        var nextPosition = new Point(0, 0);

        while (true)
        {
            nextPosition.X = currentPosition.X;
            nextPosition.Y = currentPosition.Y;
            switch (currentDirection)
            {
                case Direction.Up:
                    nextPosition.Y -= 1;
                    break;
                case Direction.Down:
                    nextPosition.Y += 1;
                    break;
                case Direction.Left:
                    nextPosition.X -= 1;
                    break;
                case Direction.Right:
                    nextPosition.X += 1;
                    break;
            }



            var dir = GetDirection(nextPosition);
            if (dir == Direction.Blocked)
            {
                switch (currentDirection)
                {
                    case Direction.Right:
                        square.TopRight.X = currentPosition.X;
                        square.TopRight.Y = currentPosition.Y;
                        break;
                    case Direction.Down:
                        square.BottomRight.X = currentPosition.X;
                        square.BottomRight.Y = currentPosition.Y;
                        break;
                    case Direction.Left:
                        square.BottomLeft.X = currentPosition.X;
                        square.BottomLeft.Y = currentPosition.Y;
                        break;
                    case Direction.Up:
                        square.TopLeft.X = currentPosition.X;
                        square.TopLeft.Y = currentPosition.Y;
                        break;
                }
                

                currentDirection = currentDirection switch
                {
                    Direction.Up => Direction.Right,
                    Direction.Right => Direction.Down,
                    Direction.Down => Direction.Left,
                    Direction.Left => Direction.Up,
                    _ => throw new InvalidOperationException("Invalid direction")
                };

                if (switchedDirection > 0 && switchedDirection % 4 == 0)
                {
                    _squares.Add(square);
                    square.Order = _squares.Count;
                    square = new Square();

                }
                switchedDirection++;

                continue;
            }

            if (nextPosition.Y < 0 || nextPosition.Y > _map.Length - 1 || nextPosition.X < 0 || nextPosition.X > _map[0].Length - 1)
            {
                return blockOptions;
                //return visitedPositions.Count;
            }

            if (visitedPositions.Contains(nextPosition))
            {
                Point loopPoint = new Point(nextPosition.X, nextPosition.Y);
                // could we loop the guard here
                switch (currentDirection)
                {
                    case Direction.Right:
                        loopPoint.X++;
                        if (loopPoint.X < _map[loopPoint.Y].Length && !_blockedPaths.Contains(loopPoint))
                            _canCreateALoopPoint.Add(loopPoint);
                        break;
                    case Direction.Down:
                        loopPoint.Y++;
                        if (loopPoint.Y < _map.Length && !_blockedPaths.Contains(loopPoint))
                            _canCreateALoopPoint.Add(loopPoint);
                        break;
                    case Direction.Left:
                        loopPoint.X--;
                        if (loopPoint.X >= 0 && !_blockedPaths.Contains(loopPoint))
                            _canCreateALoopPoint.Add(loopPoint);
                        break;
                    case Direction.Up:
                        loopPoint.Y--;
                        if (loopPoint.Y >= 0 && !_blockedPaths.Contains(loopPoint))
                            _canCreateALoopPoint.Add(loopPoint);
                        break;
                }

            }

            if (_canCreateALoopPoint.Contains(currentPosition))
                blockOptions++;

            visitedPositions.Add(nextPosition);
            currentPosition.X = nextPosition.X;
            currentPosition.Y = nextPosition.Y;
        }
    }
}

