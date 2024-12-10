using System.Collections.Concurrent;
using System.Text;
using System.Text.RegularExpressions;

namespace PuzzleSolverLib.Puzzles.Y2024;

public record Solution(long[] Numbers, int Operators);

public class Problem
{
    public Problem(long answer, long[] inputValues)
    {
        Answer = answer;
        InputValues = inputValues;
        ProblemParts = [];
        Solution = null;
    }
    public long Answer { get; }
    public long[] InputValues { get; }
    //public int NumberOfUnusedInputValue => HasSolution ? 0 : ProblemParts.Any() ? ProblemParts.Last().InputValues.Length : InputValues.Length ;
    public int NumberOfUnusedInputValue
    {
        get
        {
            if (ProblemParts.Any())
                return InputValues.Length - ProblemParts.Where(x => x.Solution != null).Sum(x => x.Solution!.Numbers.Length);
            return InputValues.Length - (Solution != null ? Solution.Numbers.Length : 0);

        }
    }
    public List<Problem> ProblemParts { get; set; }
    public Solution? Solution { get; set; }

    public bool HasSolution => Solution != null || NumberOfUnusedInputValue == 0;
}

public partial class Day07b : PuzzleBaseClass
{
    [GeneratedRegex(@"\d+")]
    public partial Regex Numbers();


    public bool TestStrings(Problem problem, out List<Problem> splitProblems)
    {
        var isPartProblem = problem.ProblemParts.Any();
        var currentProblem = isPartProblem ? problem.ProblemParts.Last() : problem;


        var answer = currentProblem.Answer.ToString().AsSpan();


        if (currentProblem.InputValues.Length == 1)
        {
            if (answer.CompareTo(currentProblem.InputValues[0].ToString(), StringComparison.Ordinal) == 0)
            {

                var problemPart = new Problem(currentProblem.InputValues[0], [currentProblem.InputValues[0]])
                {
                    Solution = new Solution([currentProblem.InputValues[0]], 0)
                };

                var possible = new Problem(problem.Answer, problem.InputValues.ToArray()) { ProblemParts = [problemPart]};
                if (isPartProblem)
                    possible.ProblemParts.InsertRange(0, problem.ProblemParts[..^1]);

                splitProblems = [possible];
                return possible.NumberOfUnusedInputValue == 0;
            }

            splitProblems = [];
            return false;
        }


        var possibleSolutions = new List<Problem>();

        // Find all start combinations which match answer
        for (var n = 0; n < currentProblem.InputValues.Length; n++)
        {
            foreach (var sum in GetNumberCombinations(currentProblem.InputValues[..n]))
            {
                var value = sum.sum.ToString();


                if (answer.StartsWith(value) && (answer.Length == value.Length || (answer.Length > value.Length && answer[value.Length] != '0')))
                {
                    var problemPart = new Problem(sum.sum, currentProblem.InputValues[..n])
                    {
                        Solution = new Solution(currentProblem.InputValues[..n], sum.operators)
                    };

                    var possible = new Problem(problem.Answer, problem.InputValues.ToArray()) { ProblemParts = [problemPart] };
                    if (isPartProblem)
                        possible.ProblemParts.InsertRange(0, problem.ProblemParts[..^1]);

                    if (n < currentProblem.InputValues.Length && answer.Length > value.Length)
                        possible.ProblemParts.Add(new Problem(long.Parse(answer[value.Length..]), currentProblem.InputValues.Skip(n).ToArray()));

                    possibleSolutions.Add(possible);
                }
            }
        }


        if (possibleSolutions.Any())
        {
            splitProblems = possibleSolutions.ToList();
            return true;
        }

        splitProblems = [];
        return false;
    }


    private List<(long sum, int operators)> GetNumberCombinations(ReadOnlySpan<long> numbers)
    {
        var calculations = new List<(long sum, int operators)>();
        var options = (int)Math.Pow(2, numbers.Length - 1);
        for (var o = 0; o < options; o++)
        {
            var evaluator = o; // 0 multiply / 1 add
            var sum = numbers[0];
            for (var n = 1; n < numbers.Length; n++)
            {
                if ((evaluator & 0x1) == 0)
                    sum *= numbers[n];
                else
                    sum += numbers[n];
                evaluator >>= 1;
            }
            calculations.Add((sum, o));
        }
        return calculations;
    }

    public bool TestNumbers(ref Problem problem)
    {
        var options = (int)Math.Pow(2, problem.InputValues.Length - 1);
        for (var o = 0; o < options; o++)
        {
            var evaluator = o; // 0 multiply / 1 add
            var sum = problem.InputValues[0];
            for (var n = 1; n < problem.InputValues.Length; n++)
            {
                if ((evaluator & 0x1) == 0)
                    sum *= problem.InputValues[n];
                else
                    sum += problem.InputValues[n];
                evaluator >>= 1;
            }
            if (sum == problem.Answer)
            {
                problem.Solution = new Solution(problem.InputValues, o);
                return true;
            }
        }

        return false;
    }


