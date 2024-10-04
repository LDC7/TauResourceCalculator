using System;
using TauResourceCalculator.Common.Abstractions;
using TauResourceCalculator.Domain.ResourceCalculator.Interfaces;

namespace TauResourceCalculator.Domain.ResourceCalculator.Models;

public class SprintResourceModifier : IIdentifiable, IResourceModifier
{
  public Guid Id { get; set; }

  public string? Name { get; set; }

  public virtual required Sprint Sprint { get; set; }

  public virtual Participant? Participant { get; set; }

  public DateOnly Start { get; set; }

  public DateOnly End { get; set; }

  public required ResourceModifierOperation Operation { get; set; }

  public required double Resource { get; set; }
}
