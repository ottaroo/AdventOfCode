using PuzzleSolverLib.Puzzles.Y2024;

namespace PuzzleSolverLib.Common;

using LinearInput = (double X, double Y, double Answer);
using LinearSolution = (double X, double Y);

public class MathFunctions
{
    public static bool TryLinearEquationSolver(LinearInput a, LinearInput b, out LinearSolution solution)
    {
        // Determinant of the coefficient matrix
        double det = a.X * b.Y - a.Y * b.X;
        if (det == 0)
        {
            solution = (0, 0);
            return false;
        }

        // Cramer's Rule
        double detA = a.Answer * b.Y - b.Answer * b.X;
        double detB = a.X * b.Answer - a.Y * a.Answer;

        double A = detA / det;
        double B = detB / det;

        solution = (A, B);

        return true;
    }
}


