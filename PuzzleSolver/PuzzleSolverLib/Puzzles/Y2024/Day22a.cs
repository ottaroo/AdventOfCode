using System.Globalization;
using System.Threading.Tasks.Dataflow;

namespace PuzzleSolverLib.Puzzles.Y2024;

public class Day22a : PuzzleBaseClass
{
    public class SecretNumberCalculator()
    {

        private ulong _current;
        private int _index = 0;

        public ulong GetNext()
        {
            _index++;
            
            var secret = _current * 64 ^ _current; 
            secret = secret % 16777216;  
            secret = secret / 32 ^ secret; 
            secret = secret % 16777216;
            secret = secret * 2048 ^ secret;
            secret = secret % 16777216;

            _current = secret;

            return _current;
        }

        public ulong GetSecretAfter(ulong initialSecret, int iterations)
        {
            _current = initialSecret;
            _index = 0;
            var result = 0UL;
            while (_index < iterations)
                result = GetNext();
            return result;
        }





    }

    public override string? OnSolve(ReadOnlySpan<char> inputFile)
    {
        var buyerSecrets = File.ReadAllLines(inputFile.ToString()).Where(l => !string.IsNullOrWhiteSpace(l)).Select(ulong.Parse).ToList();

        Parallel.For(0, buyerSecrets.Count, n =>
        {
            var calculator = new SecretNumberCalculator();
            buyerSecrets[n] = calculator.GetSecretAfter(buyerSecrets[n], 2000);
        });


        var sum = buyerSecrets.Sum(l=>(decimal)l);

        return sum.ToString(CultureInfo.InvariantCulture);
    }
}