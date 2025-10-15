using System.Collections;
using System.Text;

namespace PuzzleSolverLib.Common;

using MapPoint = (int X, int Y);


public class MapPoints : IEquatable<MapPoints>, IEnumerable<MapPoint>
{
    private readonly List<MapPoint> _mapPoints = new();

    public MapPoint this[int index] => _mapPoints[index];

    public bool IsEmpty => _mapPoints.Count == 0;

    public int Count => _mapPoints.Count;

    public void Add(MapPoint point)
    {
        if (!_mapPoints.Contains(point))
            _mapPoints.Add(point);
    }

    public void AddRange(params MapPoint[] points)
    {
        foreach(var point in points)
            _mapPoints.Add(point);
    }

    public void Remove(MapPoint point) => _mapPoints.Remove(point);
    public void RemoveAll(Predicate<MapPoint> predicate) => _mapPoints.RemoveAll(predicate);

    public void Reverse() => _mapPoints.Reverse();

    public bool Equals(MapPoints? other)
    {
        if (other is null) return false;
        if (_mapPoints.Count != other._mapPoints.Count)
            return false;
        for(var n = 0; n < _mapPoints.Count; n++)
            if (_mapPoints[n].X != other._mapPoints[n].X || _mapPoints[n].Y != other._mapPoints[n].Y)
                return false;

        return true;
    }

    public IEnumerator<MapPoint> GetEnumerator()
    {
        foreach (var point in _mapPoints)
            yield return point;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (obj.GetType() != GetType()) return false;
        return Equals((MapPoints) obj);
    }

