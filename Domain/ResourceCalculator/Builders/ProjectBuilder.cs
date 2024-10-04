using System;
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
    uint weeksPerSprint,
    uint sprintsCount)
  {
    this.project = new Project()
    {
      Name = this.team.Name,
      Version = version
    };

    foreach (var member in this.team.Members)
      _ = await this.AddParticipant(member);

    var iterationStartDate = start;
    for (uint i = 1; i <= sprintsCount; i++)
    {
      var sprint = new Sprint()
      {
        Name = SprintUtils.GenerateSprintName(version!, i, sprintsCount),
        Start = iterationStartDate,
        End = DateUtils.FindNextSunday(iterationStartDate, (int)weeksPerSprint),
        Project = this.project
      };

      iterationStartDate = sprint.End.AddDays(1);

#warning сконвертировать модификаторы.

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
}
