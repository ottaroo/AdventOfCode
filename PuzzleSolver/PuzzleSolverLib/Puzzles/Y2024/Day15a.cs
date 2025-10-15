using System.Drawing.Interop;
using System.Text;
using PuzzleSolverLib.Common;

namespace PuzzleSolverLib.Puzzles.Y2024;

using MapPoint = (int x, int y);
using Direction = (int x, int y);

public class Day15a : PuzzleBaseClass
{
    public class Box
    {
        public Box(MapPoint startPosition)
        {
            StartPosition = startPosition;
            Position = startPosition;
        }

        public MapPoint StartPosition { get;  }
        public MapPoint Position { get; set; }

        public int Gps()
        {
            return Position.x + Position.y * 100;
        }

        public bool BoxIsLockedIntoPosition { get; set; }

    }

    public class Robot
    {
        private readonly List<Box> _boxReference;


        public Robot(char[][] mapReference, List<Box> boxReference, MapPoint startPosition)
        {
            _boxReference = boxReference;
            MapReference = mapReference;
            StartPosition = startPosition;
            CurrentPosition = startPosition;
        }

        public MapPoint StartPosition { get; }
        public MapPoint CurrentPosition { get; set; }
        public char[][] MapReference { get; }



        
        public bool Move(Direction direction)
        {
            if (_boxReference.All(x => x.BoxIsLockedIntoPosition))
            {
                Console.WriteLine("ALL BOXES LOCKED!");
                return false;
            }

            MapPoint nextPosition = (CurrentPosition.x + direction.x, CurrentPosition.y + direction.y);
            if (NextPositionIsAWall(nextPosition))
                return true;

            if (NextPositionIsABox(nextPosition) && !MoveBox(nextPosition, direction))
                return true;

            MapReference[CurrentPosition.y][CurrentPosition.x] = '.';
            MapReference[nextPosition.y][nextPosition.x] = '@';

            CurrentPosition = nextPosition;
            return true;

        }

        private bool NextPositionIsAWall(MapPoint nextPosition) => MapReference[nextPosition.y][nextPosition.x] == '#' || (MapReference[nextPosition.y][nextPosition.x] == 'O' && _boxReference.First(x=>x.Position.x == nextPosition.x && x.Position.y == nextPosition.y).BoxIsLockedIntoPosition);
        private bool NextPositionIsABox(MapPoint nextPosition) => MapReference[nextPosition.y][nextPosition.x] == 'O' && !_boxReference.First(x=>x.Position.x == nextPosition.x && x.Position.y == nextPosition.y).BoxIsLockedIntoPosition;
        private bool NextPositionIsEmptySpace(MapPoint nextPosition) => MapReference[nextPosition.y][nextPosition.x] == '.' ||MapReference[nextPosition.y][nextPosition.x] == '@';


        private bool IsBoxInLockedPositionCheckDirection(MapPoint boxPosition, Direction direction)
        {
            return NextPositionIsEmptySpace((boxPosition.x + direction.x, boxPosition.y + direction.y)) || NextPositionIsABox((boxPosition.x + direction.x, boxPosition.y + direction.y));
        }
        private bool IsBoxInALockedPosition(MapPoint boxPosition)
        {
            
            var dir = MapFunctions.GetDirection(MapFunctions.Direction.Left);
            var canGoLeft = IsBoxInLockedPositionCheckDirection(boxPosition, dir);
            dir = MapFunctions.GetDirection(MapFunctions.Direction.Right);
            var canGoRight =IsBoxInLockedPositionCheckDirection(boxPosition, dir);
            dir = MapFunctions.GetDirection(MapFunctions.Direction.Up);
            var canGoUp = IsBoxInLockedPositionCheckDirection(boxPosition, dir);
            dir = MapFunctions.GetDirection(MapFunctions.Direction.Down);
            var canGoDown = IsBoxInLockedPositionCheckDirection(boxPosition, dir);

            if (canGoLeft && canGoRight)
                return false;
            if (canGoUp && canGoDown)
                return false;

            return true;
        }

        private bool MoveBox(MapPoint boxPosition, Direction direction)
        {
            var boxPositions = new Stack<MapPoint>();
            boxPositions.Push(boxPosition);
            var nextBoxPosition = boxPosition;

            while (true)
            {
                nextBoxPosition = (nextBoxPosition.x + direction.x, nextBoxPosition.y + direction.y);

                if (NextPositionIsABox(nextBoxPosition))
                {
                    boxPositions.Push(nextBoxPosition);
                    continue;
                }

                if (NextPositionIsEmptySpace(nextBoxPosition))
                {
                    var backTrackPosition = nextBoxPosition;
                    while (boxPositions.TryPop(out var mapPoint))
                    {
                        // Update box position
                        var box = _boxReference.First(x => x.Position.Equals(mapPoint));

                        MapReference[backTrackPosition.y][backTrackPosition.x] = 'O';
                        MapReference[mapPoint.y][mapPoint.x] = '.';
                        box.Position = backTrackPosition;
                        backTrackPosition = mapPoint;
                    }
                    return true;
                }

                if (NextPositionIsAWall(nextBoxPosition))
                {
                    while (boxPositions.TryPop(out var mapPoint))
                    {
                        var box = _boxReference.First(x => x.Position.x == mapPoint.x && x.Position.y == mapPoint.y);
                        if (IsBoxInALockedPosition(mapPoint))
                            box.BoxIsLockedIntoPosition = true;
                    }

                    return false;

                }

            }

        }

    }

    public Direction GetNextDirection(char direction)
    {
        return direction switch
        {
            '^' => (0, -1),
            'v' => (0, 1),
            '>' => (1, 0),
            '<' => (-1, 0),
            _ => throw new InvalidOperationException($"Invalid direction: {direction}")
        };
    }
    public override string? OnSolve(ReadOnlySpan<char> inputFile)
    {
        var lines = File.ReadAllLines(inputFile.ToString()).Where(l => !string.IsNullOrWhiteSpace(l)).ToList();
        var mapList = new List<string>();
        foreach (var line in lines)
        {
            mapList.Add(line);
            if (mapList.Count > 1 && line.Count(x => x.Equals('#')) > 10)
                break;
        }

        var listOfBoxes = new List<Box>();

        var sb = new StringBuilder();
        foreach (var line in lines.Skip(mapList.Count)) 
            sb.Append(line);
        var movement = sb.ToString();
        var robotStartPosition = (0, 0);
        var map = MapFunctions.CreateMap(mapList, (c, mp) =>
        {
            if (c == '@')
                robotStartPosition = mp;
            if (c == 'O')
            {
                listOfBoxes.Add(new Box(mp));
            }
        });


        var robot = new Robot(map, listOfBoxes, robotStartPosition);
        var count = 0;
        foreach (var dir in movement)
        {
            robot.Move(GetNextDirection(dir));

        }
        MapFunctions.PrintMapToScreen(map);

        return listOfBoxes.Sum(x=>x.Gps()).ToString();
    }
}