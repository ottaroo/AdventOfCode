using System.Drawing;
using System.Text.RegularExpressions;
using PuzzleSolverLib.Common;

namespace PuzzleSolverLib.Puzzles.Y2024;
using MapPoint = (int X, int Y);

public  class Day14b : PuzzleBaseClass
{
    public class Robot(char[][] map, MapPoint startPosition, MapPoint velocity, int id)
    {
        public MapPoint Velocity { get; } = velocity;

        public MapPoint StartPosition { get; } = startPosition;

        public MapPoint CurrentPosition { get; set; } = startPosition;
        public int Id { get; } = id;

        public void Move()
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

    }


    public override string? OnSolve(ReadOnlySpan<char> inputFile)
    {

        var robots = new List<Robot>();
        var map = MapFunctions.CreateEmptyMap(103, 101, ' ');

        var lines = File.ReadAllLines(inputFile.ToString()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        for (var i = 0; i < lines.Count; i++)
        {
            var matches = RegularExpressions.Coordinates().Matches(lines[i]);
            var startPosition = (int.Parse(matches[0].Groups["x"].Value), int.Parse(matches[0].Groups["y"].Value));
            var velocity = (int.Parse(matches[1].Groups["x"].Value), int.Parse(matches[1].Groups["y"].Value));

            robots.Add(new Robot(map, startPosition, velocity, i));
        }

        // NOTE: First attempts was to elaborate - tried to compare number of robots in left/right side
        // made sure all was connected etc... apparently all you had to was to search for # ### #####
        // well my solution was to identify the repeat cycle - draw image of all maps and look through the tumbnails
        // which worked pretty well since it became very obvious which one it was

        for (var i = 0; i < 10402; i++)
        {

            foreach(var robot in robots)
                robot.Move();


            var points = robots.Select(x => x.CurrentPosition).ToList();
            var fileName= $"D:\\temp\\crazyrobots\\seconds_{i+1}.png";
            DrawPointsAndSaveAsPng(points, fileName, map[0].Length, map.Length);


        }



        //var repeats = new ConcurrentDictionary<int, int>();
        //Parallel.ForEach(robots, robot =>
        //{
        //    for (var i = 0; i < 1000000; i++)
        //    {
        //        robot.Move();
        //        if (i > 0)
        //        {
        //            if (robot.CurrentPosition.X == robot.StartPosition.X && robot.CurrentPosition.Y == robot.StartPosition.Y)
        //            {
        //                 repeats.AddOrUpdate(robot.Id, i, (key, value) => value + 1);
        //                break;
        //            }
        //        }
        //    }
        //});

        //using var fs = new FileStream(@"D:\temp\crazyrobots_day14.txt", FileMode.Create, FileAccess.Write, FileShare.Read);
        //using var sw = new StreamWriter(fs);

        //sw.WriteLine("Robot(ID)\tRepeat Cycle");
        //foreach (var key in repeats.Keys)
        //{
        //    sw.WriteLine($"{key}\t{repeats[key]}");
        //}






        return "look in the pictures folder!";
    }

    public static void DrawPointsAndSaveAsPng(List<MapPoint> points, string filePath, int width, int height)
    {
        using var bitmap = new Bitmap(width, height);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.Clear(Color.White);

        foreach (var point in points)
        {
            bitmap.SetPixel(point.X, point.Y, Color.Green);
        }

        bitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
    }
}