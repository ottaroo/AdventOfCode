using PuzzleSolverLib.Common;

namespace PuzzleSolverLib.Puzzles.Y2024;

public class Day17a : PuzzleBaseClass
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

        public int[] Registers { get;} = [0,0,0];
        public int RegisterA => Registers[0];
        public int RegisterB => Registers[1];
        public int RegisterC => Registers[2];

        public void SetRegisterA(int value) => Registers[0] = value;
        public void SetRegisterB(int value) => Registers[1] = value;
        public void SetRegisterC(int value) => Registers[2] = value;

        public int InstructionPointer { get; set; } = 0;

        public string Execute(string program)
        {
            var getComboOperand = new Func<int, int>(op =>
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

            var outputValues = new List<int>();
            InstructionPointer = 0;
            var instructions = program.Split(",").Select(int.Parse).ToArray();
            while (InstructionPointer < instructions.Length)
            {
                var opcode = (Opcode)instructions[InstructionPointer];
                var instruction = instructions[InstructionPointer + 1];
                switch (opcode)
                {
                    case Opcode.adv:
                        SetRegisterA((int)(RegisterA / Math.Pow(2, getComboOperand.Invoke(instruction))));
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
                        SetRegisterB((int)(RegisterA / Math.Pow(2, getComboOperand.Invoke(instruction))));
                        break;
                    case Opcode.cdv:
                        SetRegisterC((int)(RegisterA / Math.Pow(2, getComboOperand.Invoke(instruction))));
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
        computer.SetRegisterA(int.Parse(registerA.Value));
        computer.SetRegisterB(int.Parse(registerB.Value));
        computer.SetRegisterC(int.Parse(registerC.Value));

        return computer.Execute(program);
    }
}