namespace PuzzleSolverLib.Test
{
    [TestClass]
    public sealed class Day06
    {
        [TestMethod]
        public void Test2024_Day06a()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Input", "2024", "Day06.txt");
            var solve = new PuzzleSolverLib.Puzzles.Y2024.Day06a();

            var solution = solve.OnSolve(path);
            Assert.IsNotNull(solution);

            Assert.IsTrue(solution.Equals("41", StringComparison.OrdinalIgnoreCase));

        }

        [TestMethod]
        public void Test2024_Day06b()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Input", "2024", "Day06.txt");
            var solve = new PuzzleSolverLib.Puzzles.Y2024.Day06b();

            var solution = solve.OnSolve(path);
            Assert.IsNotNull(solution);

            Assert.IsTrue(solution.Equals("6", StringComparison.OrdinalIgnoreCase), $"Was {solution} expected 6");

        }
    }
}
