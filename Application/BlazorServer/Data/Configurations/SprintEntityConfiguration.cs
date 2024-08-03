using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TauResourceCalculator.BlazorServer.Models;

namespace TauResourceCalculator.BlazorServer.Data.Configurations;

internal sealed class SprintEntityConfiguration : IEntityTypeConfiguration<Sprint>
{
  public void Configure(EntityTypeBuilder<Sprint> builder)
  {
    builder
      .HasMany(e => e.Entries)
      .WithOne(e => e.Sprint);
  }
}
