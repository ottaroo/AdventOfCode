using System.Text;

namespace PuzzleSolverLib.Puzzles.Y2023;

public class Day08a : PuzzleBaseClass
{
    public Dictionary<string, string> Map { get; } = new();


    public string Directions { get; private set; } = string.Empty;

    public override string? OnSolve(ReadOnlySpan<char> inputFile)
    {
        try
        {
            using var fs = new FileStream(inputFile.ToString(), FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan);
            using var sr = new StreamReader(fs, Encoding.UTF8, true, 4096, true);
            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                if (string.IsNullOrWhiteSpace(Directions))
                {
                    Directions = line;
                    continue;
                }

                Map.Add(line[..3], string.Create(6, line, (span, l) =>
                {
                    for (var n = 0; n < 3; n++)
                    {
                        span[n] = l[7 + n];
                        span[n + 3] = l[12 + n];
                    }
                }));
            }

            var instruction = "AAA";
            var finishLine = "ZZZ";
            var instructionCount = 0;
            while (!instruction.Equals(finishLine))
            {
                foreach (var ch in Directions.AsSpan())
                {
                    switch (ch)
                    {
                        case 'L':
                            instruction = Map[instruction][0..3];
                            break;
                        case 'R':
                            instruction = Map[instruction][3..];
                            break;
                    }

                    instructionCount++;
                    if (instruction.Equals(finishLine))
                        break;
                }
            }

            return instructionCount.ToString();
        }
        catch (Exception ex)
        {
            LastError = ex;
            return null;
        }
    }

    public override string Description => @"Too long, didn't read... see http://adventToCode.com/2023/day/8";
}