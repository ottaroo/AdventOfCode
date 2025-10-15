using System.Collections;
using System.Text.RegularExpressions;

namespace PuzzleSolverLib.Puzzles.Y2024;

public class Day24b : PuzzleBaseClass
{


    public override string? OnSolve(ReadOnlySpan<char> inputFile)
    {
        var initialWireValues = new Dictionary<string, long>();
        var outputWireValues = new Dictionary<string, long>();
        var regex = new Regex("(?<wire1>\\w+)\\s+(?<logicoperator>\\w+)\\s+(?<wire2>\\w+)\\s+->\\s*(?<output>\\w+)");

        var schema = new List<(string wire1, string logicOperator, string wire2, string output)>();


        var parseWireValues = new Func<long, long, string, long>((w1, w2, op) =>
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
                initialWireValues.Add(line.Substring(0, line.IndexOf(':')), long.Parse(string.Join("", line.Substring(line.IndexOf(':')).Where(char.IsDigit))));
                continue;
            }

            var match = regex.Match(line);
            if (!match.Success)
                throw new ArgumentException("Error parsing input!");

            var wire1 = match.Groups["wire1"].Value;
            var logicOperator = match.Groups["logicoperator"].Value;
            var wire2 = match.Groups["wire2"].Value;
            var output = match.Groups["output"].Value;

            schema.Add((wire1, logicOperator, wire2, output));
        }

        long xNumber = 0b11011;//   (1L << 46) | 3;
        long yNumber = 0b11101; // (1L << 46) | 5;
        long zNumber = xNumber + yNumber;

        Log.WriteWarning($"Setting x-wires to {xNumber} [{xNumber:B}]");
        Log.WriteWarning($"Setting y-wires to {yNumber} [{yNumber:B}]");
        Log.WriteWarning($"Expecting z-wires to be the sum of x+y = {zNumber} [{zNumber:B}]");


        var x = xNumber;
        foreach (var key in initialWireValues.Keys.Where(s => s.StartsWith("x", StringComparison.OrdinalIgnoreCase)).OrderBy(s => s))
        {
            initialWireValues[key] = x & 1;
            x >>= 1;
        }
        
        var y = yNumber;
        foreach (var key in initialWireValues.Keys.Where(s => s.StartsWith("y", StringComparison.OrdinalIgnoreCase)).OrderBy(s => s))
        {
            initialWireValues[key] = y & 1;
            y >>= 1;
        }

        x = 0L;
        foreach (var key in initialWireValues.Keys.Where(s => s.StartsWith("x", StringComparison.OrdinalIgnoreCase)).OrderByDescending(s => s))
        {
            x <<= 1;
            x |= initialWireValues[key];
        }
        Log.WriteDebug($"x-wires: {x} [{x:B}]");

        y = 0L;
        foreach (var key in initialWireValues.Keys.Where(s => s.StartsWith("y", StringComparison.OrdinalIgnoreCase)).OrderByDescending(s => s))
        {
            y <<= 1;
            y |= initialWireValues[key];
        }
        Log.WriteDebug($"y-wires: {y} [{y:B}]");


        var testWires = new Func<Queue<(string wire1, string logicOperator, string wire2, string output)>, (bool correctWiring, long result)>(queue =>
        {
            while (queue.TryDequeue(out var values))
            {
                var wire1Value = initialWireValues.TryGetValue(values.wire1, out var initialValueForWire1) ? initialValueForWire1 : outputWireValues.TryGetValue(values.wire1, out initialValueForWire1) ? initialValueForWire1 : -1;
                var wire2Value = initialWireValues.TryGetValue(values.wire2, out var initialValueForWire2) ? initialValueForWire2 : outputWireValues.TryGetValue(values.wire2, out initialValueForWire2) ? initialValueForWire2 : -1;

                // apply test values
                if (values.wire1.StartsWith("x", StringComparison.OrdinalIgnoreCase))
                    wire1Value = xNumber;
                if (values.wire1.StartsWith("y", StringComparison.OrdinalIgnoreCase))
                    wire1Value = yNumber;
                if (values.wire2.StartsWith("x", StringComparison.OrdinalIgnoreCase))
                    wire2Value = xNumber;
                if (values.wire1.StartsWith("y", StringComparison.OrdinalIgnoreCase))
                    wire2Value = yNumber;

                if (wire1Value == -1 || wire2Value == -1)
                {
                    queue.Enqueue(values);
                    continue;
                }

                outputWireValues[values.output] = parseWireValues(wire1Value, wire2Value, values.logicOperator);
            }

            var z = 0L;
            foreach (var key in outputWireValues.Keys.Where(s => s.StartsWith("z", StringComparison.OrdinalIgnoreCase)).OrderByDescending(s => s))
            {
                z <<= 1;
                z |= outputWireValues[key];
            }


            Log.WriteDebug($"z-wires: {z} [{z:B}]");

            return ((z & zNumber) == zNumber, z);

        });


        //while (queueWaitingForWireInput.TryDequeue(out var values))
        //{
        //    var wire1Value = initialWireValues.TryGetValue(values.wire1, out var initialValueForWire1) ? initialValueForWire1 : outputWireValues.TryGetValue(values.wire1, out initialValueForWire1) ? initialValueForWire1 : -1;
        //    var wire2Value = initialWireValues.TryGetValue(values.wire2, out var initialValueForWire2) ? initialValueForWire2 : outputWireValues.TryGetValue(values.wire2, out initialValueForWire2) ? initialValueForWire2 : -1;
        //    if (wire1Value == -1 || wire2Value == -1)
        //    {
        //        queueWaitingForWireInput.Enqueue(values);
        //        continue;
        //    }

        //    outputWireValues[values.output] = parseWireValues(wire1Value, wire2Value, values.logicOperator);
        //}

        var queue = new Queue<(string wire1, string logicOperator, string wire2, string output)>(schema);

        var result = testWires.Invoke(queue);




        return $"correct: {result.correctWiring}, output: {result.result} [{result.result:B}]";
    }
}