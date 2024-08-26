using System;
using System.Linq;
using TauResourceCalculator.Domain.ResourceCalculator.Extensions;
using TauResourceCalculator.Domain.ResourceCalculator.Models;

namespace TauResourceCalculator.Domain.ResourceCalculator.Services;

public sealed class SprintService
{
  public string GenerateSprintName(string version, int sprintNumber, int sprintsCount)
  {
    var sprintSuffix = sprintNumber.ToString();

    if (sprintNumber == sprintsCount)
      sprintSuffix = "Freeze";
    if (sprintNumber == sprintsCount - 1)
      sprintSuffix = "Support";

    return $"{version.ToLowerInvariant()} Iteration {sprintSuffix}";
  }

  public void FillEntriesByMember(Sprint sprint, Member teamMember)
  {
    var currentDate = sprint.Start;
    while (currentDate <= sprint.End)
    {
      if (currentDate.DayOfWeek != DayOfWeek.Saturday || currentDate.DayOfWeek != DayOfWeek.Sunday)
      {
        var modifiers = teamMember.Team.ResourceModifiers
          .Where(m => (m.Day == null && m.Member == teamMember) || (m.Day == currentDate.DayOfWeek && m.Member == null) || (m.Day == currentDate.DayOfWeek && m.Member == teamMember))
          .ToArray();

        var resource = modifiers.ApplyTo(teamMember.Resource);

        var entry = new SprintEntry()
        {
          Name = teamMember.Name,
          Sprint = sprint,
          Resource = resource,
          ResourceType = teamMember.ResourceType,
          Date = currentDate
        };

        sprint.Entries.Add(entry);
      }

      currentDate = currentDate.AddDays(1);
    }
  }
}
