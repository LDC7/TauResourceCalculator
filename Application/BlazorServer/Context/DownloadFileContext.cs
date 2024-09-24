using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace TauResourceCalculator.Application.BlazorServer.Context;

public sealed class DownloadFileContext
{
  private readonly ConcurrentDictionary<Guid, Stream> streams = new();

  public string CreateUri(MemoryStream stream, string fileName)
  {
    var id = Guid.NewGuid();
    this.streams.TryAdd(id, stream);
    return $"/api/download/{id}?fileName={fileName}";
  }

  public bool TryPop(Guid id, [NotNullWhen(true)] out Stream? stream)
  {
    return this.streams.TryRemove(id, out stream);
  }
}