    public override int GetHashCode()
    {
        return _mapPoints.GetHashCode();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class MapFunctions
{

    public static bool IsConnectedOnX(MapPoint a, MapPoint b) => a.Y == b.Y && Math.Abs(a.X - b.X) == 1;
    public static bool IsConnectedOnY(MapPoint a, MapPoint b) => a.X == b.X && Math.Abs(a.Y - b.Y) == 1;
    public static bool IsConnected(MapPoint a, MapPoint b) => IsConnectedOnX(a, b) || IsConnectedOnY(a, b);
    public static bool IsConnectedLeft(MapPoint a, MapPoint b) => (a.Y == b.Y) && (a.X - b.X == 1);
    public static bool IsConnectedRight(MapPoint a, MapPoint b) => (a.Y == b.Y) && (a.X - b.X == -1);
    public static bool IsConnectedDown(MapPoint a, MapPoint b) => (a.Y - b.Y == -1) && (a.X == b.X);
    public static bool IsConnectedUp(MapPoint a, MapPoint b) => (a.Y - b.Y == 1) && (a.X == b.X);

    public static MapPoint[] GetDirections() => [(0, -1), (1, 0), (0, 1), (-1, 0)];
    public static MapPoint[] GetDirectionsWithDiagonals() => [(0, -1), (1, 0), (0, 1), (-1, 0), (1, 1), (-1, 1), (1, -1), (-1, -1)];

    public static Direction GetDirection(MapPoint point)
    {
        return point switch
        {
            (0, -1) => Direction.Up,
            (0, 1) => Direction.Down,
            (-1, 0) => Direction.Left,
            (1, 0) => Direction.Right,
            _ => throw new ArgumentOutOfRangeException(nameof(point), point, null)
        };
    }

    public static MapPoint GetDirection(Direction direction)
    {
        return direction switch
        {
            Direction.Up => (0, -1),
            Direction.Down => (0, 1),
            Direction.Left => (-1, 0),
            Direction.Right => (1, 0),
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null),
        };
    }

    public static MapPoint GetDirection(DirectionEx direction)
    {
        return direction switch
        {
            DirectionEx.North => (0, 1),
            DirectionEx.South => (0, -1),
            DirectionEx.West => (-1, 0),
            DirectionEx.East => (1, 0),
            DirectionEx.NorthEast => (1, 1),
            DirectionEx.NorthWest => (-1, 1),
            DirectionEx.SouthEast => (1, -1),
            DirectionEx.SouthWest => (-1, -1),
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null),

        };
    }


    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
    }

    public enum DirectionEx
    {
        North,
        South,
        West,
        East,
        NorthEast,
        NorthWest,
        SouthEast,
        SouthWest
    }



    public static List<List<MapPoint>> FindAllConnectedPoints(List<MapPoint> mapPoints)
    {
        var connectedGroups = new List<List<MapPoint>>();
        foreach (var mapPoint in mapPoints)
        {
            var foundGroup = false;
            var groupsToMerge = new List<List<MapPoint>>();
            foreach (var group in connectedGroups)
                if (group.Any(c => IsConnected(c, mapPoint)))
                {
                    groupsToMerge.Add(group);
                    foundGroup = true;
                }

            if (foundGroup)
            {
                groupsToMerge.ForEach(group => connectedGroups.Remove(group));
                var mergedGroup = new List<MapPoint>();
                groupsToMerge.ForEach(group => mergedGroup.AddRange(group));
                mergedGroup.Add(mapPoint);
                connectedGroups.Add(mergedGroup.Distinct().ToList());
            }
            else
            {
                connectedGroups.Add([mapPoint]);
            }
        }

        return connectedGroups;
    }

    public List<(int x, int y)> GetBoundaryPoints(List<(int x, int y)> points)
    {
        var boundaryPoints = new HashSet<(int x, int y)>();
        foreach (var point in points)
        {
            foreach (var dir in GetDirections())
            {
                var neighbor = (point.x + dir.X, point.y + dir.Y);
                if (!points.Contains(neighbor))
                {
                    boundaryPoints.Add(point);
                    break;
                }
            }
        }

        return boundaryPoints.ToList();
    }



    public static void PrintMapToScreen(int[][] map)
    {
        foreach (var y in map)
        {
            foreach (var x in y)
                Console.Write(x);

            Console.WriteLine();
        }
    }

    public static void PrintMapToScreen(char[][] map)
    {
        foreach (var y in map)
        {
            foreach (var x in y)
                Console.Write(x);

            Console.WriteLine();
        }
    }

    public static char[][] CreateEmptyMap(int height, int width, char emptySpace = '.')
    {
        var map = new char[height][];
        for (var i = 0; i < height; i++)
        {
            map[i] = new char[width];
            for (var j = 0; j < width; j++)
                map[i][j] = emptySpace;
        }
        return map;
    }

    public static char[][] CreateMap(IList<string> strings, Action<char, MapPoint>? onParsedChar = null)
    {
        var map = new char[strings.Count][];
        for (var y = 0; y < strings.Count; y++)
        {
            var chars = strings[y].ToCharArray();
            if (onParsedChar != null)
            {
                for (var x = 0; x < chars.Length; x++)
                    onParsedChar(chars[x], (x,y));

            }
            map[y] = chars;
        }

        return map;
    }

    public static char[][] CreateMap(IEnumerable<MapPoint> points, char mapChar = 'X', char mapEmptySpace = '.')
    {
        var list = points.ToList();
        var minY = list.Min(p => p.Y);
        var maxY = list.Max(p => p.Y);
        var minX = list.Min(p => p.X);
        var maxX = list.Max(p => p.X);

        return CreateAndPopulateMap(maxY - minY + 1, maxX - minX + 1, list, minX * -1, minY * -1, mapChar, mapEmptySpace);

    }

    public static char[][] CreateAndPopulateMap(int height, int width, IEnumerable<MapPoint> points, int offsetX = 0, int offsetY = 0, char mapChar = 'X', char mapEmptySpace = '.')
    {
        var map = new char[height][];
        for (var i = 0; i < height; i++)
        {
            map[i] = new char[width];
            for (var j = 0; j < width; j++)
                map[i][j] = mapEmptySpace;
        }

        foreach (var point in points)
            map[point.Y+offsetY][point.X+offsetX] = mapChar;

        return map;
    }

    public static char[][] RotateMap(char[][] map)
    {
        var rotatedMap = new char[map.Select(y=>y.Length).Max()][];
        for(var x = 0; x < rotatedMap.Length; x++)
            rotatedMap[x] = new char[map.Length];


        for (var y = 0; y < map.Length; y++)
        {
            for (var x = 0; x < map[0].Length; x++)
                rotatedMap[x][y] = map[y][x];
        }

        return rotatedMap;
    }


    public static string MapToString(char[][] map)
    {
        var sb = new StringBuilder();
        foreach (var y in map)
        {
            foreach (var x in y)
                sb.Append(x);
            sb.AppendLine();
        }
        return sb.ToString();
    }

    public static string[] MapToStringArray(char[][] map)
    {
        return MapToString(map).Split('\n');
    }



    public static void PrintMapToScreen(char[][] map, List<(int x, int y)> points)
    {
        foreach (var (x, y) in points)
            map[y][x] = 'X';
        PrintMapToScreen(map);
    }

    public static void WriteMapToFile(char[][] map, List<(int x, int y)> points, string path)
    {
        foreach (var (x, y) in points)
            map[y][x] = 'X';

        WriteMapToFile(map, path);
    }

    public static void WriteMapToFile(int[][] map, string path)
    {
        using var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read);
        using var sw = new StreamWriter(fs);

        foreach (var y in map)
        {
            foreach (var x in y)
                sw.Write(x);

            sw.WriteLine();
        }
    }

    public static void WriteMapToFile(char[][] map, string path)
    {
        using var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read);
        using var sw = new StreamWriter(fs);

        foreach (var y in map)
        {
            foreach (var x in y)
                sw.Write(x);

            sw.WriteLine();
        }
    }

    public static void ClearMap(char[][] map, char emptySpace = '.')
    {
        for(var y = 0; y < map.Length; y++)
            for(var x = 0; x < map[y].Length; x++)
                map[y][x] = emptySpace;
    }

    public static void ClearMap(char[][] map, char emptySpace, params char[] charsToClear)
    {
        for(var y = 0; y < map.Length; y++)
        for(var x = 0; x < map[y].Length; x++)
            if (charsToClear.Any(ch=>ch.Equals(map[y][x])))
                map[y][x] = emptySpace;
    }


}