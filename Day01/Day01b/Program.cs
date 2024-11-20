using System;
using System.Buffers;
using System.IO;
using System.Linq;



internal class Program
{
    private static void Main(string[] args)
    {
        using var fs = File.OpenRead(Path.Combine(AppContext.BaseDirectory, "input.txt"));
        using var sr = new StreamReader(fs);

        var sum = 0;
        var ary = new char[2];
        SearchValues<string> digits = SearchValues.Create(new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "zero" }, StringComparison.OrdinalIgnoreCase);
        SearchValues<string> digitsInReverse = SearchValues.Create(new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "eno", "owt", "eerht", "ruof", "evif", "xis", "neves", "thgie", "enin", "orez" }, StringComparison.OrdinalIgnoreCase);

        var getDigit = new Func<ReadOnlySpan<char>, char>(s =>
        {
            switch(s[0])
            {
                case '0': return '0';
                case '1': return '1';
                case '2': return '2';
                case '3': return '3';
                case '4': return '4';
                case '5': return '5';
                case '6': return '6';
                case '7': return '7';
                case '8': return '8'; 
                case '9': return '9';
            }

            return s switch
            {
                "zer" => '0',
                "one" => '1',
                "two" => '2',
                "thr" => '3',
                "fou" => '4',
                "fiv" => '5',
                "six" => '6',
                "sev" => '7',
                "eig" => '8',
                "nin" => '9',

                "ore" => '0',
                "eno" => '1',
                "owt" => '2',
                "eer" => '3',
                "ruo" => '4',
                "evi" => '5',
                "xis" => '6',
                "nev" => '7',
                "thg" => '8',
                "eni" => '9',

                _ => '0'

            };

        });

        var i = 0;

        while (!sr.EndOfStream)
        {
            var line = sr.ReadLine();
            if (line == null) continue;

            var work = line.AsSpan();

            var firstIndex = work.IndexOfAny(digits);
            var sliceSize = work.Length - firstIndex;
            ary[0] = getDigit(work.Slice(firstIndex, sliceSize >= 3 ? 3 : 1));

            var reverse = work.ToString().Reverse().ToArray().AsSpan();
            var lastIndex = reverse.IndexOfAny(digitsInReverse);
            sliceSize = reverse.Length - lastIndex;

            ary[1] = getDigit(reverse.Slice(lastIndex, sliceSize >= 3 ? 3 : 1));

            var n = int.Parse(ary);


            sum += n;


        }

        Console.WriteLine(sum);
    }
}