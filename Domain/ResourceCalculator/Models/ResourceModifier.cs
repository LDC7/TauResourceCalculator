using System;
using TauResourceCalculator.Common.Abstractions;

namespace TauResourceCalculator.Domain.ResourceCalculator.Models;

public class ResourceModifier : IIdentifiable
{
  public Guid Id { get; set; }

  public virtual required Team Team { get; set; }

  public virtual Member? Member { get; set; }

  public DayOfWeek? Day { get; set; }

  public required ResourceModifierOperation Operation { get; set; }

  public required double Resource { get; set; }
}
