using TauResourceCalculator.Domain.ResourceCalculator.Models;

namespace TauResourceCalculator.Domain.ResourceCalculator.Interfaces;

public interface IResourceModifier
{
  public string? Name { get; set; }

  ResourceModifierOperation Operation { get; set; }

  double Resource { get; set; }
}
