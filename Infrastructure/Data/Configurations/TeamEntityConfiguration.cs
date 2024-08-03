using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TauResourceCalculator.Domain.ResourceCalculator.Models;

namespace TauResourceCalculator.Infrastructure.Data.Configurations;

internal sealed class TeamEntityConfiguration : IEntityTypeConfiguration<Team>
{
  public void Configure(EntityTypeBuilder<Team> builder)
  {
    builder
      .HasMany(e => e.Members)
      .WithOne(e => e.Team);

    builder
      .OwnsMany(e => e.ResourceSubstractionsPerDay, substractionBuilder =>
      {
        substractionBuilder
          .WithOwner()
            .HasForeignKey(s => s.TeamId);

        substractionBuilder
          .HasKey(s => new { s.TeamId, s.Day });
      });
  }
}
