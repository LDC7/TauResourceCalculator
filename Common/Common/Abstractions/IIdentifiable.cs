using System;

namespace TauResourceCalculator.Common.Abstractions;

public interface IIdentifiable
{
  Guid Id { get; }
}
