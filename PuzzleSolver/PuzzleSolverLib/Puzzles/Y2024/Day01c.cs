using System.Text;
using System.Text.RegularExpressions;

namespace PuzzleSolverLib.Puzzles.Y2024
{
    /// <summary>
    /// | Method                 | Mean       | Error    | StdDev   | Gen0    | Gen1   | Allocated |
    /// |----------------------- |-----------:|---------:|---------:|--------:|-------:|----------:|
    /// | SolvePuzzle_2024_Day1b | 1,415.2 us | 27.79 us | 28.54 us | 80.0781 | 7.8125 |  665.2 KB |
    /// | SolvePuzzle_2024_Day1c |   363.7 us |  3.96 us |  3.70 us | 27.3438 | 1.9531 | 226.26 KB |
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

        public List<int> Left { get; } = new(1000);
        public List<int> Right { get; } = new(1000);

        private void ParseInput(ReadOnlySpan<char> inputFile)
        {
            
            using var fs = new FileStream(inputFile.ToString(), FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan);
            using var sr = new StreamReader(fs, Encoding.UTF8, true, 4096, true);
            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                if(string.IsNullOrWhiteSpace(line)) continue;
                var values = line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                Left.Add(int.Parse(values[0]));
                Right.Add(int.Parse(values[1]));
            }
            Left.Sort();
            Right.Sort();
        }
    }
}
