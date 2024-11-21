using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Linq;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Attributes;

namespace Day02a
{


    public partial class Program
    {
        [GeneratedRegex(@"((?<n>\d+)\s*(?<cube>red|green|blue))", RegexOptions.IgnoreCase)]
        private static partial Regex Cubes();

        private static string numbers = "0123456789";

        static void Main(string[] args)
        {
            //var summary = BenchmarkRunner.Run<Program>();
            ExecuteMain();
        }

        [Benchmark]
        public static void ExecuteMain()
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

            var redCubes = 12;
            var blueCubes = 14;
            var greenCubes = 13;
            var sumOfIds = 0;
            var defaultColor = Console.ForegroundColor;

            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                if (line == null) continue;
                var data = line.AsSpan();
                (var id, var index) = getId(data);

                data = data[index..];

                foreach (var set in data.Split(';'))
                {
                    var s = data[set.Start..set.End];

                    foreach (Match match in Cubes().Matches(data[set.Start..set.End].ToString()))
                    {
                        if (!match.Success) continue;

                        switch (match.Groups["cube"].Value.Trim().ToLower())
                        {
                            case "red":
                                if (int.Parse(match.Groups["n"].Value) > redCubes)
                                    goto DO_NOT_INCLUDE_IN_SUM;
                                break;
                            case "blue":
                                if (int.Parse(match.Groups["n"].Value) > blueCubes)
                                    goto DO_NOT_INCLUDE_IN_SUM;
                                break;
                            case "green":
                                if (int.Parse(match.Groups["n"].Value) > greenCubes)
                                    goto DO_NOT_INCLUDE_IN_SUM;
                                break;
                            default:
                                throw new Exception("Invalid color");
                        }
                    }
                }

                sumOfIds += id;

                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write("INCLUDE IN SUM: ");
                Console.ForegroundColor = defaultColor;
                Console.WriteLine(line);
                continue;

            DO_NOT_INCLUDE_IN_SUM:;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("EXCLUDE IN SUM: ");
                Console.ForegroundColor = defaultColor;
                Console.WriteLine(line);
            }

            Console.WriteLine($"Sum of IDs: {sumOfIds}");
        }
    }
}
