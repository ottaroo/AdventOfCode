using System.Collections;
using System.Text.RegularExpressions;

namespace PuzzleSolverLib.Puzzles.Y2024;

public class Day24a : PuzzleBaseClass
{


    public override string? OnSolve(ReadOnlySpan<char> inputFile)
    {
        var initialWireValues = new Dictionary<string, int>();
        var outputWireValues = new Dictionary<string, int>();
        var regex = new Regex("(?<wire1>\\w+)\\s+(?<logicoperator>\\w+)\\s+(?<wire2>\\w+)\\s+->\\s*(?<output>\\w+)");

        var queueWaitingForWireInput = new Queue<(string wire1, string logicOperator, string wire2, string output)>();


        var parseWireValues = new Func<int, int, string, int>((w1, w2, op) =>
        {
            switch (op.ToUpper())
            {
                case "AND":
                    return w1 & w2;
                case "OR":
                    return w1 | w2;
                case "XOR":
                    return w1 ^ w2;
                default:
                    throw new ArgumentException("Invalid operator");
            }

        });


        foreach (var line in File.ReadLines(inputFile.ToString()).Where(s => !string.IsNullOrWhiteSpace(s)))
        {
            if (line.IndexOf(':') != -1)
            {
                initialWireValues.Add(line.Substring(0, line.IndexOf(':')), int.Parse(string.Join("", line.Substring(line.IndexOf(':')).Where(char.IsDigit))));
                continue;
            }

            var match = regex.Match(line);
            if (!match.Success)
                throw new ArgumentException("Error parsing input!");

            var wire1 = match.Groups["wire1"].Value;
            var logicOperator = match.Groups["logicoperator"].Value;
            var wire2 = match.Groups["wire2"].Value;
            var output = match.Groups["output"].Value;

            queueWaitingForWireInput.Enqueue((wire1, logicOperator, wire2, output));
        }

        while (queueWaitingForWireInput.TryDequeue(out var values))
        {
            var wire1Value = initialWireValues.TryGetValue(values.wire1, out var initialValueForWire1) ? initialValueForWire1 : outputWireValues.TryGetValue(values.wire1, out initialValueForWire1) ? initialValueForWire1 : -1;
            var wire2Value = initialWireValues.TryGetValue(values.wire2, out var initialValueForWire2) ? initialValueForWire2 : outputWireValues.TryGetValue(values.wire2, out initialValueForWire2) ? initialValueForWire2 : -1;
            if (wire1Value == -1 || wire2Value == -1)
            {
                queueWaitingForWireInput.Enqueue(values);
                continue;
            }

            outputWireValues[values.output] = parseWireValues(wire1Value, wire2Value, values.logicOperator);
        }

        var number = 0UL;
        foreach (var key in outputWireValues.Keys.Where(x => x.StartsWith("z", StringComparison.OrdinalIgnoreCase)).OrderByDescending(z => z))
        {
            number = number << 1;
            number |= (uint) outputWireValues[key];
        }


        return number.ToString();
    }
}