using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuzzleSolverLib.Test
{
    [TestClass]
    public class Day09
    {
        [TestMethod]
        public void Test2024_Day09a()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Input", "2024", "Day09a.txt");
            var solve = new PuzzleSolverLib.Puzzles.Y2024.Day09a();


            var solution = solve.OnSolve(path);
            Assert.IsNotNull(solution);

            Assert.IsTrue(solution.Equals("1928", StringComparison.OrdinalIgnoreCase), $"Was {solution}");

        }

        [TestMethod]
        public void Test2024_Day09b()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Input", "2024", "Day09a.txt");
            var solve = new PuzzleSolverLib.Puzzles.Y2024.Day09b();


            var solution = solve.OnSolve(path);
            Assert.IsNotNull(solution);

            Assert.IsTrue(solution.Equals("2858", StringComparison.OrdinalIgnoreCase), $"Was {solution}");

        }

    }
}
