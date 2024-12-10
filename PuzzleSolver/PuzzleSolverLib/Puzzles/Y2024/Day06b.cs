using System.Collections.Concurrent;

namespace PuzzleSolverLib.Puzzles.Y2024;


public class Guard(int[][] map, int x, int y, Direction startDirection, Action<Guard, bool>? onMazeCompleted = null, Action<(int x, int y, Direction direction)>? onGuardMove = null)
{
    private readonly HashSet<(int x, int y, Direction direction)> _visited = [];
    private int _totalSteps;
    private bool _mazeIsCompleted = false;
    private int _blockX = -1;
    private int _blockY = -1;


    public IReadOnlyList<(int x, int y, Direction direction)> VisitedPositions => _visited.ToArray();

    public int TotalSteps => _totalSteps;

    public int StartPositionX { get; } = x;

    public int StartPositionY { get; } = y;

    public Direction StartDirection { get; } = startDirection;

    private (int x, int y) GetNextPosition()
    {
        return Direction switch
        {
            Direction.Up => (X, Y - 1),
            Direction.Right => (X + 1, Y),
            Direction.Down => (X, Y + 1),
            Direction.Left => (X - 1, Y),
        };
    }

    private bool IsPositionOutsideMap(int x, int y)
    {
        return (x < 0 || x >= map[0].Length || y < 0 || y >= map.Length);

    }

    private bool IsPositionBlocked(int x, int y)
    {
        return map[y][x] == 1 || ( x == _blockX && y == _blockY);
    }

    

    private void Move()
    {
        var nextPosition = GetNextPosition();
        if (IsPositionOutsideMap(nextPosition.x, nextPosition.y))
        {
            _mazeIsCompleted = true;
            onMazeCompleted?.Invoke(this, _blockX < 0 && _blockY < 0);
            return;
        }

        if (IsPositionBlocked(nextPosition.x, nextPosition.y))
        {
            ChangeDirection();
            return;
        }

        X = nextPosition.x;
        Y = nextPosition.y;

        if (_visited.Contains((X, Y, Direction)))
        {
            _mazeIsCompleted = true;
            onMazeCompleted?.Invoke(this, true);
            return;
        }


        _visited.Add((X, Y, Direction));
        onGuardMove?.Invoke((X,Y, Direction));
        _totalSteps++;
    }

    private void ChangeDirection()
    {
        var newDirection = Direction switch
        {
            Direction.Up => Direction.Right,
            Direction.Right => Direction.Down,
            Direction.Down => Direction.Left,
            Direction.Left => Direction.Up,
            _ => throw new InvalidOperationException("Invalid direction")
        };
        Direction = newDirection;
    }

    public (int totalSteps, int uniquePositions) Start(int blockX, int blockY, Direction blockDirection)
    {
        _blockX = blockX;
        _blockY = blockY;
        BlockDirection = blockDirection;
        
        return Start();
    }

    public (int totalSteps, int uniquePositions) Start()
    {
        while(!_mazeIsCompleted)
            Move();

        return (_totalSteps, _visited.Count);
    }

    public int BlockX => _blockX;
    public int BlockY => _blockY;
    public Direction BlockDirection { get; private set; }

    public int X { get; private set; } = x;
    public int Y { get; private set; } = y;
    public Direction Direction { get; private set; } = startDirection;
}


public class Day06b : PuzzleBaseClass
{
    private ConcurrentQueue<(int blockX, int blockY, Direction blockDirection)> _positionsToCheckForLoop = new();
    private ConcurrentBag<(int x, int y, Direction direction)> _loopPoints = new();
    private static int[][] _map;
    private ManualResetEvent _virtualGuardProducerDone = new(false);

    public override string? OnSolve(ReadOnlySpan<char> inputFile)
    {
        var tmp = File.ReadAllLines(inputFile.ToString());
        _map = new int[tmp.Length][];
        var startPositionX = 0;
        var startPositionY = 0;
        for (var y = 0; y < tmp.Length; y++)
        {
            _map[y] = new int[tmp[y].Length];
            for (var x = 0; x < tmp[y].Length; x++)
            {
                _map[y][x] = tmp[y][x] == '#' ? 1 : 0;

                if (tmp[y][x] == '^')
                {
                    startPositionX = x;
                    startPositionY = y;
                }
            }
        }

        var guard = new Guard(_map, startPositionX, startPositionY, Direction.Up, (guard1, b) =>
        {
            _virtualGuardProducerDone.Set();
        }, OnGuardMove);
        _ = guard.Start();

        var workers = new List<Task>();
        for (var n = 0; n < Environment.ProcessorCount; n++)
        {
            workers.Add(Task.Run(() =>
            {
                while (true)
                {
                    if (!_positionsToCheckForLoop.TryDequeue(out var position))
                    {
                        if (_virtualGuardProducerDone.WaitOne(100))
                            break;

                        continue;
                    }
                    var testTheLoopGuard = new Guard(_map, startPositionX, startPositionY, Direction.Up, OnMazeCompleted, null);
                    testTheLoopGuard.Start(position.blockX, position.blockY, position.blockDirection);
                }
            }));
        }
        
        Task.WaitAll(workers.ToArray());


        var result = _loopPoints.Select(x=>(x.x, x.y)).Distinct().ToList();
        result.RemoveAll(p=> p.x == startPositionX && p.y == startPositionY);
        return result.Count.ToString();
    }

    private void OnGuardMove((int x, int y, Direction direction) position)
    {
        _positionsToCheckForLoop.Enqueue((position.x, position.y, position.direction));
    }

    private void OnMazeCompleted(Guard guard, bool success)
    {
        if (success)
            _loopPoints.Add((guard.BlockX, guard.BlockY, guard.BlockDirection));
    }
}