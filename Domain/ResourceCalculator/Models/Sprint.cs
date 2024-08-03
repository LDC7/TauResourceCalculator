﻿using System;
using System.Collections.Generic;
using TauResourceCalculator.Common.Abstractions;

namespace TauResourceCalculator.Domain.ResourceCalculator.Models;

public class Sprint : IIdentifiable
{
  public Guid Id { get; set; }

  public required string Name { get; set; }

  public virtual required Project Project { get; set; }

  public DateOnly Start { get; set; }

  public DateOnly End { get; set; }

  public virtual ICollection<SprintEntry> Entries { get; set; } = new List<SprintEntry>();
}