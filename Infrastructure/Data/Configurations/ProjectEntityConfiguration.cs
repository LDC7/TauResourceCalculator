using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TauResourceCalculator.Domain.ResourceCalculator.Models;

namespace TauResourceCalculator.Infrastructure.Data.Configurations;

internal sealed class ProjectEntityConfiguration : IEntityTypeConfiguration<Project>
{
  public void Configure(EntityTypeBuilder<Project> builder)
  {
    builder
      .HasMany(e => e.Participants)
      .WithOne(e => e.Project);

    builder
      .HasMany(e => e.Sprints)
      .WithOne(e => e.Project);
  }
}
