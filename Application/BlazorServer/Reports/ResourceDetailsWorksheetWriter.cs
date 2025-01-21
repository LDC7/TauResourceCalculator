using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
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

  private async Task WriteSprint(Sprint sprint, CancellationToken cancellationToken)
  {
    cancellationToken.ThrowIfCancellationRequested();
    var sprintResourceTypeRowIndexMap = new Dictionary<ResourceType, HashSet<int>>(2);
    var sprintInfo = new SprintReportInfo() { SprintId = sprint.Id };

    var titleCell = this.worksheet.Cells[this.currentRowIndex, 1];
    titleCell.Value = sprint.Name;
    titleCell.Style.Font.Bold = true;
    titleCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
    titleCell.Style.Fill.BackgroundColor.SetColor(Color.LightSkyBlue);
    var columnIndex = 4;
    for (var date = sprint.Start; date <= sprint.End; date = date.AddDays(1))
    {
      this.worksheet.Cells[this.currentRowIndex, columnIndex].Value = date.ToString("dd.MM.yyyy");
      columnIndex++;
    }
    this.currentRowIndex++;

    var participants = sprint.Project.Participants
      .OrderBy(p => p.ResourceType)
      .ThenBy(p => p.Name)
      .ToImmutableArray();

    foreach (var participant in participants)
    {
      this.membersRowIndexMap.AddToCollection(participant.Name, this.currentRowIndex);
      sprintResourceTypeRowIndexMap.AddToCollection(participant.ResourceType, this.currentRowIndex);

      var memberNameCell = this.worksheet.Cells[this.currentRowIndex, 1];
      memberNameCell.Value = participant.Name;
      memberNameCell.Style.Font.Italic = true;

      var memberResourceSumCell = this.worksheet.Cells[this.currentRowIndex, 2];
      memberResourceSumCell.Formula = $"SUM(D{this.currentRowIndex}:CZ{this.currentRowIndex})";
      memberResourceSumCell.Style.Font.Italic = true;

      var memberDefaultResourceCell = this.worksheet.Cells[this.currentRowIndex, 3];
      memberDefaultResourceCell.Value = participant.Resource;
      memberDefaultResourceCell.Style.Font.Italic = true;

      columnIndex = 4;
      for (var date = sprint.Start; date <= sprint.End; date = date.AddDays(1))
      {
        var dateResource = this.worksheet.Cells[this.currentRowIndex, columnIndex];
        dateResource.Formula = await this.CalculateResourceFormulaForDate(date, sprint, participant);

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

      this.resourceTypeRowIndexMap.AddToCollection(row.Key, this.currentRowIndex);

      this.currentRowIndex++;
    }

    var totalCell = this.worksheet.Cells[this.currentRowIndex, 1];
    totalCell.Value = "Total";
    totalCell.Style.Font.Bold = true;

    var totalValueCell = this.worksheet.Cells[this.currentRowIndex, 2];
    var firstSprintResourceTypeRowIndex = this.currentRowIndex - sprintResourceTypeRowIndexMap.Count;
    var lastSprintResourceTypeRowIndex = this.currentRowIndex - 1;
    totalValueCell.Formula = $"SUM(B{firstSprintResourceTypeRowIndex}:B{lastSprintResourceTypeRowIndex})";
    totalValueCell.Style.Font.Bold = true;
    sprintInfo.TotalResourceCell = totalValueCell.Start;
    this.currentRowIndex++;

    this.reportInfo?.SprintsInfo.Add(sprintInfo);
    this.currentRowIndex++;
  }

  private Task<string> CalculateResourceFormulaForDate(DateOnly date, Sprint sprint, Participant participant)
  {
    var modifiers = sprint.ResourceModifiers
      .Where(m => (m.Participant == null || m.Participant == participant) && date >= m.Start && date <= m.End)
      .OrderByDescending(m => (int)m.Operation)
      .ToImmutableArray();

    // HACK: если есть умножение на 0, то всё игнорим.
    if (modifiers.Any(m => m.Resource == 0 && m.Operation == ResourceModifierOperation.Multiplication))
      return Task.FromResult("0");

    var sb = new StringBuilder($"C{this.currentRowIndex}");

    sb = sb.Insert(0, "(", modifiers.Length);
    foreach (var modifier in modifiers)
    {
      var @operator = modifier.Operation switch
      {
        ResourceModifierOperation.Division => " / ",
        ResourceModifierOperation.Multiplication => " * ",
        ResourceModifierOperation.Subtraction => " - ",
        ResourceModifierOperation.Addition => " + ",
        _ => throw new NotImplementedException()
      };

      sb = sb
        .Append(@operator)
        .Append(modifier.Resource.ToString(CultureInfo.InvariantCulture))
        .Append(')');
    }

    sb = sb
      .Insert(0, "MAX(0, ")
      .Append(')');

    return Task.FromResult(sb.ToString());
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
    public IDictionary<ResourceType, ExcelCellAddress> ResourceTypeCells { get; } = new Dictionary<ResourceType, ExcelCellAddress>(2);

    public ICollection<SprintReportInfo> SprintsInfo { get; } = new List<SprintReportInfo>(7);
  }

  internal sealed record SprintReportInfo
  {
    public Guid SprintId { get; init; }

    public ExcelCellAddress TotalResourceCell { get; set; }

    public IDictionary<ResourceType, ExcelCellAddress> ResourceTypeCells { get; } = new Dictionary<ResourceType, ExcelCellAddress>(2);
  }
}
