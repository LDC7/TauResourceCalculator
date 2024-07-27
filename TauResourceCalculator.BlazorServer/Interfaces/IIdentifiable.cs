using System;
using System.ComponentModel.DataAnnotations;

namespace TauResourceCalculator.BlazorServer.Interfaces;

public interface IIdentifiable
{
  [Key]
  public Guid Id { get; }
}
