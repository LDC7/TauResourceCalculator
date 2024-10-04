using TauResourceCalculator.Domain.ResourceCalculator.Models;

namespace TauResourceCalculator.Domain.ResourceCalculator.Extensions;

public static class ProjectExtensions
{
  public static Participant CreateParticipantFromMember(this Project project, Member member)
  {
    var participant = new Participant()
    {
      MemberId = member.Id,
      Name = member.Name,
      Project = project,
      ResourceType = member.ResourceType,
      Resource = member.Resource
    };

    project.Participants.Add(participant);
    return participant;
  }
}
