using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using TauResourceCalculator.Common.Extensions;
using TauResourceCalculator.Domain.ResourceCalculator.Models;

namespace TauResourceCalculator.Application.BlazorServer.Reports;

internal sealed class ResourceDetailsWorksheetWriter
{
  private readonly ExcelWorksheet worksheet;
  private int currentRowIndex;
  private Dictionary<string, HashSet<int>> membersRowIndexMap = new(0);
  private Dictionary<ResourceType, HashSet<int>> resourceTypeRowIndexMap = new(0);
  private ReportInfo? reportInfo;

  public ResourceDetailsWorksheetWriter(ExcelWorksheet worksheet)
  {
    this.worksheet = worksheet;
    this.currentRowIndex = 1;
  }

  public async Task<ReportInfo> Write(Project project, CancellationToken cancellationToken)
  {
    cancellationToken.ThrowIfCancellationRequested();

    this.membersRowIndexMap = new();
    this.resourceTypeRowIndexMap = new(2);
    this.reportInfo = new(this.worksheet);

    foreach (var sprint in project.Sprints.OrderBy(s => s.Start))
      await this.WriteSprint(sprint, cancellationToken);

    await this.WriteResourcesPerMembers(project, cancellationToken);
    await this.WriteResourcesPerType(project, cancellationToken);

    this.worksheet.Cells.Style.Font.Size = 12;
    this.worksheet.Columns.AutoFit();

    return this.reportInfo;
  }

  private Task WriteSprint(Sprint sprint, CancellationToken cancellationToken)
  {
    cancellationToken.ThrowIfCancellationRequested();
    var sprintResourceTypeRowIndexMap = new Dictionary<ResourceType, HashSet<int>>(2);
    var sprintInfo = new SprintReportInfo() { SprintId = sprint.Id };

    var titleCell = this.worksheet.Cells[this.currentRowIndex, 1];
    titleCell.Value = sprint.Name;
    titleCell.Style.Font.Bold = true;
    titleCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
    titleCell.Style.Fill.BackgroundColor.SetColor(Color.LightSkyBlue);
    var columnIndex = 3;
    for (var date = sprint.Start; date <= sprint.End; date = date.AddDays(1))
    {
      this.worksheet.Cells[this.currentRowIndex, columnIndex].Value = date.ToString("dd.MM.yyyy");
      columnIndex++;
    }
    this.currentRowIndex++;

    var resourcesPerMembers = sprint.Entries
      .GroupBy(e => e.Name)
      .ToDictionary(g => g.Key, g => g.ToArray());
    foreach (var item in resourcesPerMembers)
    {
      this.membersRowIndexMap.AddToCollection(item.Key, this.currentRowIndex);
      var resourceType = item.Value.FirstOrDefault()?.ResourceType;
      if (resourceType != null)
      {
        this.resourceTypeRowIndexMap.AddToCollection(resourceType.Value, this.currentRowIndex);
        sprintResourceTypeRowIndexMap.AddToCollection(resourceType.Value, this.currentRowIndex);
      }

      this.worksheet.Cells[this.currentRowIndex, 1].Value = item.Key;
      var memberResourceCell = this.worksheet.Cells[this.currentRowIndex, 2];
      memberResourceCell.Formula = $"SUM(C{this.currentRowIndex}:CZ{this.currentRowIndex})";
      sprintInfo.MembersResourceCells.Add(item.Key, memberResourceCell.Start);

      columnIndex = 3;
      for (var date = sprint.Start; date <= sprint.End; date = date.AddDays(1))
      {
        var entry = item.Value.FirstOrDefault(e => e.Date == date);
        if (entry != null)
        {
          this.worksheet.Cells[this.currentRowIndex, columnIndex].Value = entry.Resource;
        }

        columnIndex++;
      }

      this.currentRowIndex++;
    }

    foreach (var row in sprintResourceTypeRowIndexMap)
    {
      var keyCell = this.worksheet.Cells[this.currentRowIndex, 1];
      keyCell.Value = row.Key.ToString();
      keyCell.Style.Font.Bold = true;

      var valueCell = this.worksheet.Cells[this.currentRowIndex, 2];
      var cells = row.Value.Select(i => $"B{i}");
      valueCell.Formula = $"SUM({string.Join(',', cells)})";
      valueCell.Style.Font.Bold = true;
      sprintInfo.ResourceTypeCells.Add(row.Key, valueCell.Start);

      this.currentRowIndex++;
    }

    var totalCell = this.worksheet.Cells[this.currentRowIndex, 1];
    totalCell.Value = "Total";
    totalCell.Style.Font.Bold = true;

    var totalValueCell = this.worksheet.Cells[this.currentRowIndex, 2];
    var firstSprintResourceTypeRowIndex = this.currentRowIndex - sprintResourceTypeRowIndexMap.Count - 1;
    var lastSprintResourceTypeRowIndex = this.currentRowIndex - 1;
    totalValueCell.Formula = $"SUM(B{firstSprintResourceTypeRowIndex}:B{lastSprintResourceTypeRowIndex})";
    totalValueCell.Style.Font.Bold = true;
    sprintInfo.TotalResourceCell = totalValueCell.Start;
    this.currentRowIndex++;

    this.reportInfo?.SprintsInfo.Add(sprintInfo);
    this.currentRowIndex++;
    return Task.CompletedTask;
  }

