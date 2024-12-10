using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuzzleSolverLib.Test
{
    [TestClass]
    public class Day08
    {
        [TestMethod]
        public void Test2024_Day08a()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Input", "2024", "Day08.txt");
            var solve = new PuzzleSolverLib.Puzzles.Y2024.Day08a();


            var solution = solve.OnSolve(path);
            Assert.IsNotNull(solution);

            Assert.IsTrue(solution.Equals("14", StringComparison.OrdinalIgnoreCase));

        }

        [TestMethod]
        public void Test2024_Day08b()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Input", "2024", "Day08b_1.txt");
            var solve = new PuzzleSolverLib.Puzzles.Y2024.Day08b();


            var solution = solve.OnSolve(path);
            Assert.IsNotNull(solution);

            Assert.IsTrue(solution.Equals("9", StringComparison.OrdinalIgnoreCase));

        }
    }
}
