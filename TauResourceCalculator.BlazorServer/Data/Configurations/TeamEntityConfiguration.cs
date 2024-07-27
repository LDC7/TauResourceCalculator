using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TauResourceCalculator.BlazorServer.Models;

namespace TauResourceCalculator.BlazorServer.Data.Configurations;

internal sealed class TeamEntityConfiguration : IEntityTypeConfiguration<Team>
{
  public void Configure(EntityTypeBuilder<Team> builder)
  {
    builder
      .HasMany(e => e.Members)
      .WithOne(e => e.Team);
  }
}
