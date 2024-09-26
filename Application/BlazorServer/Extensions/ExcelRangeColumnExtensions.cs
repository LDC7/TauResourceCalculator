using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml;

namespace TauResourceCalculator.Application.BlazorServer.Extensions;

internal static class ExcelRangeColumnExtensions
{
  public static void SetMaxWidth(this IEnumerable<ExcelRangeColumn> columns)
  {
    if (!columns.Any())
      return;

    var maxWidth = columns.Max(c => c.Width);
    foreach (var column in columns)
      column.Width = maxWidth;
  }
}
