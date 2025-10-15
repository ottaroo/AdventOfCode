using PuzzleSolverLib.Common;

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
            '^' => MapFunctions.Direction.Up,
            '<' => MapFunctions.Direction.Left,
            '>' => MapFunctions.Direction.Right,
            'v' => MapFunctions.Direction.Down,
            _ => throw new InvalidOperationException("Invalid start direction")
        };
        var mazeWidth = mapSpan.IndexOf('\n') + 1;

        while (true)
        {
            var nextPosition = currentPosition;
            switch (currentDirection)
            {
                case MapFunctions.Direction.Up:
                    nextPosition -= mazeWidth;
                    break;
                case MapFunctions.Direction.Down:
                    nextPosition += mazeWidth;
                    break;
                case MapFunctions.Direction.Left:
                    nextPosition -= 1;
                    break;
                case MapFunctions.Direction.Right:
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
                    MapFunctions.Direction.Up => MapFunctions.Direction.Right,
                    MapFunctions.Direction.Right => MapFunctions.Direction.Down,
                    MapFunctions.Direction.Down => MapFunctions.Direction.Left,
                    MapFunctions.Direction.Left => MapFunctions.Direction.Up,
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

