using System.Text.RegularExpressions;
using PuzzleSolverLib.Common;

namespace PuzzleSolverLib.Puzzles.Y2024;

public class MathProblem
{
    public long Answer { get; set; }
    
    public long[] Parts { get; set; }

    public bool SolutionExists => Solutions.Any();
    public List<string> Solutions { get; set; } = new();
}

public partial class Day07b : PuzzleBaseClass
{
    [GeneratedRegex(@"(?<operator>[*+|]){0,1}(?<value>\d+)")]
    public partial Regex MathOps();

    public Queue<MathProblem> Problems { get; set; } = new();

    public List<MathProblem> Solutions { get; set; } = new();

    public long EvaluateExpression(string expr)
    {
        var tmp = expr;
        var matches = MathOps().Matches(tmp);
        var num = long.Parse(matches.First().Groups["value"].Value);
        foreach (Match match in matches)
        {
            switch (match.Groups["operator"].Value)
            {
                case "*":
                    num *= long.Parse(match.Groups["value"].Value);
                    break;
                case "+":
                    num += long.Parse(match.Groups["value"].Value);
                    break;
                case "|":
                    num = long.Parse($"{num}{match.Groups["value"].Value}");
                    break;
            }
        }

        return num;
    }

    public override string? OnSolve(ReadOnlySpan<char> inputFile)
    {


        using var fs = new FileStream(inputFile.ToString(), FileMode.Open, FileAccess.Read, FileShare.Read);
        using var sr = new StreamReader(fs);
        while (!sr.EndOfStream)
        {
            var line = sr.ReadLine();
            if (string.IsNullOrWhiteSpace(line)) continue;

            var matches = RegularExpressions.Numbers().Matches(line);
            var problem = new MathProblem() {Answer = long.Parse(matches.First().Value)};
            problem.Parts = new long[matches.Count - 1];
            for (var n = 1; n < matches.Count; n++)
                problem.Parts[n - 1] = long.Parse(matches[n].Value);

            Problems.Enqueue(problem);
        }




        while (Problems.TryDequeue(out var p))
        {
            if (p == null)
                throw new ArgumentException("MathProblem cannot be null");

            var solutions = new List<string>(){p.Parts[0].ToString()};

            for (var n = 1; n < p.Parts.Length; n++)
            {
                var tmp = solutions.ToArray();
                solutions.Clear();

                foreach (var s in tmp)
                {
                    var expr = EvaluateExpression(s);
                    var sum = expr + p.Parts[n];
                    if (sum <= p.Answer)
                        solutions.Add($"{s}+{p.Parts[n]}");
                    sum = expr * p.Parts[n];
                    if (sum <= p.Answer)
                        solutions.Add($"{s}*{p.Parts[n]}");
                    if (long.Parse($"{expr}{p.Parts[n]}") <= p.Answer)
                        solutions.Add($"{s}|{p.Parts[n]}");
                }
            }

            foreach (var solution in solutions)
            {
                if (EvaluateExpression(solution) == p.Answer)
                {
                    p.Solutions.Add(solution);
                }
            }
            if (p.SolutionExists)
                Solutions.Add(p);
        }


        foreach (var solution in Solutions)
        {
            Console.WriteLine($"Answer: {solution.Answer} [solution: {solution.Solutions.First()}]");
        }



        return Solutions.Where(x=>x.SolutionExists).Sum(x=>x.Answer).ToString();

    }
}