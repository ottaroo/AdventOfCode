using System.Text.RegularExpressions;

namespace PuzzleSolverLib.Common;

public partial class RegularExpressions
{
    [GeneratedRegex(@"\d+")]
    public static partial Regex Numbers();

    [GeneratedRegex(@"\d")]
    public static partial Regex Digits();

}