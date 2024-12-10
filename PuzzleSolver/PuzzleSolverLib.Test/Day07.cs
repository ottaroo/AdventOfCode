namespace PuzzleSolverLib.Test
{
    [TestClass]
    public sealed class Day07
    {
        [TestMethod]
        public void Test2024_Day07a()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Input", "2024", "Day07.txt");
            var solve = new PuzzleSolverLib.Puzzles.Y2024.Day07a();


            var solution = solve.OnSolve(path);
            Assert.IsNotNull(solution);

            Assert.IsTrue(solution.Equals("3749", StringComparison.OrdinalIgnoreCase));

        }

        [TestMethod]
        public void Test2024_Day07b()
        {
            /*
             *--- Part Two ---
               The engineers seem concerned; the total calibration result you gave them is nowhere close to being within safety tolerances. Just then, you spot your mistake: some well-hidden elephants are holding a third type of operator.
               
               The concatenation operator (||) combines the digits from its left and right inputs into a single number. For example, 12 || 345 would become 12345. All operators are still evaluated left-to-right.
               
               Now, apart from the three equations that could be made true using only addition and multiplication, the above example has three more equations that can be made true by inserting operators:
               
               156: 15 6 can be made true through a single concatenation: 15 || 6 = 156.
               7290: 6 8 6 15 can be made true using 6 * 8 || 6 * 15.
               192: 17 8 14 can be made true using 17 || 8 + 14.
               Adding up all six test values (the three that could be made before using only + and * plus the new three that can now be made by also using ||) produces the new total calibration result of 11387.


                Either I'm reading this wrong or there is something wrong with the example. (Bonk! left to right)
                6 * 8 = 48 || 6 = 486 * 15 = 7290
                17 || 22 = 1722

             */


            var path = Path.Combine(AppContext.BaseDirectory, "Input", "2024", "Day07.txt");
            var solve = new PuzzleSolverLib.Puzzles.Y2024.Day07b();

            var solution = solve.OnSolve(path);
            Assert.IsNotNull(solution);

            //Assert.IsTrue(solution.Equals("46744143989399", StringComparison.OrdinalIgnoreCase), $"Was {solution} expected 46744143989399");
            Assert.IsTrue(solution.Equals("11387", StringComparison.OrdinalIgnoreCase), $"Was {solution} expected 11387");

        }
    }
}
