using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace TauResourceCalculator.Common.Extensions;

public static class StringExtensions
{
  [return: NotNullIfNotNull(nameof(@string))]
  public static string? ToKebabCaseFromPascal(this string? @string)
  {
    if (string.IsNullOrWhiteSpace(@string))
      return @string;

    var builder = new StringBuilder(@string.Length * 2);
    builder.Append(char.ToLowerInvariant(@string[0]));
    for (var i = 1; i < @string.Length; i++)
    {
      var letter = @string[i];
      if (char.IsUpper(letter))
        builder.Append('-').Append(char.ToLowerInvariant(letter));
      else
        builder.Append(letter);
    }

    return builder.ToString();
  }

  [return: NotNullIfNotNull(nameof(@string))]
  public static string? WithCapitalLetter(this string? @string)
  {
    if (string.IsNullOrWhiteSpace(@string))
      return @string;

    if (@string.Length < 2)
      return @string.ToUpperInvariant();

    return $"{char.ToUpperInvariant(@string[0])}{@string[1..].ToLowerInvariant()}";
  }
}
