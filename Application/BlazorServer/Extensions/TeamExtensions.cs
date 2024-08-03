using System;
using System.Linq;
using TauResourceCalculator.BlazorServer.Models;

namespace TauResourceCalculator.BlazorServer.Extensions;

internal static class TeamExtensions
{
  public static void FillTeamWithSubstractions(this Team team)
  {
    team.AddSubstractionIfNotExists(DayOfWeek.Monday);
    team.AddSubstractionIfNotExists(DayOfWeek.Tuesday);
    team.AddSubstractionIfNotExists(DayOfWeek.Wednesday);
    team.AddSubstractionIfNotExists(DayOfWeek.Thursday);
    team.AddSubstractionIfNotExists(DayOfWeek.Friday);
  }

  private static void AddSubstractionIfNotExists(this Team team, DayOfWeek dayOfWeek)
  {
    if (!team.ResourceSubstractionsPerDay.Any(s => s.Day == dayOfWeek))
      team.ResourceSubstractionsPerDay.Add(new() { TeamId = team.Id, Day = dayOfWeek, Resource = 0 });
  }
}