    private string PrintSolution(Solution solution)
    {
        
        var sln = new StringBuilder();
        var operators = solution.Operators;
        sln.Append(solution.Numbers[0].ToString());
        for (var n = 1; n < solution.Numbers.Length; ++n)
        {
            if ((operators & 1) == 0)
                sln.Append(" * ");
            else
                sln.Append(" + ");
            sln.Append(solution.Numbers[n].ToString());
            operators >>= 1;
        }

        return sln.ToString();
    }

    public string PrintSolution(Problem problem)
    {
        try
        {
            var sb = new StringBuilder();

            if (problem.Solution != null)
                return PrintSolution(problem.Solution);

            for (var s = 0; s < problem.ProblemParts.Count; s++)
            {
                var solutionPart = problem.ProblemParts[s].Solution;
                if (solutionPart == null)
                {
                    for (var n = 0; n < problem.ProblemParts[s].InputValues.Length; n++)
                    {
                        sb.Append(problem.ProblemParts[s].InputValues[n].ToString());
                        if (n < problem.ProblemParts[s].InputValues.Length - 1)
                        {
                            sb.Append(" || ");
                        }
                    }

                    continue;
                }

                sb.Append(PrintSolution(solutionPart));
                if (s < problem.ProblemParts.Count - 1)
                    sb.Append(" || ");
            }

            return sb.ToString();
        }
        catch (Exception e)
        {
            return "N/A";
        }
    }


    public override string? OnSolve(ReadOnlySpan<char> inputFile)
    {
        try
        {
            using var fs = new FileStream(inputFile.ToString(), FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan);
            using var sr = new StreamReader(fs);

            var workRound1 = new ConcurrentBag<Problem>();
            var workRound2 = new ConcurrentQueue<Problem>();
            var solutions = new ConcurrentBag<Problem>();
            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;

                var matches = Numbers().Matches(line);
                if (!matches.Any()) continue;

                var expectedAnswer = long.Parse(matches.First().Value);

                var numbers = new List<long>();
                foreach (Match match in matches.Skip(1))
                    numbers.Add(long.Parse(match.Value));


                workRound1.Add(new Problem(expectedAnswer, numbers.ToArray()));
            }

            var options = new ParallelOptions() {MaxDegreeOfParallelism = Environment.ProcessorCount};

            Parallel.ForEach(workRound1, options, () => new Dictionary<bool, List<Problem>>(), (p, s, l) =>
            {
                if (TestNumbers(ref p))
                {
                    if (l.TryGetValue(true, out var trueList))
                        trueList.Add(p);
                    else
                        l.Add(true, new List<Problem> { p });

                    return l;
                }

                if (l.TryGetValue(false, out var falseList))
                    falseList.Add(p);
                else
                    l.Add(false, new List<Problem> { p });
                return l;
            }, result =>
            {
                if (result.TryGetValue(true, out var trueList))
                    foreach (var p in trueList)
                        solutions.Add(p);
                if (result.TryGetValue(false, out var falseList))
                    foreach (var p in falseList)
                        workRound2.Enqueue(p);
            });

            Log.WriteWarning($"Queued {workRound2.Count} items to test for ||");

            if (!workRound2.Any())
                return solutions.Sum(x=>x.Answer).ToString();


            var workTasks = new List<Task>();
            for (var n = 0; n < Environment.ProcessorCount; ++n)
            {
                var task = new Task(() =>
                {
                    while (true)
                    {
                        if (!workRound2.TryDequeue(out var problem))
                            break;

                        // Just in case we have already solved it
                        if (solutions.Any(x => x.Answer.Equals(problem.Answer)))
                        {
                            Log.WriteWarning($"Already logged solution for answer: {problem.Answer}");
                            continue;
                        }

                        if (TestStrings(problem, out var splitProblems))
                        {
                            if (splitProblems.Any(x => x.HasSolution && x.NumberOfUnusedInputValue == 0))
                            {
                                solutions.Add(splitProblems.First(x => x.HasSolution && x.NumberOfUnusedInputValue == 0));
                                continue;
                            }

                            foreach (var splitPartProblem in splitProblems)
                            {
                                if (splitPartProblem.NumberOfUnusedInputValue > 0)
                                    workRound2.Enqueue(splitPartProblem);
                            }

                        }
                        else 
                            Log.WriteError($"Not able to find any solutions for: {problem.Answer}: {string.Join(" ", problem.InputValues.Select(x=>x.ToString()))}");

                    }




                });
                task.Start();
                workTasks.Add(task);
            }

            Task.WaitAll(workTasks.ToArray());


            var sumOfAllCalibrationReports = 0L;
            foreach (var solution in solutions)
            {
                Log.WriteInfo($"Answer: {solution.Answer} Input: {string.Join(" ", solution.InputValues.Select(x=>x.ToString()))} [solution: {PrintSolution(solution)}]");
                sumOfAllCalibrationReports += solution.Answer;
            }

            Log.WriteSuccess($"Total number of verified reports: {solutions.Count}");

            return sumOfAllCalibrationReports.ToString();

        }
        catch (Exception ex)
        {
            LastError = ex;
            return null;
        }
    }
}