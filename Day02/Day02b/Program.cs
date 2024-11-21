using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Linq;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Attributes;

namespace Day02a
{


    [MemoryDiagnoser]
    public partial class Program
    {
        [GeneratedRegex(@"((?<n>\d+)\s*(?<cube>red|green|blue))", RegexOptions.IgnoreCase)]
        private static partial Regex Cubes();

        private static string numbers = "0123456789";

        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Program>();
            //ExecuteMain();
        }

        [Benchmark]
        public void ExecuteMain()
        {
            using var fs = new FileStream(Path.Combine(AppContext.BaseDirectory, "input.txt"), FileMode.Open, FileAccess.Read);
            using var sr = new StreamReader(fs);

            var getId = new Func<ReadOnlySpan<char>, Tuple<int, int>>(source =>
            {
                var idStart = source.IndexOfAny(numbers.AsSpan());
                int idEnd = source[idStart..].IndexOf<char>(':') + idStart;

                var id = int.Parse(source[idStart..idEnd]);

                return new Tuple<int, int>(id, idEnd);
            });

            var sumOfCubePower = 0;
            var defaultColor = Console.ForegroundColor;

            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                if (line == null) continue;
                var data = line.AsSpan();
                (var id, var index) = getId(data);


                var redCubes = 0;
                var blueCubes = 0;
                var greenCubes = 0;

                data = data[index..];

                foreach (var set in data.Split(';'))
                {
                    var s = data[set.Start..set.End];

                    foreach (Match match in Cubes().Matches(data[set.Start..set.End].ToString()))
                    {
                        if (!match.Success) continue;

                        var value = int.Parse(match.Groups["n"].Value);

                        switch (match.Groups["cube"].Value.Trim().ToLower())
                        {
                            case "red":
                                if (value > redCubes)
                                    redCubes = value;
                                break;
                            case "blue":
                                if (value > blueCubes)
                                    blueCubes = value;
                                break;
                            case "green":
                                if (value > greenCubes)
                                    greenCubes = value;
                                break;
                            default:
                                throw new Exception("Invalid color");
                        }
                    }
                }



                sumOfCubePower += (redCubes * blueCubes * greenCubes);

                Console.ForegroundColor = defaultColor;
                Console.Write($"ID: {id}, POWER [");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($" {redCubes} RED, ");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write($"{greenCubes} GREEN, ");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write($"{blueCubes} BLUE ");
                Console.ForegroundColor = defaultColor;
                Console.WriteLine($"] {line}");

            }

            Console.WriteLine($"Power: {sumOfCubePower}");

        }
    }
}