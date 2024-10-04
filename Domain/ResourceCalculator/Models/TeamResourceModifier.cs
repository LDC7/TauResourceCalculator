using System;
using TauResourceCalculator.Common.Abstractions;
using TauResourceCalculator.Domain.ResourceCalculator.Interfaces;

namespace TauResourceCalculator.Domain.ResourceCalculator.Models;

public class TeamResourceModifier : IIdentifiable, IResourceModifier
{
  public Guid Id { get; set; }

  public string? Name { get; set; }

  public virtual required Team Team { get; set; }

  public virtual Member? Member { get; set; }

  public int? WeekIndex { get; set; }

  public DayOfWeek? Day { get; set; }

  public required ResourceModifierOperation Operation { get; set; }

  public required double Resource { get; set; }
}
