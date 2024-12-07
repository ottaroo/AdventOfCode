using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;

namespace PuzzleSolverLib.Puzzles.Y2024;

public partial class Day07b : PuzzleBaseClass
{
    [GeneratedRegex(@"\d+")]
    public partial Regex Numbers();


    private bool ConcatenationToTheRescue(List<long> numbers, long valueToMatch)
    {
        var options = (int)Math.Pow(2, numbers.Count);
        for (var o = 0; o < options; o++)
        {
            var evaluator = o; // 0 = || 
            var sum = numbers[1];
            for (var n = 2; n < numbers.Count; n++)
            {
                if ((evaluator & 0x1) == 0)
                    sum *= numbers[n];
                else
                    sum += numbers[n];
                evaluator >>= 1;
            }
            if (sum == valueToMatch)
            {
                return true;
            }
        }
        return false;

    }

    private List<long> EvaluateNumbers(List<long> numbers)
    {
        var calculations = new HashSet<long>();
        var options = (int)Math.Pow(2, numbers.Count);
        for (var o = 0; o < options; o++)
        {
            var evaluator = o; // 0 multiply / 1 add
            var sum = numbers[0];
            for (var n = 1; n < numbers.Count; n++)
            {
                if ((evaluator & 0x1) == 0)
                    sum *= numbers[n];
                else
                    sum += numbers[n];

                evaluator >>= 1;
            }

            calculations.Add(sum);
        }

        return calculations.ToList();
    }

    public class Concatenation
    {
        public List<long> Left { get; set; }
        public List<long> Right { get; set; }
    }


    public long TestNumbers(long expected, long[] numbers)
    {


        var options = (int)Math.Pow(2, numbers.Length - 2);
        for (var o = 0; o < options; o++)
        {
            var evaluator = o; // 0 multiply / 1 add
            var sum = numbers[1];
            for (var n = 2; n < numbers.Length; n++)
            {
                if ((evaluator & 0x1) == 0)
                    sum *= numbers[n];
                else
                    sum += numbers[n];
                evaluator >>= 1;
            }
            if (sum == expected)
            {
                return expected;
            }
        }

        // prepare to use some more resources
        var expressions = new List<string>();
        for (var o = 0; o < options; o++)
        {
            var evaluator = o; // 0 multiply / 1 add
            var sb = new StringBuilder();
            sb.Append(numbers[1]);
            for (var n = 2; n < numbers.Length; n++)
            {
                if ((evaluator & 0x1) == 0)
                    sb.Append('*');
                else
                    sb.Append('+');
                sb.Append(numbers[n].ToString());
                evaluator >>= 1;
            }
            expressions.Add(sb.ToString());
        }


        return 0;
    }


    public override string? OnSolve(ReadOnlySpan<char> inputFile)
    {
        try
        {
            using var fs = new FileStream(inputFile.ToString(), FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan);
            using var sr = new StreamReader(fs);

            var numberOfCorrectCalibrations = 0L;

            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;

                var matches = Numbers().Matches(line);
                if (!matches.Any()) continue;

                var numbers = new List<long>();
                foreach (Match match in matches)
                    numbers.Add(long.Parse(match.Value));

                var expectedAnswer = numbers.First();

                var options = (int) Math.Pow(2, numbers.Count - 2);
                for (var o = 0; o < options; o++)
                {
                    var evaluator = o; // 0 multiply / 1 add
                    var sum = numbers[1];
                    for (var n = 2; n < numbers.Count; n++)
                    {
                        if ((evaluator & 0x1) == 0)
                            sum *= numbers[n];
                        else
                            sum += numbers[n];

                        evaluator >>= 1;
                    }


                    if (sum == expectedAnswer)
                    {
                        // found at least one valid combination
                        numberOfCorrectCalibrations += expectedAnswer;
                        goto FOUND_MATCH;
                    }
                }

                // It was not specified, but let's assume the concatenation is only once per expression

                // Okay.. so it might be a bit messier - we need to test concatenation on all possible splits

                var concatenations = new List<Concatenation>();
                for (var n = 1; n < numbers.Count; n++)
                {
                    var left = numbers.Skip(1).Take(n).ToList();
                    var right = numbers.Skip(n + 1).ToList();
                    var c = new Concatenation {Left = left, Right = right};
                    concatenations.Add(c);
                }

                var expectedAnswerAsString = expectedAnswer.ToString().AsSpan();
                foreach (var concatenation in concatenations)
                {
                    var left = EvaluateNumbers(concatenation.Left);
                    foreach (var l in left)
                    {
                        var leftPart = l.ToString();
                        if (!expectedAnswerAsString.StartsWith(l.ToString()))
                            continue;

                        if (!concatenation.Right.Any())
                        {
                            if (expectedAnswerAsString.CompareTo(l.ToString(), StringComparison.Ordinal) == 0)
                            {
                                numberOfCorrectCalibrations += expectedAnswer;
                                goto FOUND_MATCH;
                            }
                            continue;
                        }

                        var right = EvaluateNumbers(concatenation.Right);
                        foreach (var r in right)
                        {
                            var rightPart = r.ToString();
                            if (expectedAnswerAsString.Slice(leftPart.Length).IndexOf(rightPart) == -1)
                                continue;

                            Console.WriteLine($"Found string match for: {expectedAnswer}");

                            numberOfCorrectCalibrations += expectedAnswer;
                            goto FOUND_MATCH;
                        }
                    }


                }


                FOUND_MATCH: ;

            }

            return numberOfCorrectCalibrations.ToString();
        }
        catch (Exception ex)
        {
            LastError = ex;
            return null;
        }
    }
}