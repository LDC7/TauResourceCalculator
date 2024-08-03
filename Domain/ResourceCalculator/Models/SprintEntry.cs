using System;
using TauResourceCalculator.Common.Abstractions;

namespace TauResourceCalculator.Domain.ResourceCalculator.Models;

public class SprintEntry : IIdentifiable
{
  public Guid Id { get; set; }

  public required string Name { get; set; }

  public virtual required Sprint Sprint { get; set; }

  public DateOnly Date { get; set; }

  public double Resource { get; set; }

  public ResourceType ResourceType { get; set; }
}
