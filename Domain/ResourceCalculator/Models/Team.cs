using System;
using System.Collections.Generic;
using TauResourceCalculator.Common.Abstractions;

namespace TauResourceCalculator.Domain.ResourceCalculator.Models;

public class Team : IIdentifiable
{
  public Guid Id { get; set; }

  public required string Name { get; set; }

  public virtual ICollection<Member> Members { get; set; } = new List<Member>();

  public ICollection<DayOfWeekResourceSubstraction> ResourceSubstractionsPerDay { get; set; } = new List<DayOfWeekResourceSubstraction>();
}
