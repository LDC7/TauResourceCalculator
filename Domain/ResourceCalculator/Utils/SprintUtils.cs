namespace TauResourceCalculator.Domain.ResourceCalculator.Utils;

public static class SprintUtils
{
  public static string GenerateSprintName(string version, uint sprintNumber, uint sprintsCount)
  {
    var sprintSuffix = sprintNumber.ToString();

    if (sprintNumber == sprintsCount)
      sprintSuffix = "Freeze";
    if (sprintNumber == sprintsCount - 1)
      sprintSuffix = "Support";

    return $"{version.ToLowerInvariant()} Iteration {sprintSuffix}";
  }
}
