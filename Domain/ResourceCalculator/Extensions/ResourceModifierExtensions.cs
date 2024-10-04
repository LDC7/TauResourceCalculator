using System;
using System.Collections.Generic;
using TauResourceCalculator.Domain.ResourceCalculator.Interfaces;
using TauResourceCalculator.Domain.ResourceCalculator.Models;

namespace TauResourceCalculator.Domain.ResourceCalculator.Extensions;

internal static class ResourceModifierExtensions
{
  public static double ApplyTo(this IResourceModifier modifier, double resource)
  {
    return modifier.Operation switch
    {
      ResourceModifierOperation.Addition => resource + modifier.Resource,
      ResourceModifierOperation.Subtraction => resource - modifier.Resource,
      ResourceModifierOperation.Multiplication => resource * modifier.Resource,
      ResourceModifierOperation.Division => resource / modifier.Resource,
      _ => throw new InvalidOperationException($"Unknown {nameof(ResourceModifierOperation)}.")
    };
  }

  public static double ApplyTo(this IEnumerable<IResourceModifier> modifiers, double resource)
  {
    foreach (var modifier in modifiers)
      resource = modifier.ApplyTo(resource);

    return resource;
  }
}
