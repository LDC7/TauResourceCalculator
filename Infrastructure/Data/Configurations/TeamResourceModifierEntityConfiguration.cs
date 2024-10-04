using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TauResourceCalculator.Domain.ResourceCalculator.Models;

namespace TauResourceCalculator.Infrastructure.Data.Configurations;

internal sealed class TeamResourceModifierEntityConfiguration : IEntityTypeConfiguration<TeamResourceModifier>
{
  public void Configure(EntityTypeBuilder<TeamResourceModifier> builder)
  {
    builder
      .HasOne(e => e.Member);
  }
}
