using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NUlid;

namespace TauResourceCalculator.Infrastructure.Data.ValueConversion;

internal sealed class UlidGuidToStringConverter : ValueConverter<Guid, string>
{
  private static readonly ConverterMappingHints defaultHints = new(size: 26);

  public UlidGuidToStringConverter() : this(null)
  {
  }

  public UlidGuidToStringConverter(ConverterMappingHints? mappingHints)
    : base(
      convertToProviderExpression: x => new Ulid(x).ToString(),
      convertFromProviderExpression: x => new Ulid(x).ToGuid(),
      mappingHints: defaultHints.With(mappingHints))
  {
  }
}
