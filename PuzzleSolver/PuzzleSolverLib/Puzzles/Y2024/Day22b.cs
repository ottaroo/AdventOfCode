using System.Collections.Concurrent;
using System.Text;

namespace PuzzleSolverLib.Puzzles.Y2024;

public class Day22b : PuzzleBaseClass
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

        public (ulong secret, int diff, int profit)[] GetDiffAndProfit(ulong initialSecret, int iterations)
        {
            _current = initialSecret;
            _index = 0;

            var diffAndProfit = new List<(ulong secret, int diff, int profit)>();

            var previousResult = (int)(initialSecret % 10);
            while (_index < iterations)
            {
                var result = (int) (GetNext() % 10);
                diffAndProfit.Add((_current, result - previousResult, result));
                previousResult = result;
            }
            return diffAndProfit.ToArray();
        }



    }



    public override string? OnSolve(ReadOnlySpan<char> inputFile)
    {

        var buyerSecrets = File.ReadAllLines(inputFile.ToString()).Where(l => !string.IsNullOrWhiteSpace(l)).Select(ulong.Parse).ToList();

        var sequenceMap = new Dictionary<string, int>();
        foreach (var secret in buyerSecrets)
        {
            var calculator = new SecretNumberCalculator();
            var diffAndProfit = calculator.GetDiffAndProfit(secret, 2000);

            var sequenceKeyUsed = new HashSet<string>();

            for (var i = 0; i < diffAndProfit.Length; i++)
            {
                var localIndex = i;
                var sb = new StringBuilder();
                if (i > 3)
                {
                    sb.Append($"{diffAndProfit[i - 3].diff},");
                    sb.Append($"{diffAndProfit[i - 2].diff},");
                    sb.Append($"{diffAndProfit[i - 1].diff},");
                    sb.Append($"{diffAndProfit[i].diff}");
                    var sequenceKey = sb.ToString();

                    // Only 1 sequence hit per buyer
                    if (sequenceKeyUsed.Contains(sequenceKey))
                        continue;

                    if (sequenceMap.TryGetValue(sequenceKey, out var current))
                        sequenceMap[sequenceKey] = current + diffAndProfit[localIndex].profit;
                    else
                        sequenceMap.Add(sequenceKey, diffAndProfit[localIndex].profit);

                    sequenceKeyUsed.Add(sequenceKey);
                }
            }
        }

        var maxProfit = sequenceMap.Max(x => x.Value);
        var bestSequence = sequenceMap.First(x => x.Value == maxProfit).Key;

        return $"Max profit: {maxProfit} - sequence: {bestSequence}"; // 1995 - to low according to AoC gotta get back to this one...
    }

}