using System;
using TauResourceCalculator.BlazorServer.Interfaces;

namespace TauResourceCalculator.BlazorServer.Models;

public class Member : IIdentifiable
{
  public Guid Id { get; set; }

  public required string Name { get; set; }

  public virtual required Team Team { get; set; }

  public ResourceType ResourceType { get; set; }

  public double Resource { get; set; }
}
