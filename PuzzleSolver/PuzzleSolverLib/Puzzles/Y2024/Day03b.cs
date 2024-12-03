using System.Text.RegularExpressions;

namespace PuzzleSolverLib.Puzzles.Y2024
{
    public partial class Day03b : PuzzleBaseClass
    {

        [GeneratedRegex(@"(?<do>don't\(\)|do\(\))|mul\((?<left>\d+),(?<right>\d+)\)")]
        public partial Regex MulPattern();

        public override string? OnSolve(ReadOnlySpan<char> inputFile)
        {
            try
            {
                var corruptedFile = File.ReadAllText(inputFile.ToString());
                var corruptedFileSpan = corruptedFile.AsSpan();
                var sum = 0;
                var doMultiplication = true;
                foreach (ValueMatch valueMatch in MulPattern().EnumerateMatches(corruptedFileSpan))
                {
                    var txt = corruptedFileSpan.Slice(valueMatch.Index, valueMatch.Length);
                    
                    var match = MulPattern().Match(txt.ToString());

                    if(match.Groups["do"].Success)
                    {
                        doMultiplication = match.Groups["do"].Value.Equals("do()");
                        continue;
                    }

                    if (!doMultiplication)
                        continue;

                    var left = int.Parse(match.Groups["left"].Value);
                    var right = int.Parse(match.Groups["right"].Value);
                    sum += (left * right);
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