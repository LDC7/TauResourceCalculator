using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using TauResourceCalculator.Domain.ResourceCalculator.Models;
using TauResourceCalculator.Domain.ResourceCalculator.Utils;

namespace TauResourceCalculator.Domain.ResourceCalculator.Builders;

public sealed class ProjectBuilder
{
  private readonly Team team;
  private Project? project;

  public ProjectBuilder(Team team)
  {
    this.team = team;
  }

  public async Task<Project> Build(
    string version,
    DateOnly start,
    int weeksPerSprint,
    int sprintsCount)
  {
    this.project = new Project()
    {
      Name = this.team.Name,
      Version = version
    };

    foreach (var member in this.team.Members)
      _ = await this.AddParticipant(member);

    var iterationStartDate = start;
    for (var i = 1; i <= sprintsCount; i++)
    {
      var sprint = new Sprint()
      {
        Name = SprintUtils.GenerateSprintName(version!, i, sprintsCount),
        Start = iterationStartDate,
        End = DateUtils.FindNextSundayByIndex(iterationStartDate, (int)weeksPerSprint - 1),
        Project = this.project
      };

      iterationStartDate = sprint.End.AddDays(1);

      foreach (var teamResourceModifier in this.team.ResourceModifiers)
        _ = await this.AddResourceModifiersToSprint(teamResourceModifier, sprint);

      this.project.Sprints.Add(sprint);
    }

    return this.project;
  }

  private Task<Participant> AddParticipant(Member member)
  {
    var participant = new Participant()
    {
      MemberId = member.Id,
      Name = member.Name,
      Project = this.project!,
      ResourceType = member.ResourceType,
      Resource = member.Resource
    };

    this.project!.Participants.Add(participant);
    return Task.FromResult(participant);
  }

  private async Task<IReadOnlyCollection<SprintResourceModifier>> AddResourceModifiersToSprint(TeamResourceModifier teamResourceModifier, Sprint sprint)
  {
    var modifiers = new List<SprintResourceModifier>();

    if (teamResourceModifier.Day != null)
    {
      if (teamResourceModifier.WeekIndex != null)
      {
        // На определённой неделе в определённый день.
        var weekDates = DateUtils.FindNextWeekByIndex(sprint.Start, teamResourceModifier.WeekIndex.Value);
        var selectedDate = weekDates.First(d => d.DayOfWeek == teamResourceModifier.Day);
        var modifier = await this.CreateSprintResourceModifierTemplate(teamResourceModifier, sprint, selectedDate, selectedDate);
        modifiers.Add(modifier);
      }
      else
      {
        // Все недели в определённый день.
        var sprintDates = SprintUtils.GetDates(sprint);
        var selectadDates = sprintDates.Where(d => d.DayOfWeek == teamResourceModifier.Day).ToImmutableArray();
        foreach (var selectedDate in selectadDates)
        {
          var modifier = await this.CreateSprintResourceModifierTemplate(teamResourceModifier, sprint, selectedDate, selectedDate);
          modifiers.Add(modifier);
        }
      }
    }
    else
    {
      if (teamResourceModifier.WeekIndex != null)
      {
        // Определённая неделя.
        var weekDates = DateUtils.FindNextWeekByIndex(sprint.Start, teamResourceModifier.WeekIndex.Value);
        var modifier = await this.CreateSprintResourceModifierTemplate(teamResourceModifier, sprint, weekDates.First(), weekDates.Last());
        modifiers.Add(modifier);
      }
      else
      {
        // Весь спринт.
        var modifier = await this.CreateSprintResourceModifierTemplate(teamResourceModifier, sprint, sprint.Start, sprint.End);
        modifiers.Add(modifier);
      }
    }

    return modifiers;
  }

  private Task<SprintResourceModifier> CreateSprintResourceModifierTemplate(TeamResourceModifier teamResourceModifier, Sprint sprint, DateOnly start, DateOnly end)
  {
    var modifier = new SprintResourceModifier()
    {
      Sprint = sprint,
      Name = teamResourceModifier.Name,
      Operation = teamResourceModifier.Operation,
      Resource = teamResourceModifier.Resource,
      Start = start,
      End = end
    };

    if (teamResourceModifier.Member != null)
      modifier.Participant = sprint.Project.Participants.FirstOrDefault(p => p.MemberId == teamResourceModifier.Member.Id);

    sprint.ResourceModifiers.Add(modifier);
    return Task.FromResult(modifier);
  }
}
