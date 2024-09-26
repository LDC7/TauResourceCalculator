using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using TauResourceCalculator.Application.BlazorServer.Extensions;
using TauResourceCalculator.Domain.ResourceCalculator.Models;

namespace TauResourceCalculator.Application.BlazorServer.Reports;

internal sealed class FeatureResourcesWorksheetWritter
{
  private readonly ExcelWorksheet worksheet;

  public FeatureResourcesWorksheetWritter(ExcelWorksheet worksheet)
  {
    this.worksheet = worksheet;
  }

  public Task Write(Project project, ProjectResourcesWorksheetWritter.ReportInfo projectResourcesReportInfo, CancellationToken cancellationToken)
  {
    const int sprintColumnIndexOffset = 3;

    var sprints = project.Sprints.OrderBy(s => s.Start).ToImmutableArray();
    var resourcesInfo = projectResourcesReportInfo.ResourceTypesInfo
      .Where(t => t.Name != ProjectResourcesWorksheetWritter.TotalResourceTypeInfoTitle)
      .ToImmutableArray();

    var featureResourceHeader = this.worksheet.Cells[3, 2];
    featureResourceHeader.Value = "Ресурс на фичи";
    featureResourceHeader.Style.Font.Bold = true;
    var estimatedResourceHeader = this.worksheet.Cells[4, 2];
    estimatedResourceHeader.Value = "Запланировано";
    estimatedResourceHeader.Style.Font.Bold = true;

    var sprintColumns = new List<ExcelRangeColumn>(sprints.Length * resourcesInfo.Length);
    for (var i = 0; i < sprints.Length; i++)
    {
      var sprint = sprints[i];
      var sprintColumnIndex = i * resourcesInfo.Length + sprintColumnIndexOffset;
      var sprintHeader = this.worksheet.Cells[1, sprintColumnIndex, 1, sprintColumnIndex + resourcesInfo.Length - 1];
      sprintHeader.Merge = true;
      sprintHeader.Value = sprint.Name;
      sprintHeader.Style.Font.Bold = true;

      for (var j = 0; j < resourcesInfo.Length; j++)
      {
        var info = resourcesInfo[j];
        var resourceTypeColumnIndex = sprintColumnIndex + j;
        var resourceTypeHeader = this.worksheet.Cells[2, resourceTypeColumnIndex];
        resourceTypeHeader.Value = info.Name;
        resourceTypeHeader.Style.Font.Bold = true;
        sprintColumns.Add(resourceTypeHeader.EntireColumn);

        if (info.ResourcePerSprint.TryGetValue(sprint.Id, out var sprintResourceCell))
        {
          var availableResourceCell = this.worksheet.Cells[3, resourceTypeColumnIndex];
          availableResourceCell.Formula = $"'{projectResourcesReportInfo.Worksheet.Name}'!{sprintResourceCell.Address}";

          var estimatedCell = this.worksheet.Cells[4, resourceTypeColumnIndex];
          var estimatedRange = this.worksheet.Cells[5, resourceTypeColumnIndex, 999, resourceTypeColumnIndex];
          estimatedCell.Formula = $"SUM({estimatedRange.Address})";
          var estimatedFormattingRule = estimatedCell.ConditionalFormatting.AddGreaterThan();
          estimatedFormattingRule.Formula = availableResourceCell.Start.Address;
          estimatedFormattingRule.Style.Fill.PatternType = ExcelFillStyle.Solid;
          estimatedFormattingRule.Style.Fill.BackgroundColor.SetColor(Color.Crimson);
        }
      }
    }

    this.worksheet.Cells.Style.Font.Size = 12;
    this.worksheet.Columns.AutoFit();
    sprintColumns.SetMaxWidth();

    return Task.CompletedTask;
  }
}
