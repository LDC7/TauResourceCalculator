using System;
using System.Linq;
using TauResourceCalculator.Domain.ResourceCalculator.Models;

namespace TauResourceCalculator.Domain.ResourceCalculator.Services;

public sealed class TeamService
{
  public void FillTeamWithSubstractions(Team team)
  {
    this.AddSubstractionIfNotExists(team, DayOfWeek.Monday);
    this.AddSubstractionIfNotExists(team, DayOfWeek.Tuesday);
    this.AddSubstractionIfNotExists(team, DayOfWeek.Wednesday);
    this.AddSubstractionIfNotExists(team, DayOfWeek.Thursday);
    this.AddSubstractionIfNotExists(team, DayOfWeek.Friday);
  }

  private void AddSubstractionIfNotExists(Team team, DayOfWeek dayOfWeek)
  {
    if (!team.ResourceSubstractionsPerDay.Any(s => s.Day == dayOfWeek))
      team.ResourceSubstractionsPerDay.Add(new() { TeamId = team.Id, Day = dayOfWeek, Resource = 0 });
  }
}
