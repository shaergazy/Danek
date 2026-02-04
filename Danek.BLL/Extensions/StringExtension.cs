using System.Text.RegularExpressions;

namespace Danek.BLL.Extesions;

public static class StringExtension
{

    public static string ToWithoutWhitespaces(this string str)
    {
        return Regex.Replace(str, @"\s+", string.Empty);
    }

    public static string Slice(this string str, int len)
    {
        return str.Length > len
            ? str.Substring(0, len)
            : str;
    }

    public static string CutExceptionCode(this string str)
    {
        return str.ByRegex(new Regex(@"^(\d+)\.\s?(.+?)$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant), 2);
    }

    private static string ByRegex(this string str, Regex re, int groupIndex)
    {
        var match = re.Match(str);
        return match.Success ? match.Groups[groupIndex].Value : str;
    }

    public static string WrapWithSquareBrackets(this string str)
    {
        return str.WrapWithDiffWords("[", "]");
    }

    public static string WrapWithSameWords(this string str, string word)
    {
        return str.WrapWithDiffWords(word, word);
    }

    public static string WrapWithDiffWords(this string str, string left, string right)
    {
        return $"{left}{str}{right}";
    }

    public static bool IsBase64Str(this string str)
    {
        var buffer = new byte[(str.Length * 3 + 3) / 4 - (str.Length > 0 && str[^1] == '='
            ? str.Length > 1 && str[^2] == '='
                ? 2 : 1 : 0)];

        return Convert.TryFromBase64String(str, buffer, out _);
    }

    public static bool IsNotNullOrWhiteSpace(this string str)
    {
        return !str.IsNullOrWhiteSpace();
    }

    public static bool IsNullOrWhiteSpace(this string str)
    {
        return string.IsNullOrWhiteSpace(str);
    }

    public static string Format(this string str, params string[] lst)
    {
        return string.Format(str, lst);
    }

    public static string Join(this string[] lst)
    {
        return string.Join(", ", lst);
    }

    public static string Join(this IEnumerable<string> lst)
    {
        return string.Join(", ", lst);
    }
}
