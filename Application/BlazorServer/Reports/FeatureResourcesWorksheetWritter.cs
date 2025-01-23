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

    var featureTypeInfo = projectResourcesReportInfo.WorkTypesInfo.First(i => i.Name == ProjectResourcesWorksheetWritter.FeaturesWorkTypeName);

    var availableFeatureResourceHeader = this.worksheet.Cells[2, 2];
    availableFeatureResourceHeader.Value = "Ресурс:";
    availableFeatureResourceHeader.Style.Font.Bold = true;
    availableFeatureResourceHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

    var estimatedResourceSumHeader = this.worksheet.Cells[3, 2];
    estimatedResourceSumHeader.Value = "Запланировано:";
    estimatedResourceSumHeader.Style.Font.Bold = true;
    estimatedResourceSumHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

    var featureResourceHeader = this.worksheet.Cells[5, 2];
    featureResourceHeader.Value = "Ресурс:";
    featureResourceHeader.Style.Font.Bold = true;
    featureResourceHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

    var estimatedResourceHeader = this.worksheet.Cells[6, 2];
    estimatedResourceHeader.Value = "Запланировано:";
    estimatedResourceHeader.Style.Font.Bold = true;
    estimatedResourceHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

    var sprintColumns = new List<ExcelRangeColumn>(sprints.Length * resourcesInfo.Length);
    for (var i = 0; i < sprints.Length; i++)
    {
      var sprint = sprints[i];
      var sprintColumnIndex = i * resourcesInfo.Length + sprintColumnIndexOffset;
      var sprintHeader = this.worksheet.Cells[1, sprintColumnIndex, 1, sprintColumnIndex + resourcesInfo.Length - 1];
      sprintHeader.Merge = true;
      sprintHeader.Value = sprint.Name;
      sprintHeader.Style.Font.Bold = true;
      sprintHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

      var availableResourceCells = new List<ExcelCellAddress>(resourcesInfo.Length);
      var estimatedResourceCells = new List<ExcelCellAddress>(resourcesInfo.Length);

      for (var j = 0; j < resourcesInfo.Length; j++)
      {
        var info = resourcesInfo[j];
        var resourceTypeColumnIndex = sprintColumnIndex + j;
        var resourceTypeHeader = this.worksheet.Cells[4, resourceTypeColumnIndex];
        resourceTypeHeader.Value = info.Name;
        resourceTypeHeader.Style.Font.Bold = true;
        resourceTypeHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        sprintColumns.Add(resourceTypeHeader.EntireColumn);

        if (info.ResourcePerSprint.TryGetValue(sprint.Id, out var sprintResourceInfo))
        {
          var availableResourceCell = this.worksheet.Cells[5, resourceTypeColumnIndex];
          var featureAvailableResourceAddress = sprintResourceInfo.ResourcePerWorkType[featureTypeInfo].Address;
          availableResourceCell.Formula = $"'{projectResourcesReportInfo.Worksheet.Name}'!{featureAvailableResourceAddress}";
          availableResourceCells.Add(availableResourceCell.Start);

          var estimatedCell = this.worksheet.Cells[6, resourceTypeColumnIndex];
          estimatedResourceCells.Add(estimatedCell.Start);
          var estimatedRange = this.worksheet.Cells[7, resourceTypeColumnIndex, 999, resourceTypeColumnIndex];
          estimatedCell.Formula = $"SUM({estimatedRange.Address})";
          var estimatedFormattingRule = estimatedCell.ConditionalFormatting.AddGreaterThan();
          estimatedFormattingRule.Formula = availableResourceCell.Start.Address;
          estimatedFormattingRule.Style.Fill.PatternType = ExcelFillStyle.Solid;
          estimatedFormattingRule.Style.Fill.BackgroundColor.SetColor(Color.Crimson);
        }
      }

      var availableTotalResourceCell = this.worksheet.Cells[2, sprintColumnIndex, 2, sprintColumnIndex + resourcesInfo.Length - 1];
      availableTotalResourceCell.Merge = true;
      availableTotalResourceCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
      availableTotalResourceCell.Formula = $"SUM({string.Join(',', availableResourceCells.Select(c => c.Address))})";

      var estimatedTotalResourceCell = this.worksheet.Cells[3, sprintColumnIndex, 3, sprintColumnIndex + resourcesInfo.Length - 1];
      estimatedTotalResourceCell.Merge = true;
      estimatedTotalResourceCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
      estimatedTotalResourceCell.Formula = $"SUM({string.Join(',', estimatedResourceCells.Select(c => c.Address))})";
      var estimatedTotalFormattingRule = estimatedTotalResourceCell.ConditionalFormatting.AddGreaterThan();
      estimatedTotalFormattingRule.Formula = availableTotalResourceCell.Start.Address;
      estimatedTotalFormattingRule.Style.Fill.PatternType = ExcelFillStyle.Solid;
      estimatedTotalFormattingRule.Style.Fill.BackgroundColor.SetColor(Color.Crimson);
    }

    this.worksheet.Cells.Style.Font.Size = 12;
    this.worksheet.Columns.AutoFit();
    sprintColumns.SetMaxWidth();
    this.worksheet.Columns[1].Width = 15;
    this.worksheet.Columns[2].Width = 80;

    return Task.CompletedTask;
  }
}
