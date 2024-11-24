using System.Buffers;

namespace PuzzleSolver.Puzzles.Y2023
{
    public class Day01a : PuzzleBaseClass
    {
        public override string Description => @"The newly-improved calibration document consists of lines of text; each line originally contained a specific calibration value that the Elves now need to recover. 
On each line, the calibration value can be found by combining the first digit and the last digit (in that order) to form a single two-digit number.

For example:

1abc2
pqr3stu8vwx
a1b2c3d4e5f
treb7uchet
In this example, the calibration values of these four lines are 12, 38, 15, and 77. Adding these together produces 142.

Consider your entire calibration document. What is the sum of all of the calibration values?";


        public override string? OnSolve(ReadOnlySpan<char> inputFile)
        {
            try
            {
                using var fs = new FileStream(inputFile.ToString(), FileMode.Open, FileAccess.Read);
                using var sr = new StreamReader(fs);

                var sum = 0;
                var ary = new char[2];
                var digits = SearchValues.Create(['0', '1', '2', '3', '4', '5', '6', '7', '8', '9']);

                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (line == null) continue;

                    var work = line.AsSpan();

                    var firstIndex = work.IndexOfAny(digits);
                    ary[0] = work[firstIndex];

                    var lastIndex = work.LastIndexOfAny(digits);
                    ary[1] = work[lastIndex];

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
