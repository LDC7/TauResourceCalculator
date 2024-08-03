using System;
using System.Collections.Generic;
using TauResourceCalculator.Common.Abstractions;

namespace TauResourceCalculator.Domain.ResourceCalculator.Models;

public class Project : IIdentifiable
{
  public Guid Id { get; set; }

  public required string Name { get; set; }

  public required string Version { get; set; }

  public DateTime Created { get; set; } = DateTime.UtcNow;

  public virtual ICollection<Sprint> Sprints { get; set; } = new List<Sprint>();

  public override string ToString()
  {
    return $"Project {this.Name} {this.Version}";
  }
}
