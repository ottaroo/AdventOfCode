﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PuzzleSolver.Puzzles.Y2023;
using PuzzleSolver.Services;

namespace PuzzleSolver.Puzzles
{
    public abstract class PuzzleBaseClass : IPuzzle
    {
        protected ILogService Log { get; } = new LogService();
        protected Exception? LastError { get; set; } = null;

        public int Solve(ReadOnlySpan<char> inputFile)
        {
            var puzzleName = GetType().Name;
            var puzzleNamespace = GetType().Namespace;
            var puzzleYear = puzzleNamespace!.Split('.').Last();

            Log.WriteInfo($"Solving puzzle {puzzleName} [{puzzleYear.Substring(1)}]");
            Log.WriteInfo($"Input file: {inputFile}");

            var solution = OnSolve(inputFile);


            if (solution != -1)
            {
                Log.EmptyLine();
                Log.WriteInfo(Description);
                Log.EmptyLine();


                Log.WriteSuccess($"Puzzle solution  = {solution}");
            }
            else
                Log.WriteError($"Puzzle solution not found - Exception: {(LastError?.ToString() ?? "N/A")}");

            return solution;
        }

        public int Solve()
        {
            var puzzleName = GetType().Name;
            var puzzleNamespace = GetType().Namespace;
            var puzzleYear = puzzleNamespace!.Split('.').Last();

            return Solve(Path.Combine(AppContext.BaseDirectory, "Puzzles", puzzleYear, "InputFiles", $"{puzzleName.TrimEnd('a', 'A', 'b', 'B')}.txt"));
        }

        public abstract int OnSolve(ReadOnlySpan<char> inputFile);


        public abstract string Description { get; }
    }
}