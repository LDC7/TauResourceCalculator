using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OfficeOpenXml;
using TauResourceCalculator.Application.BlazorServer.Extensions;
using TauResourceCalculator.Common.Extensions;
using TauResourceCalculator.Domain.ResourceCalculator.Models;

namespace TauResourceCalculator.Application.BlazorServer.Reports;

internal sealed class ProjectResourcesWorksheetWritter
{
  internal const string TotalResourceTypeInfoTitle = "Total";

  private readonly ExcelWorksheet worksheet;
  private int currentRowIndex;
  private ExcelCellAddress focusCell = new();
  private ReportInfo? reportInfo;

  public ProjectResourcesWorksheetWritter(ExcelWorksheet worksheet)
  {
    this.worksheet = worksheet;
    this.currentRowIndex = 1;
  }

  public async Task<ReportInfo> Write(Project project, ResourceDetailsWorksheetWriter.ReportInfo resourceDetailsReportInfo, CancellationToken cancellationToken)
  {
    cancellationToken.ThrowIfCancellationRequested();

    this.reportInfo = new(this.worksheet);

    await this.FillWorkTypeItems();

    this.reportInfo.ResourceTypesInfo.Add(new(TotalResourceTypeInfoTitle));
    foreach (var resourceType in resourceDetailsReportInfo.ResourceTypeCells.Keys)
      this.reportInfo.ResourceTypesInfo.Add(new(resourceType.ToString()));

    var sprints = project.Sprints.OrderBy(s => s.Start).ToImmutableArray();
    var sprintColumns = new List<ExcelRangeColumn>(sprints.Length);

    const int sprintColumnIndexOffset = 2;
    this.currentRowIndex++;
    for (var i = 0; i < sprints.Length; i++)
    {
      var headerCell = this.worksheet.Cells[this.currentRowIndex, i + sprintColumnIndexOffset];
      headerCell.Value = sprints[i].Name;
      headerCell.Style.Font.Bold = true;
      sprintColumns.Add(headerCell.EntireColumn);
    }
    this.currentRowIndex++;


    foreach (var resourceTypeInfo in this.reportInfo.ResourceTypesInfo)
    {
      this.worksheet.Cells[this.currentRowIndex, 1].Value = $"Ресурс с фокусом ({resourceTypeInfo.Name})";
      for (var i = 0; i < sprints.Length; i++)
      {
        var sprint = sprints[i];
        var sprintInfo = resourceDetailsReportInfo.SprintsInfo.FirstOrDefault(s => s.SprintId == sprint.Id);
        if (sprintInfo != null)
        {
          var sprintResourceCell = resourceTypeInfo.Name == TotalResourceTypeInfoTitle
            ? sprintInfo.TotalResourceCell
            : sprintInfo.ResourceTypeCells.FirstOrDefault(p => p.Key.ToString() == resourceTypeInfo.Name).Value;
          if (sprintResourceCell != null)
          {
            var сell = this.worksheet.Cells[this.currentRowIndex, i + sprintColumnIndexOffset];
            сell.Formula = $"'{resourceDetailsReportInfo.Worksheet.Name}'!{sprintResourceCell.Address} * {this.focusCell.Address}";

            resourceTypeInfo.ResourcePerSprint.Add(sprintInfo.SprintId, сell.Start);
          }
        }
      }
      this.currentRowIndex++;
    }
    this.currentRowIndex++;


    this.currentRowIndex++;
    foreach (var resourceTypeInfo in this.reportInfo.ResourceTypesInfo)
    {
      var titleCell = this.worksheet.Cells[this.currentRowIndex, 1];
      titleCell.Value = $"Распределение ресурса ({resourceTypeInfo.Name})";
      titleCell.Style.Font.Bold = true;
      this.currentRowIndex++;

      foreach (var workTypeItem in this.reportInfo.WorkTypesInfo)
      {
        this.worksheet.Cells[this.currentRowIndex, 1].Value = workTypeItem.Name;
        for (var i = 0; i < sprints.Length; i++)
        {
          var sprint = sprints[i];
          var sprintInfo = resourceDetailsReportInfo.SprintsInfo.FirstOrDefault(s => s.SprintId == sprint.Id);
          if (sprintInfo != null && resourceTypeInfo.ResourcePerSprint.TryGetValue(sprintInfo.SprintId, out var resourceCell))
          {
            var сell = this.worksheet.Cells[this.currentRowIndex, i + sprintColumnIndexOffset];
            сell.Formula = $"{resourceCell.Address} * {workTypeItem.MultiplierCell.Address}";
          }
        }
        this.currentRowIndex++;
      }
      this.currentRowIndex++;
    }

    this.worksheet.Cells.Style.Font.Size = 12;
    this.worksheet.Columns.AutoFit();
    sprintColumns.SetMaxWidth();

    return this.reportInfo;
  }

  private Task FixStyles(IReadOnlyCollection<ExcelRangeColumn> sprintColumns)
  {
    var maxWidth = sprintColumns.Max(c => c.Width);
    foreach (var column in sprintColumns)
      column.Width = maxWidth;

    return Task.CompletedTask;
  }

  private Task FillWorkTypeItems()
  {
    this.worksheet.Cells[this.currentRowIndex, 1].Value = "Фокус";
    var focusValueCell = this.worksheet.Cells[this.currentRowIndex, 2];
    focusValueCell.Value = 0.6;
    this.focusCell = focusValueCell.Start;
    this.currentRowIndex++;

    WorkTypeInfo[] workTypeItems =
    [
      new("Фичи", 0.4),
      new("Доработоки", 0.2),
      new("Задачи отдела", 0.1),
      new("Техдолг", 0.1),
      new("Срочные доработоки", 0.2)
    ];

    foreach (var item in workTypeItems)
    {
      this.worksheet.Cells[this.currentRowIndex, 1].Value = item.Name;
      var cell = this.worksheet.Cells[this.currentRowIndex, 2];
      cell.Value = item.Value;
      item.MultiplierCell = cell.Start;
      this.currentRowIndex++;
    }

    this.reportInfo?.WorkTypesInfo.AddRange(workTypeItems);
    return Task.CompletedTask;
  }

  internal sealed record ReportInfo(ExcelWorksheet Worksheet)
  {
    public ICollection<ResourceTypeInfo> ResourceTypesInfo { get; } = new List<ResourceTypeInfo>(3);

    public ICollection<WorkTypeInfo> WorkTypesInfo { get; } = new List<WorkTypeInfo>(5);
  }

  internal sealed record ResourceTypeInfo(string Name)
  {
    public IDictionary<Guid, ExcelCellAddress> ResourcePerSprint { get; } = new Dictionary<Guid, ExcelCellAddress>(7);
  }

  internal sealed record WorkTypeInfo(string Name, double? Value)
  {
    public ExcelCellAddress? MultiplierCell { get; set; }
  }
}
