using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PuzzleSolverLib.Puzzles.Y2024
{
    public partial class Day01a : PuzzleBaseClass
    {
        public override string? OnSolve(ReadOnlySpan<char> inputFile)
        {
            try
            {
                ParseInput(inputFile);
                var totalDistance = 0;
                for (var n = 0; n < Left.Length; n++)
                {
                    totalDistance += Math.Abs(Left[n] - Right[n]);
                }


                return totalDistance.ToString();
            }
            catch(Exception ex)
            {
                LastError = ex;
                return null;
            }
        }

        public int[] Left { get; set; }
        public int[] Right { get; set; }

        [GeneratedRegex(@"\s*(?<left>\d+)\s+(?<right>\d+)")]
        public partial Regex Numbers();

        private void ParseInput(ReadOnlySpan<char> inputFile)
        {
            var left = new List<int>();
            var right = new List<int>();
            
            using var fs = new FileStream(inputFile.ToString(), FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan);
            using var sr = new StreamReader(fs, Encoding.UTF8, true, 4096, true);
            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                if(string.IsNullOrWhiteSpace(line)) continue;
                var match = Numbers().Match(line);
                if (!match.Success) continue;
                left.Add(int.Parse(match.Groups["left"].Value));
                right.Add(int.Parse(match.Groups["right"].Value));
            }

            Left = left.OrderBy(x => x).ToArray();
            Right = right.OrderBy(x => x).ToArray();
        }
    }
}
