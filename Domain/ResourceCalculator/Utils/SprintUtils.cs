using System;
using System.Collections.Immutable;
using System.Linq;
using TauResourceCalculator.Domain.ResourceCalculator.Models;

namespace TauResourceCalculator.Domain.ResourceCalculator.Utils;

public static class SprintUtils
{
  public static string GenerateSprintName(string version, int sprintNumber, int sprintsCount)
  {
    var sprintSuffix = sprintNumber.ToString();

    if (sprintNumber == sprintsCount)
      sprintSuffix = "Freeze";
    if (sprintNumber == sprintsCount - 1)
      sprintSuffix = "Support";

    return $"{version.ToLowerInvariant()} Iteration {sprintSuffix}";
  }

  public static ImmutableArray<DateOnly> GetDates(Sprint sprint)
  {
    var days = sprint.End.DayOfYear - sprint.Start.DayOfYear;
    return Enumerable.Range(0, days + 1).Select(sprint.Start.AddDays).ToImmutableArray();
  }
}
