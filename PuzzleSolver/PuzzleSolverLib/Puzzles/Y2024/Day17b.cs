using System.ComponentModel.DataAnnotations;
using PuzzleSolverLib.Common;
using static System.Net.Mime.MediaTypeNames;

namespace PuzzleSolverLib.Puzzles.Y2024;

public class Day17b : PuzzleBaseClass
{
    public class ChristmasComputer
    {
        public enum Opcode
        {
            adv,
            bxl,
            bst,
            jnz,
            bxc,
            @out,
            bdv,
            cdv
        }

        public ulong[] Registers { get;} = [0,0,0];
        public ulong RegisterA => Registers[0];
        public ulong RegisterB => Registers[1];
        public ulong RegisterC => Registers[2];

        public void SetRegisterA(ulong value) => Registers[0] = value;
        public void SetRegisterB(ulong value) => Registers[1] = value;
        public void SetRegisterC(ulong value) => Registers[2] = value;

        public ulong InstructionPointer { get; set; } = 0;

        public string Execute(string program)
        {
            var getComboOperand = new Func<ulong, ulong>(op =>
            {
                var value = op switch
                {
                    < 4 => op,
                    4 => RegisterA,
                    5 => RegisterB,
                    6 => RegisterC,
                    _ => throw new InvalidOperationException()
                };

                return value;
            });

            var outputValues = new List<ulong>();
            InstructionPointer = 0;
            var instructions = program.Split(",").Select(ulong.Parse).ToArray();
            while (InstructionPointer < (ulong)instructions.Length)
            {


                var opcode = (Opcode)instructions[InstructionPointer];
                var instruction = instructions[InstructionPointer + 1];
                switch (opcode)
                {
                    case Opcode.adv:
                        SetRegisterA((ulong)(RegisterA / Math.Pow(2, getComboOperand.Invoke(instruction))));
                        break;
                    case Opcode.bxl:
                        SetRegisterB(RegisterB ^ instruction);
                        break;
                    case Opcode.bst:
                        SetRegisterB(getComboOperand.Invoke(instruction) % 8);
                        break;
                    case Opcode.jnz:
                        if (RegisterA == 0)
                            break;
                        InstructionPointer = getComboOperand.Invoke(instruction);
                        continue;
                    case Opcode.bxc:
                        SetRegisterB(RegisterB ^ RegisterC);
                        break;
                    case Opcode.@out:
                        outputValues.Add(getComboOperand.Invoke(instruction) % 8);
                        break;
                    case Opcode.bdv:
                        SetRegisterB((ulong)(RegisterA / Math.Pow(2, getComboOperand.Invoke(instruction))));
                        break;
                    case Opcode.cdv:
                        SetRegisterC((ulong)(RegisterA / Math.Pow(2, getComboOperand.Invoke(instruction))));
                        break;
                }
                InstructionPointer += 2;
            }

            return string.Join(",", outputValues.Select(x => $"{x}"));
        }


    }

    public override string? OnSolve(ReadOnlySpan<char> inputFile)
    {
        var lines = File.ReadAllLines(inputFile.ToString()).Where(x=>!string.IsNullOrWhiteSpace(x)).ToList();
        var registerA = RegularExpressions.Numbers().Match(lines[0]);
        var registerB = RegularExpressions.Numbers().Match(lines[1]);
        var registerC = RegularExpressions.Numbers().Match(lines[2]);
        var program = lines[3].Substring("Program: ".Length);

        var computer = new ChristmasComputer();

        // 
        // The output seems to write the last 3 bit ( % 8)
        // And register A is always divided by 8 before next iteration
        // Try to work it backwards
        //

        var digits = program.Replace(",", string.Empty);

        var possibleSolutions = new List<ulong>();
        possibleSolutions.Add(0);

        for (var n = digits.Length - 1; n >= 0; n--)
        {
            var tmpSolutions = possibleSolutions.ToArray();
            possibleSolutions.Clear();
            foreach (var tmpSolution in tmpSolutions)
            {
                for (var r = 0; r < 8; r++)
                {
                    var nextA = tmpSolution * 8 + (ulong) r;
                    computer.SetRegisterA(nextA);
                    computer.SetRegisterB(0);
                    computer.SetRegisterC(0);
                    var solve = computer.Execute(program);
                    if (program.EndsWith(solve, StringComparison.Ordinal))
                        possibleSolutions.Add(nextA);

                }
            }
        }

        var solution = possibleSolutions.Min(); 

        computer.SetRegisterA(solution);
        computer.SetRegisterB(0);
        computer.SetRegisterC(0);
        
        return $"{solution} = {computer.Execute(program)}";
    }
}