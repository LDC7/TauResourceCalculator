using System;

namespace TauResourceCalculator.BlazorServer.Models;

public class DayOfWeekResourceSubstraction
{
  public required Guid TeamId { get; set; }

  public required DayOfWeek Day { get; set; }

  public double Resource { get; set; }
}
