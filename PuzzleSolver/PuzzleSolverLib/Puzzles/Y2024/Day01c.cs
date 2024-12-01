using System.Text;
using System.Text.RegularExpressions;

namespace PuzzleSolverLib.Puzzles.Y2024
{
    /// <summary>
    /// | Method                 | Mean       | Error    | StdDev   | Gen0    | Gen1   | Allocated |
    /// |----------------------- |-----------:|---------:|---------:|--------:|-------:|----------:|
    /// | SolvePuzzle_2024_Day1b | 1,415.2 us | 27.79 us | 28.54 us | 80.0781 | 7.8125 |  665.2 KB |
    /// | SolvePuzzle_2024_Day1c |   365.1 us |  3.89 us |  3.64 us | 16.1133 | 0.9766 |  132.5 KB |
    /// </summary>

    public partial class Day01c : PuzzleBaseClass
    {
        public override string? OnSolve(ReadOnlySpan<char> inputFile)
        {
            try
            {
                ParseInput(inputFile);
                var similarityScore = 0;
                var searchIndex = 0;
                for (var n = 0; n < Left.Count; n++)
                {
                    similarityScore += (Left[n] * Count(ref searchIndex, Left[n]));
                }


                return similarityScore.ToString();
            }
            catch(Exception ex)
            {
                LastError = ex;
                return null;
            }
        }

        public int Count(ref int index, int number)
        {
            var count = 0;
            while (index < Right.Count && Right[index] < number)
                index++;

            while (index < Right.Count && Right[index] == number)
            {
                count++;
                index++;
            }
            return count;
        }

        // Exploit the fact that the input is known to be 1000 elements long
        public List<int> Left { get; } = new(1000);
        public List<int> Right { get; } = new(1000);

        private void ParseInput(ReadOnlySpan<char> inputFile)
        {
            
            using var fs = new FileStream(inputFile.ToString(), FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan);
            using var sr = new StreamReader(fs, Encoding.UTF8, true, 4096, false);
            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                if(string.IsNullOrWhiteSpace(line)) continue;

                // Exploit we know its always 5 digits <space> 5 digits
                Left.Add(int.Parse(line[..5]));
                Right.Add(int.Parse(line[^5..]));
            }
            Left.Sort();
            Right.Sort();
        }
    }
}
