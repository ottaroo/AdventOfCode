using System.Text.RegularExpressions;

namespace PuzzleSolverLib.Puzzles.Y2023
{


    public partial class Day02b : PuzzleBaseClass
    {
        [GeneratedRegex(@"((?<n>\d+)\s*(?<cube>red|green|blue))", RegexOptions.IgnoreCase)]
        private static partial Regex Cubes();

        private static string numbers = "0123456789";

        public override string Description => @"As you continue your walk, the Elf poses a second question: in each game you played, what is the fewest number of cubes of each color that could have been in the bag to make the game possible?

Again consider the example games from earlier:

Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green
Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue
Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red
Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red
Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green
In game 1, the game could have been played with as few as 4 red, 2 green, and 6 blue cubes. If any color had even one fewer cube, the game would have been impossible.
Game 2 could have been played with a minimum of 1 red, 3 green, and 4 blue cubes.
Game 3 must have been played with at least 20 red, 13 green, and 6 blue cubes.
Game 4 required at least 14 red, 3 green, and 15 blue cubes.
Game 5 needed no fewer than 6 red, 3 green, and 2 blue cubes in the bag.
The power of a set of cubes is equal to the numbers of red, green, and blue cubes multiplied together. The power of the minimum set of cubes in game 1 is 48. In games 2-5 it was 12, 1560, 630, and 36, respectively. Adding up these five powers produces the sum 2286.

For each game, find the minimum set of cubes that must have been present. What is the sum of the power of these sets?";

        public override string? OnSolve(ReadOnlySpan<char> inputFile)
        {
            try
            {
                using var fs = new FileStream(inputFile.ToString(), FileMode.Open, FileAccess.Read);
                using var sr = new StreamReader(fs);

                var getId = new Func<ReadOnlySpan<char>, Tuple<int, int>>(source =>
                {
                    var idStart = source.IndexOfAny(numbers.AsSpan());
                    int idEnd = source[idStart..].IndexOf(':') + idStart;

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
                                    LastError = new Exception($"Invalid color in input [{match.Groups["cube"].Value}]");
                                    return null;
                            }
                        }
                    }



                    sumOfCubePower += redCubes * blueCubes * greenCubes;

                    Log.WriteInfo($"ID: {id}, [POWER: {redCubes} RED, {greenCubes} GREEN, {blueCubes} BLUE] {line}");

                }

                return sumOfCubePower.ToString();
            }
            catch (Exception ex)
            {
                LastError = ex;
                return null;
            }
        }
    }
}