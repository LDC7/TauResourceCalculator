using System;
using System.Collections.Generic;
using TauResourceCalculator.BlazorServer.Interfaces;

namespace TauResourceCalculator.BlazorServer.Models;

public class Team : IIdentifiable
{
  public Guid Id { get; set; }

  public required string Name { get; set; }

  public virtual ICollection<Member> Members { get; set; } = new List<Member>();
}
