using PuzzleSolverLib.Common;

namespace PuzzleSolverLib.Puzzles.Y2024;
using MapPoint = (int X, int Y);

public  class Day14a : PuzzleBaseClass
{
    public class Robot(char[][] map, MapPoint startPosition, MapPoint velocity, int id)
    {
        public MapPoint Velocity { get; } = velocity;

        public MapPoint StartPosition { get; } = startPosition;

        public MapPoint CurrentPosition { get; set; } = startPosition;
        public int Id { get; } = id;

        public void Move()
        {
            try
            {
                MapPoint nextPosition = (CurrentPosition.X + Velocity.X, CurrentPosition.Y + Velocity.Y);
                if (nextPosition.X < 0)
                {
                    nextPosition.X = map[0].Length + nextPosition.X;
                }

                if (nextPosition.X >= map[0].Length)
                {
                     nextPosition.X = Math.Abs(Velocity.X) - (map[0].Length - CurrentPosition.X);
                }

                if (nextPosition.Y < 0)
                {
                    nextPosition.Y = map.Length + nextPosition.Y;
                }

                if (nextPosition.Y >= map.Length)
                {
                    nextPosition.Y = Math.Abs(Velocity.Y) - (map.Length - CurrentPosition.Y);
                }

                if (nextPosition.X < 0 || nextPosition.X >= map[0].Length || nextPosition.Y < 0 || nextPosition.Y >= map.Length)
                    return;

                CurrentPosition = nextPosition;
            }
            catch
            {
                int p = 0;
            }
        }

    }

    public static void AddRobotToMap(Robot robot, char[][] map)
    {
        var num = 1;
        var existing = map[robot.CurrentPosition.Y][robot.CurrentPosition.X];
        if (char.IsDigit(existing))
            num = existing - '0' + 1;

        map[robot.CurrentPosition.Y][robot.CurrentPosition.X] = (char)( '0' + num);
    }

    public static int CalculateSafetyFactor(char[][] map)
    {
        var sum1 = 0;
        var sum2 = 0;
        var sum3 = 0;
        var sum4 = 0;
        try
        {
            var height = map.Length / 2;
            var width = map[0].Length / 2;

            var ignoreX = map[0].Length - (width * 2);
            var ignoreY = map.Length - (height * 2);




            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var quadrant1 = map[y][x];
                    var quadrant2 = map[y][x+ignoreX+width];
                    var quadrant3 = map[y+ignoreY+height][x];
                    var quadrant4 = map[y+ignoreY+height][x+ignoreX+width];

                    if (char.IsDigit(quadrant1))
                        sum1 += quadrant1 - '0';
                    if (char.IsDigit(quadrant2))
                        sum2 += quadrant2 - '0';
                    if (char.IsDigit(quadrant3))
                        sum3 += quadrant3 - '0';
                    if (char.IsDigit(quadrant4))
                        sum4 += quadrant4 - '0';

                }
            }
        }
        catch (Exception ex)
        {
            int p = 0;
        }


        return sum1*sum2*sum3*sum4;

    }


    public override string? OnSolve(ReadOnlySpan<char> inputFile)
    {

        var robots = new List<Robot>();
        var map = MapFunctions.CreateEmptyMap(103, 101);

        var lines = File.ReadAllLines(inputFile.ToString()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        for (var i = 0; i < lines.Count; i++)
        {
            var matches = RegularExpressions.Coordinates().Matches(lines[i]);
            var startPosition = (int.Parse(matches[0].Groups["x"].Value), int.Parse(matches[0].Groups["y"].Value));
            var velocity = (int.Parse(matches[1].Groups["x"].Value), int.Parse(matches[1].Groups["y"].Value));

            robots.Add(new Robot(map, startPosition, velocity, i));
        }

        Parallel.ForEach(robots, robot =>
        {
            for (var n = 0; n < 100; n++)
                robot.Move();
        });


        foreach (var robot in robots)
            AddRobotToMap(robot, map);

        MapFunctions.PrintMapToScreen(map);



        return CalculateSafetyFactor(map).ToString();
    }
}