using System.Buffers;

namespace PuzzleSolver.Puzzles.Y2023
{
    public class Day01b : PuzzleBaseClass
    {
        public override string Description => @"Your calculation isn't quite right. It looks like some of the digits are actually spelled out with letters: one, two, three, four, five, six, seven, eight, and nine also count as valid ""digits"".

Equipped with this new information, you now need to find the real first and last digit on each line. For example:

two1nine
eightwothree
abcone2threexyz
xtwone3four
4nineeightseven2
zoneight234
7pqrstsixteen
In this example, the calibration values are 29, 83, 13, 24, 42, 14, and 76. Adding these together produces 281.

What is the sum of all of the calibration values?";


        public override string? OnSolve(ReadOnlySpan<char> inputFile)
        {

            try
            {

                using var fs = File.OpenRead(inputFile.ToString());
                using var sr = new StreamReader(fs);

                var sum = 0;
                var ary = new char[2];
                SearchValues<string> digits = SearchValues.Create(new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "zero" }, StringComparison.OrdinalIgnoreCase);
                SearchValues<string> digitsInReverse = SearchValues.Create(new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "eno", "owt", "eerht", "ruof", "evif", "xis", "neves", "thgie", "enin", "orez" }, StringComparison.OrdinalIgnoreCase);

                var getDigit = new Func<ReadOnlySpan<char>, char>(s =>
                {
                    switch (s[0])
                    {
                        case '0': return '0';
                        case '1': return '1';
                        case '2': return '2';
                        case '3': return '3';
                        case '4': return '4';
                        case '5': return '5';
                        case '6': return '6';
                        case '7': return '7';
                        case '8': return '8';
                        case '9': return '9';
                    }

                    return s switch
                    {
                        "zer" => '0',
                        "one" => '1',
                        "two" => '2',
                        "thr" => '3',
                        "fou" => '4',
                        "fiv" => '5',
                        "six" => '6',
                        "sev" => '7',
                        "eig" => '8',
                        "nin" => '9',

                        "ore" => '0',
                        "eno" => '1',
                        "owt" => '2',
                        "eer" => '3',
                        "ruo" => '4',
                        "evi" => '5',
                        "xis" => '6',
                        "nev" => '7',
                        "thg" => '8',
                        "eni" => '9',

                        _ => '0'

                    };

                });

                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (line == null) continue;

                    var work = line.AsSpan();

                    var firstIndex = work.IndexOfAny(digits);
                    var sliceSize = work.Length - firstIndex;
                    ary[0] = getDigit(work.Slice(firstIndex, sliceSize >= 3 ? 3 : 1));

                    var reverse = work.ToString().Reverse().ToArray().AsSpan();
                    var lastIndex = reverse.IndexOfAny(digitsInReverse);
                    sliceSize = reverse.Length - lastIndex;

                    ary[1] = getDigit(reverse.Slice(lastIndex, sliceSize >= 3 ? 3 : 1));

                    var n = int.Parse(ary);


                    sum += n;


                }

                return sum.ToString();
            }
            catch (Exception ex)
            {
                LastError = ex;
                return null;
            }
        }
    }
}