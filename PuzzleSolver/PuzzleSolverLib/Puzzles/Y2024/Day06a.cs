namespace PuzzleSolverLib.Puzzles.Y2024;
    public class Day06a : PuzzleBaseClass
    {
        public int WalkTheMaze(ReadOnlySpan<char> map)
        {

            var visitedPositions = new HashSet<int>();
        var mapSpan = map;
        var currentPosition = mapSpan.IndexOfAny(['^', '<', '>', 'v']);
        var currentDirection = mapSpan[currentPosition] switch
        {
            '^' => Direction.Up,
            '<' => Direction.Left,
            '>' => Direction.Right,
            'v' => Direction.Down,
            _ => throw new InvalidOperationException("Invalid start direction")
        };
        var mazeWidth = mapSpan.IndexOf('\n') + 1;

        while (true)
        {
            var nextPosition = currentPosition;
            switch (currentDirection)
            {
                case Direction.Up:
                    nextPosition -= mazeWidth;
                    break;
                case Direction.Down:
                    nextPosition += mazeWidth;
                    break;
                case Direction.Left:
                    nextPosition -= 1;
                    break;
                case Direction.Right:
                    nextPosition += 1;
                    break;
            }

            if (nextPosition < 0 || nextPosition >= mapSpan.Length)
            {
                return visitedPositions.Count;
            }

            if (mapSpan[nextPosition] == '#')
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

            visitedPositions.Add(nextPosition);
            currentPosition = nextPosition;
        }


    }


        public override string? OnSolve(ReadOnlySpan<char> inputFile)
        {
            var map = File.ReadAllText(inputFile.ToString());
            var mapSpan = map.AsSpan();


            return WalkTheMaze(mapSpan).ToString();
        }



    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
    }
