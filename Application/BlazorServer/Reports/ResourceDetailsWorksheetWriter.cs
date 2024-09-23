using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OfficeOpenXml;
using TauResourceCalculator.Common.Extensions;
using TauResourceCalculator.Domain.ResourceCalculator.Models;

namespace TauResourceCalculator.Application.BlazorServer.Reports;

internal sealed class ResourceDetailsWorksheetWriter
{
  private readonly ExcelWorksheet worksheet;
  private int currentRowIndex;
  private Dictionary<string, HashSet<int>> membersRowIndexMap = new();
  private Dictionary<ResourceType, HashSet<int>> resourceTypeRowIndexMap = new();

  public ResourceDetailsWorksheetWriter(ExcelWorksheet worksheet)
  {
    this.worksheet = worksheet;
    this.currentRowIndex = 1;
  }

  public async Task Write(Project project, CancellationToken cancellationToken)
  {
    cancellationToken.ThrowIfCancellationRequested();

    this.membersRowIndexMap = new();
    this.resourceTypeRowIndexMap = new();

    foreach (var sprint in project.Sprints)
      await this.WriteSprint(sprint, cancellationToken);

    await this.WriteResourcesPerMembers(project, cancellationToken);
    await this.WriteResourcesPerType(project, cancellationToken);
  }

  private Task WriteSprint(Sprint sprint, CancellationToken cancellationToken)
  {
    cancellationToken.ThrowIfCancellationRequested();

    this.worksheet.Cells[this.currentRowIndex, 1].Value = sprint.Name;
    this.currentRowIndex++;

    var rowIndex = 3;
    for (var date = sprint.Start; date <= sprint.End; date = date.AddDays(1))
    {
      this.worksheet.Cells[this.currentRowIndex, rowIndex].Value = date.ToString("dd.MM.yyyy");
      rowIndex++;
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
        this.resourceTypeRowIndexMap.AddToCollection(resourceType.Value, this.currentRowIndex);

      this.worksheet.Cells[this.currentRowIndex, 1].Value = item.Key;
      this.worksheet.Cells[this.currentRowIndex, 2].Formula = $"SUM(C{this.currentRowIndex}:CZ{this.currentRowIndex})";

      rowIndex = 3;
      for (var date = sprint.Start; date <= sprint.End; date = date.AddDays(1))
      {
        var entry = item.Value.FirstOrDefault(e => e.Date == date);
        if (entry != null)
        {
          this.worksheet.Cells[this.currentRowIndex, rowIndex].Value = entry.Resource;
        }

        rowIndex++;
      }

      this.currentRowIndex++;
    }

    this.currentRowIndex++;
    return Task.CompletedTask;
  }

  private Task WriteResourcesPerMembers(Project project, CancellationToken cancellationToken)
  {
    cancellationToken.ThrowIfCancellationRequested();

    this.worksheet.Cells[this.currentRowIndex, 1].Value = "Ресурсы по членам команды";
    this.currentRowIndex++;

    foreach (var rowIndexesForMember in this.membersRowIndexMap)
    {
      this.worksheet.Cells[this.currentRowIndex, 1].Value = rowIndexesForMember.Key;
      var cells = rowIndexesForMember.Value.Select(i => $"B{i}");
      this.worksheet.Cells[this.currentRowIndex, 2].Formula = $"SUM({string.Join("; ", cells)})";
      this.currentRowIndex++;
    }

    this.currentRowIndex++;
    return Task.CompletedTask;
  }

  private Task WriteResourcesPerType(Project project, CancellationToken cancellationToken)
  {
    cancellationToken.ThrowIfCancellationRequested();

    this.worksheet.Cells[this.currentRowIndex, 1].Value = "Ресурсы по типу";
    this.currentRowIndex++;

    foreach (var rowIndexesForType in this.resourceTypeRowIndexMap)
    {
      this.worksheet.Cells[this.currentRowIndex, 1].Value = rowIndexesForType.Key.ToString();
      var cells = rowIndexesForType.Value.Select(i => $"B{i}");
      this.worksheet.Cells[this.currentRowIndex, 2].Formula = $"SUM({string.Join("; ", cells)})";
      this.currentRowIndex++;
    }

    this.currentRowIndex++;
    return Task.CompletedTask;
  }
}
