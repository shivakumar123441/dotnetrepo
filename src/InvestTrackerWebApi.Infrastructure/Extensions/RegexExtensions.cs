namespace InvestTrackerWebApi.Infrastructure.Extensions;
using System.Text.RegularExpressions;

public static class RegexExtensions
{
    private static readonly Regex Whitespace = new(@"\s+");

    public static string ReplaceWhitespace(this string input, string replacement) =>
        Whitespace.Replace(input, replacement);
}
