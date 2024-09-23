using System.IO;
using System.Threading;
using System.Threading.Tasks;
using OfficeOpenXml;
using TauResourceCalculator.Application.BlazorServer.Reports;
using TauResourceCalculator.Domain.ResourceCalculator.Models;

namespace TauResourceCalculator.Application.BlazorServer.Services;

internal sealed class XlsxReportService
{
  public async Task<Stream> BuildReport(Project project, CancellationToken cancellationToken)
  {
    cancellationToken.ThrowIfCancellationRequested();
    using (var excelPackage = new ExcelPackage())
    {
      _ = await this.AddResourceDetailsWorksheet(excelPackage, project, cancellationToken);
      _ = await this.AddProjectResourcesWorksheet(excelPackage, project, cancellationToken);
      _ = await this.AddFeatureResourcesWorksheet(excelPackage, project, cancellationToken);

      foreach (var sprint in project.Sprints)
        _ = await this.AddSprintWorksheets(excelPackage, sprint, cancellationToken);

      // увы.
      var stream = new MemoryStream();
      //await excelPackage.SaveAsAsync(stream, cancellationToken);

      await excelPackage.SaveAsAsync("test.xlsx", cancellationToken);
      return stream;
    }
  }

  private async Task<ExcelWorksheet> AddResourceDetailsWorksheet(ExcelPackage excelPackage, Project project, CancellationToken cancellationToken)
  {
    cancellationToken.ThrowIfCancellationRequested();

    var worksheet = excelPackage.Workbook.Worksheets.Add("Детализация ресурсов");
    var writer = new ResourceDetailsWorksheetWriter(worksheet);
    await writer.Write(project, cancellationToken);

    return worksheet;
  }

  private async Task<ExcelWorksheet> AddProjectResourcesWorksheet(ExcelPackage excelPackage, Project project, CancellationToken cancellationToken)
  {
    cancellationToken.ThrowIfCancellationRequested();

    var worksheet = excelPackage.Workbook.Worksheets.Add("Ресурс на проект");
    return worksheet;
  }

  private async Task<ExcelWorksheet> AddFeatureResourcesWorksheet(ExcelPackage excelPackage, Project project, CancellationToken cancellationToken)
  {
    cancellationToken.ThrowIfCancellationRequested();

    var worksheet = excelPackage.Workbook.Worksheets.Add("Планирование фич");
    return worksheet;
  }

  private async Task<ExcelWorksheet> AddSprintWorksheets(ExcelPackage excelPackage, Sprint sprint, CancellationToken cancellationToken)
  {
    cancellationToken.ThrowIfCancellationRequested();

    var worksheet = excelPackage.Workbook.Worksheets.Add(sprint.Name);

    return worksheet;
  }
}
