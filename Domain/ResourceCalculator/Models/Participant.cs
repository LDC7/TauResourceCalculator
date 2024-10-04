using System;
using TauResourceCalculator.Common.Abstractions;

namespace TauResourceCalculator.Domain.ResourceCalculator.Models;

public class Participant : IIdentifiable
{
  public Guid Id { get; set; }

  public required string Name { get; set; }

  public Guid? MemberId { get; set; }

  public ResourceType ResourceType { get; set; }

  public double Resource { get; set; }

  public virtual required Project Project { get; set; }
}
