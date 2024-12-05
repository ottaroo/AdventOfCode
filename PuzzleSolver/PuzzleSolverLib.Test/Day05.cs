namespace PuzzleSolverLib.Test
{
    [TestClass]
    public sealed class Day05
    {
        [TestMethod]
        public void Test2024_Day05a()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Input", "2024", "Day05.txt");
            var solve = new PuzzleSolverLib.Puzzles.Y2024.Day05a();

            var solution = solve.OnSolve(path);
            Assert.IsNotNull(solution);

            Assert.IsTrue(solution.Equals("143", StringComparison.OrdinalIgnoreCase));

        }

        [TestMethod]
        public void Test2024_Day05b()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Input", "2024", "Day05.txt");
            var solve = new PuzzleSolverLib.Puzzles.Y2024.Day05b();

            var solution = solve.OnSolve(path);
            Assert.IsNotNull(solution);

            Assert.IsTrue(solution.Equals("123", StringComparison.OrdinalIgnoreCase));

        }
    }
}
