using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OfficeOpenXml;
using TauResourceCalculator.Application.BlazorServer.Reports;
using TauResourceCalculator.Domain.ResourceCalculator.Models;

namespace TauResourceCalculator.Application.BlazorServer.Services;

internal sealed class XlsxReportService
{
  public async Task<MemoryStream> BuildReport(Project project, CancellationToken cancellationToken)
  {
    cancellationToken.ThrowIfCancellationRequested();
    using (var excelPackage = new ExcelPackage())
    {
      var resourceDetailsWorksheet = excelPackage.Workbook.Worksheets.Add("Детализация ресурсов");
      var resourceDetailsWriter = new ResourceDetailsWorksheetWriter(resourceDetailsWorksheet);
      var resourceDetailsReportInfo = await resourceDetailsWriter.Write(project, cancellationToken);

      var projectResourcesWorksheet = excelPackage.Workbook.Worksheets.Add("Ресурс на проект");
      var projectResourcesWritter = new ProjectResourcesWorksheetWritter(projectResourcesWorksheet);
      var projectResourcesReportInfo = await projectResourcesWritter.Write(project, resourceDetailsReportInfo, cancellationToken);

      var featureResourcesWorksheet = excelPackage.Workbook.Worksheets.Add("Планирование фич");
#warning Undone!

      foreach (var sprint in project.Sprints.OrderBy(s => s.Start))
        _ = await this.AddSprintWorksheets(excelPackage, sprint, projectResourcesReportInfo, cancellationToken);

      // увы.
      var stream = new MemoryStream();
      await excelPackage.SaveAsAsync(stream, cancellationToken);

      stream.Position = 0;
      return stream;
    }
  }

  private async Task<ExcelWorksheet> AddSprintWorksheets(
    ExcelPackage excelPackage,
    Sprint sprint,
    ProjectResourcesWorksheetWritter.ReportInfo projectResourcesReportInfo,
    CancellationToken cancellationToken)
  {
    cancellationToken.ThrowIfCancellationRequested();

    var worksheet = excelPackage.Workbook.Worksheets.Add(sprint.Name);

    return worksheet;
  }
}
