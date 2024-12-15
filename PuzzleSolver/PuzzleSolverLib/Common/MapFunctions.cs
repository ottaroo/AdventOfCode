using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PuzzleSolverLib.Common;

using MapPoint = (int X, int Y);


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
}