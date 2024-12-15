using System.Text.RegularExpressions;
using PuzzleSolverLib.Common;

namespace PuzzleSolverLib.Puzzles.Y2024;

public partial class Day13b : PuzzleBaseClass
{


    [GeneratedRegex(@"Button\s*(?<button>[A|B]):|X[\+\-\*\/]{1}(?<x>\d+)|Y[\+\-\*\/]{1}(?<y>\d+)|Prize:\s*x\=(?<prizeX>\d+)\s*,\s*y\=(?<prizeY>\d+)", RegexOptions.IgnoreCase)]
    public partial Regex ParseClawMachineData();

    public class ClawMachine(ButtonState buttonA, ButtonState buttonB, ButtonState prize)
    {

        public bool TryToSolve(out ButtonState numberOfButtonPush)
        {
            double a1 = buttonA.X, b1 = buttonA.Y, c1 = prize.X;
            double a2 = buttonB.X, b2 = buttonB.Y, c2 = prize.Y;

            if (MathFunctions.TryLinearEquationSolver((a1, b1, c1), (a2, b2, c2), out var solution))
            {
                numberOfButtonPush = new ButtonState(solution.X, solution.Y);
                return true;
            }


            numberOfButtonPush = new ButtonState(0, 0);
            return false;
        }
        
    }

    public readonly struct ButtonState(double x, double y)
    {
        public double X { get; } = x;
        public double Y { get; } = y;
    }



    public override string? OnSolve(ReadOnlySpan<char> inputFile)
    {
        var clawMachines = new List<ClawMachine>();
        var lines = File.ReadAllLines(inputFile.ToString()).Where(x=>!string.IsNullOrWhiteSpace(x)).ToList();
        var states = new Stack<ButtonState>();

        var nextValue = 0d;
        var xValue = 0d;
        var xPrize = 0d;
        var yValue = 0d;
        var yPrize = 0d;

        foreach (var line in lines)
        {
            var matches = ParseClawMachineData().Matches(line);
            foreach (Match match in matches)
            {
                if (match.Groups["x"].Success)
                {
                    xValue = double.Parse(match.Groups["x"].Value);
                    continue;
                }

                if (match.Groups["y"].Success)
                {
                    yValue = double.Parse(match.Groups["y"].Value);
                    states.Push(new ButtonState(xValue, yValue));
                    break;
                }

                if (match.Groups["prizeX"].Success)
                {
                    xPrize = double.Parse(match.Groups["prizeX"].Value) + 10000000000000;

                    if (match.Groups["prizeY"].Success)
                        yPrize = double.Parse(match.Groups["prizeY"].Value) + 10000000000000;

                    var buttonB = states.Pop();
                    var buttonA = states.Pop();

                    var claw = new ClawMachine(buttonA, buttonB, new ButtonState(xPrize, yPrize));
                    clawMachines.Add(claw);
                }
            }




        }

        foreach (var machine in clawMachines)
        {
            if (machine.TryToSolve(out var numberOfButtonPush) && numberOfButtonPush.X % 1 == 0 && numberOfButtonPush.Y % 1 == 0)
            {
                Console.WriteLine($"X: {numberOfButtonPush.X} Y: {numberOfButtonPush.Y} = {numberOfButtonPush.X * 3 + numberOfButtonPush.Y}");
            }
        }
        

        return clawMachines.Sum(x => x.TryToSolve(out var state) && state.X % 1 == 0 && state.Y % 1 == 0 ? (state.X * 3 + state.Y * 1) : 0).ToString();
    }
}