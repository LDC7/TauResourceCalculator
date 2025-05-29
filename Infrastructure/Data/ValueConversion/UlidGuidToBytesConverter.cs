using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NUlid;

namespace TauResourceCalculator.Infrastructure.Data.ValueConversion;

internal sealed class UlidGuidToBytesConverter : ValueConverter<Guid, byte[]>
{
  private static readonly ConverterMappingHints defaultHints = new(size: 16);

  public UlidGuidToBytesConverter() : this(null)
  {
  }

  public UlidGuidToBytesConverter(ConverterMappingHints? mappingHints)
    : base(
      convertToProviderExpression: x => new Ulid(x).ToByteArray(),
      convertFromProviderExpression: x => new Ulid(x).ToGuid(),
      mappingHints: defaultHints.With(mappingHints))
  {
  }
}
