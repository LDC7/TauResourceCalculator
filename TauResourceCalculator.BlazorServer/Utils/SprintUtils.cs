﻿using System;
using System.Linq;
using TauResourceCalculator.BlazorServer.Models;

namespace TauResourceCalculator.BlazorServer.Utils;

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

  public static void FillEntriesByMember(Sprint sprint, Member teamMember)
  {
    var currentDate = sprint.Start;
    while (currentDate <= sprint.End)
    {
      if (currentDate.DayOfWeek != DayOfWeek.Saturday || currentDate.DayOfWeek != DayOfWeek.Sunday)
      {
        var resource = teamMember.Resource;
        resource += teamMember.Team.ResourceSubstractionsPerDay.FirstOrDefault(s => s.Day == currentDate.DayOfWeek)?.Resource ?? 0;

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
