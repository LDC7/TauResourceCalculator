using System;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using NUlid;

namespace TauResourceCalculator.Infrastructure.Data.ValueGeneration;

internal sealed class UlidGuidValueGenerator : ValueGenerator<Guid>
{
  public override bool GeneratesTemporaryValues => false;

  public override Guid Next(EntityEntry entry)
  {
    var ulid = Ulid.NewUlid();
    return ulid.ToGuid();
  }
}
