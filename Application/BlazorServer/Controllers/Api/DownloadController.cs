using System;
using Microsoft.AspNetCore.Mvc;
using TauResourceCalculator.Application.BlazorServer.Context;

namespace TauResourceCalculator.Application.BlazorServer.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public sealed class DownloadController : ControllerBase
{
  private readonly DownloadFileContext downloadFileContext;

  public DownloadController(DownloadFileContext downloadFileContext)
  {
    this.downloadFileContext = downloadFileContext;
  }

  [HttpGet("{fileId}")]
  public IActionResult Get([FromRoute] Guid fileId, [FromQuery] string? fileName = null)
  {
    if (!this.downloadFileContext.TryPop(fileId, out var stream))
      return this.NoContent();

    return this.File(stream, "text/plain", fileName);
  }
}
