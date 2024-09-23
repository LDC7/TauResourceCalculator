using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace TauResourceCalculator.Application.BlazorServer.Components;

public abstract class ApplicationComponentBase : ComponentBase, IAsyncDisposable, IDisposable
{
  private CancellationTokenSource? disposeCancellationTokenSource;

  protected CancellationToken CancellationToken => (this.disposeCancellationTokenSource ??= new()).Token;

  public void Dispose()
  {
    var task = this.DisposeAsync(true);
    if (!task.IsCompleted && !task.IsCanceled)
      task.AsTask().Wait();

    GC.SuppressFinalize(this);
  }

  public async ValueTask DisposeAsync()
  {
    await this.DisposeAsync(true);
    GC.SuppressFinalize(this);
  }

  protected virtual async ValueTask DisposeAsync(bool disposing)
  {
    if (disposing)
    {
      if (this.disposeCancellationTokenSource != null)
      {
        await this.disposeCancellationTokenSource.CancelAsync();
        this.disposeCancellationTokenSource.Dispose();
        this.disposeCancellationTokenSource = null;
      }
    }
  }
}
