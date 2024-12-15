using System.Text.RegularExpressions;

namespace PuzzleSolverLib.Common;

public partial class RegularExpressions
{
    [GeneratedRegex(@"\d+")]
    public static partial Regex Numbers();

    [GeneratedRegex(@"\d")]
    public static partial Regex Digits();

    [GeneratedRegex(@"\w+")]
    public static partial Regex Word();

    [GeneratedRegex(@"(?<x>[-]{0,1}\d+)\s*,\s*(?<y>[-]{0,1}\d+)")]
    public static partial Regex Coordinates();

}