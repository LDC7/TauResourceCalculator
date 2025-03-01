﻿using System.Collections.Generic;
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

internal sealed class SprintWorksheetWritter
{
  private readonly ExcelWorksheet worksheet;

  public SprintWorksheetWritter(ExcelWorksheet worksheet)
  {
    this.worksheet = worksheet;
  }

  public Task Write(Sprint sprint, ProjectResourcesWorksheetWritter.ReportInfo projectResourcesReportInfo, CancellationToken cancellationToken)
  {
    var sprintHeader = this.worksheet.Cells[1, 2];
    sprintHeader.Value = sprint.Name;
    sprintHeader.Style.Font.Bold = true;
    sprintHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

    var availableTotalResourceHeader = this.worksheet.Cells[2, 2];
    availableTotalResourceHeader.Value = "Ресурс:";
    availableTotalResourceHeader.Style.Font.Bold = true;
    availableTotalResourceHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

    var estimatedTotalResourceHeader = this.worksheet.Cells[3, 2];
    estimatedTotalResourceHeader.Value = "Запланировано:";
    estimatedTotalResourceHeader.Style.Font.Bold = true;
    estimatedTotalResourceHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

    var availableResourceHeader = this.worksheet.Cells[5, 2];
    availableResourceHeader.Value = "Ресурс:";
    availableResourceHeader.Style.Font.Bold = true;
    availableResourceHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

    var estimatedResourceHeader = this.worksheet.Cells[6, 2];
    estimatedResourceHeader.Value = "Запланировано:";
    estimatedResourceHeader.Style.Font.Bold = true;
    estimatedResourceHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

    var resourcesInfo = projectResourcesReportInfo.ResourceTypesInfo
      .Where(t => t.Name != ProjectResourcesWorksheetWritter.TotalResourceTypeInfoTitle)
      .ToImmutableArray();

    var workTypesInfo = projectResourcesReportInfo.WorkTypesInfo.ToImmutableArray();

    const int resourceColumnIndexOffset = 3;
    var resourceColumns = new List<ExcelRangeColumn>(resourcesInfo.Length * workTypesInfo.Length);
    for (var i = 0; i < workTypesInfo.Length; i++)
    {
      var workTypeInfo = workTypesInfo[i];
      var workTypeColumnIndex = i * resourcesInfo.Length + resourceColumnIndexOffset;
      var workTypeLastColumnIndex = workTypeColumnIndex + resourcesInfo.Length - 1;

      var workTypeHeader = this.worksheet.Cells[1, workTypeColumnIndex, 1, workTypeLastColumnIndex];
      workTypeHeader.Merge = true;
      workTypeHeader.Value = workTypeInfo.Name;
      workTypeHeader.Style.Font.Bold = true;
      workTypeHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

      var availableResourceForWorkTypeCell = this.worksheet.Cells[2, workTypeColumnIndex, 2, workTypeLastColumnIndex];
      availableResourceForWorkTypeCell.Merge = true;
      availableResourceForWorkTypeCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
      var availableResourcesForWorkTypeRange = this.worksheet.Cells[5, workTypeColumnIndex, 5, workTypeLastColumnIndex];
      availableResourceForWorkTypeCell.Formula = $"SUM({availableResourcesForWorkTypeRange.Address})";

      var estimatedResourceForWorkTypeCell = this.worksheet.Cells[3, workTypeColumnIndex, 3, workTypeLastColumnIndex];
      estimatedResourceForWorkTypeCell.Merge = true;
      estimatedResourceForWorkTypeCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
      var estimatedResourcesRange = this.worksheet.Cells[6, workTypeColumnIndex, 6, workTypeLastColumnIndex];
      estimatedResourceForWorkTypeCell.Formula = $"SUM({estimatedResourcesRange.Address})";
      var estimatedTotalFormattingRule = estimatedResourceForWorkTypeCell.ConditionalFormatting.AddGreaterThan();
      estimatedTotalFormattingRule.Formula = availableResourceForWorkTypeCell.Start.Address;
      estimatedTotalFormattingRule.Style.Fill.PatternType = ExcelFillStyle.Solid;
      estimatedTotalFormattingRule.Style.Fill.BackgroundColor.SetColor(Color.Crimson);

      for (var j = 0; j < resourcesInfo.Length; j++)
      {
        var resourceTypeInfo = resourcesInfo[j];
        var resourceTypeColumnIndex = workTypeColumnIndex + j;
        var resourceTypeHeader = this.worksheet.Cells[4, resourceTypeColumnIndex];
        resourceTypeHeader.Value = resourceTypeInfo.Name;
        resourceTypeHeader.Style.Font.Bold = true;
        resourceTypeHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        resourceColumns.Add(resourceTypeHeader.EntireColumn);

        var availableResourceForTypeAddress = resourceTypeInfo.ResourcePerSprint[sprint.Id].ResourcePerWorkType[workTypeInfo];
        var availableResourceForTypeCell = this.worksheet.Cells[5, resourceTypeColumnIndex];
        availableResourceForTypeCell.Formula = $"'{projectResourcesReportInfo.Worksheet.Name}'!{availableResourceForTypeAddress.Address}";

        var estimatedResourceForTypeCell = this.worksheet.Cells[6, resourceTypeColumnIndex];
        var estimatedRange = this.worksheet.Cells[7, resourceTypeColumnIndex, 999, resourceTypeColumnIndex];
        estimatedResourceForTypeCell.Formula = $"SUM({estimatedRange.Address})";
        var estimatedFormattingRule = estimatedResourceForTypeCell.ConditionalFormatting.AddGreaterThan();
        estimatedFormattingRule.Formula = availableResourceForTypeCell.Start.Address;
        estimatedFormattingRule.Style.Fill.PatternType = ExcelFillStyle.Solid;
        estimatedFormattingRule.Style.Fill.BackgroundColor.SetColor(Color.Crimson);
      }
    }

    this.worksheet.Cells.Style.Font.Size = 12;
    this.worksheet.Columns.AutoFit();
    resourceColumns.SetMaxWidth();
    this.worksheet.Columns[1].Width = 15;
    this.worksheet.Columns[2].Width = 80;

    return Task.CompletedTask;
  }
}