  private Task WriteResourcesPerMembers(Project project, CancellationToken cancellationToken)
  {
    cancellationToken.ThrowIfCancellationRequested();

    var titleCell = this.worksheet.Cells[this.currentRowIndex, 1];
    titleCell.Value = "Ресурсы по членам команды";
    titleCell.Style.Font.Bold = true;
    titleCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
    titleCell.Style.Fill.BackgroundColor.SetColor(Color.LightSkyBlue);
    this.currentRowIndex++;

    foreach (var row in this.membersRowIndexMap)
    {
      var keyCell = this.worksheet.Cells[this.currentRowIndex, 1];
      keyCell.Value = row.Key;
      keyCell.Style.Font.Bold = true;

      var valueCell = this.worksheet.Cells[this.currentRowIndex, 2];
      var cells = row.Value.Select(i => $"B{i}");
      valueCell.Formula = $"SUM({string.Join(',', cells)})";
      valueCell.Style.Font.Bold = true;
      this.reportInfo?.MembersResourceCells.Add(row.Key, valueCell.Start);

      this.currentRowIndex++;
    }

    this.currentRowIndex++;
    return Task.CompletedTask;
  }

  private Task WriteResourcesPerType(Project project, CancellationToken cancellationToken)
  {
    cancellationToken.ThrowIfCancellationRequested();

    var titleCell = this.worksheet.Cells[this.currentRowIndex, 1];
    titleCell.Value = "Ресурсы по типу";
    titleCell.Style.Font.Bold = true;
    titleCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
    titleCell.Style.Fill.BackgroundColor.SetColor(Color.LightSkyBlue);
    this.currentRowIndex++;

    foreach (var row in this.resourceTypeRowIndexMap)
    {
      var keyCell = this.worksheet.Cells[this.currentRowIndex, 1];
      keyCell.Value = row.Key.ToString();
      keyCell.Style.Font.Bold = true;

      var valueCell = this.worksheet.Cells[this.currentRowIndex, 2];
      var cells = row.Value.Select(i => $"B{i}");
      valueCell.Formula = $"SUM({string.Join(',', cells)})";
      valueCell.Style.Font.Bold = true;
      this.reportInfo?.ResourceTypeCells.Add(row.Key, valueCell.Start);

      this.currentRowIndex++;
    }

    this.currentRowIndex++;
    return Task.CompletedTask;
  }

  internal sealed record ReportInfo(ExcelWorksheet Worksheet)
  {
    public IDictionary<string, ExcelCellAddress> MembersResourceCells { get; } = new Dictionary<string, ExcelCellAddress>(10);

    public IDictionary<ResourceType, ExcelCellAddress> ResourceTypeCells { get; } = new Dictionary<ResourceType, ExcelCellAddress>(2);

    public ICollection<SprintReportInfo> SprintsInfo { get; } = new List<SprintReportInfo>(7);
  }

  internal sealed record SprintReportInfo
  {
    public Guid SprintId { get; init; }

    public ExcelCellAddress TotalResourceCell { get; set; }

    public IDictionary<string, ExcelCellAddress> MembersResourceCells { get; } = new Dictionary<string, ExcelCellAddress>(10);

    public IDictionary<ResourceType, ExcelCellAddress> ResourceTypeCells { get; } = new Dictionary<ResourceType, ExcelCellAddress>(2);
  }
}
