using System.Buffers;


internal class Program
{
    private static void Main(string[] args)
    {
        using var fs = File.OpenRead(Path.Combine(AppContext.BaseDirectory, "input.txt"));
        using var sr = new StreamReader(fs);

        var sum = 0;
        var ary = new char[2];
        var digits = SearchValues.Create(['0','1', '2', '3', '4', '5', '6', '7', '8', '9']);

        while (!sr.EndOfStream)
        {
            var line = sr.ReadLine();
            if (line == null) continue;

            var work = line.AsSpan();

            var firstIndex = work.IndexOfAny(digits);
            ary[0] = work[firstIndex];

            var lastIndex = work.LastIndexOfAny(digits);
            ary[1] = work[lastIndex];

            var n = int.Parse(ary);
            sum += n;
        }

        Console.WriteLine(sum);
    }
}