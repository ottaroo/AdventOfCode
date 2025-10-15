using System.Text;
using PuzzleSolverLib.Common;

namespace PuzzleSolverLib.Puzzles.Y2024;

using MapPoint = (int x, int y);
using Direction = (int x, int y);
using ObjectSize = (int width, int height);

public class Day15b : PuzzleBaseClass
{
    //TODO: Get back to this - not correct yet (works with both examples though)

    public abstract class MapObject
    {
        private readonly bool _isMovableObject;
        private bool _isLocked;

        protected MapObject(MapPoint position, ObjectSize size, bool isMovableObject)
        {
            _isMovableObject = isMovableObject;
            _isLocked = false;
            Position = position;
            StartPosition = position;
            Size = size;
        }
        public MapPoint StartPosition { get; set; }

        // Top left + size.Width, Top left + size.Height
        public MapPoint Position { get; set; }
        public ObjectSize Size { get; }

        public bool IsMovable { get; set; }

        public bool LockPosition()
        {
            if (!_isLocked)
            {
                _isLocked = true;
                return true;
            }

            return false;
        }

        public bool UnlockPosition()
        {
            if (_isLocked)
            {
                _isLocked = false;
                return true;
            }

            return false;
        }

        public bool IsHitTest(MapPoint point) => point.x >= Position.x && point.x < Position.x + Size.width && point.y >= Position.y && point.y < Position.y + Size.height;

        public bool Move(Direction direction)
        {
            if (!_isMovableObject)
                return false;
            Position = (Position.x + direction.x, Position.y + direction.y);
            return true;
        }

    }

    public class FreeSpace : MapObject
    {
        public FreeSpace(MapPoint position) : base(position, (1,1), true)
        {
        }
    }

    public class Wall : MapObject
    {
        public Wall(MapPoint position) : base(position, (1, 1), false)
        {
        }
    }

    public class Box : MapObject
    {
        public Box(MapPoint position) : base(position, (2, 1), true)
        {
        }

        public int Gps()
        {
            var minX = Position.x;
            var minY = Position.y; 

            return minX + (minY * 100);
        }

        public List<MapObject> GetObjects(List<MapObject> mapObjects, Direction direction)
        {
            var positionsToTest = new List<MapPoint>();

            switch (direction)
            {
                case (0,1):
                    positionsToTest.Add((Position.x, Position.y + 1));
                    positionsToTest.Add((Position.x + 1, Position.y + 1));
                    break;
                case (0, -1):
                    positionsToTest.Add((Position.x, Position.y - 1));
                    positionsToTest.Add((Position.x + 1, Position.y - 1));
                    break;
                case (1, 0):
                    positionsToTest.Add((Position.x + Size.width, Position.y));
                    break;
                case (-1, 0):
                    positionsToTest.Add((Position.x - 1, Position.y));
                    break;
            }

            var objects = new List<MapObject>();

            foreach (var positionToTest in positionsToTest)
            {
                var obj = mapObjects.FirstOrDefault(x => x.IsHitTest(positionToTest)) ?? new FreeSpace(positionToTest);
                objects.Add(obj);
            }


            if (objects.All(x=>x is FreeSpace))
                return objects;

            if (objects.Any(x=>x is Wall))
            {
                return objects.Where(x=>x is Wall).Take(1).ToList();
            }

            var boxes = new List<MapObject>();
            foreach (var box in objects)
            {
                if (boxes.All(x=>!object.ReferenceEquals(x, box)))
                    boxes.Add(box);
            }

            return boxes.Where(x=>x is Box).ToList();
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

    private MapObject[] ParseNewMapLine(string line, int y, Action<MapPoint> startPosition)
    {
        var mapObjects = new List<MapObject>();
        var index = 0;
        foreach (var ch in line)
        {
            switch (ch)
            {
                case '#':
                    mapObjects.Add(new Wall((index++, y)));
                    mapObjects.Add(new Wall((index++, y)));
                    break;
                case '.':
                    index++;
                    index++;
                    break;
                case 'O':
                    mapObjects.Add(new Box((index++, y)));
                    index++;
                    break;
                case '@':
                    startPosition.Invoke((index++, y));
                    index++;
                    break;
            }
        }

        return mapObjects.ToArray();
    }

    public override string? OnSolve(ReadOnlySpan<char> inputFile)
    {
        var lines = File.ReadAllLines(inputFile.ToString()).Where(l => !string.IsNullOrWhiteSpace(l)).ToList();
        var mapList = new List<string>();
        foreach (var line in lines)
        {
            mapList.Add(line);
            if (mapList.Count > 1 && line.Trim().All(x => x.Equals('#')))
                break;
        }

        //var listOfBoxes = new List<Box>();

        var sb = new StringBuilder();
        foreach (var line in lines.Skip(mapList.Count))
            sb.Append(line);
        var movement = sb.ToString();
        var robotStartPosition = (0, 0);
        var mapObjects = new List<MapObject>();
        for (var y = 0; y < mapList.Count; y++)
            mapObjects.AddRange(ParseNewMapLine(mapList[y], y, mapPoint => { robotStartPosition = mapPoint; }));


        var drawMap = new Action<MapPoint>((mp) =>
        {
            var map = MapFunctions.CreateEmptyMap(mapList.Count, mapList[0].Length * 2);
            foreach (var mapObject in mapObjects)
            {
                for (var x = 0; x < mapObject.Size.width; x++)
                {
                    for (var y = 0; y < mapObject.Size.height; y++)
                    {
                        switch (mapObject)
                        {
                            case Wall w:
                                map[mapObject.Position.y + y][mapObject.Position.x + x] = '#';
                                break;
                            case Box b:
                                map[mapObject.Position.y + y][mapObject.Position.x + x] = x == 0 ? '[' : ']';
                                break;
                        }
                    }
                }
            }

            map[mp.y][mp.x] = '@';

            MapFunctions.PrintMapToScreen(map);


        });




        MapPoint currentPosition = robotStartPosition;
        var movementIndex = 0;
        while(true)
        {
            if (movementIndex >= movement.Length)
                break;

            var dir = movement[movementIndex++];

            var nextDirection = GetNextDirection(dir);
            MapPoint nextPosition = (currentPosition.x + nextDirection.x, currentPosition.y + nextDirection.y);

            var mapObject = mapObjects.FirstOrDefault(x => x.IsHitTest(nextPosition));

            if (mapObject == null)
            {
                currentPosition = nextPosition;
                continue;
            }

            if (mapObject is Wall)
                continue;

            if (mapObject is Box b)
            {
                var objectsToMove = new Stack<MapObject>();
                var objectsToCheck = new Queue<MapObject>();
                objectsToCheck.Enqueue(b);

                while (objectsToCheck.TryDequeue(out var obj))
                {
                    if (obj is Wall)
                        goto NEXT;
                    if (obj is Box box)
                    {
                        foreach (var o in box.GetObjects(mapObjects, nextDirection))
                            objectsToCheck.Enqueue(o);

                        objectsToMove.Push(box);
                    }

                }

                while(objectsToMove.TryPop(out var obj))
                    obj.Move(nextDirection);


                currentPosition = nextPosition;
            }

            NEXT: ;

        }


        drawMap.Invoke(currentPosition);
        
        // Gives correct answer for both samples
        // but
        // 1553003 - to high

        return mapObjects.Where(x=>x is Box).Cast<Box>().Sum(x => x.Gps()).ToString();
    }
}