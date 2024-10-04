using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TauResourceCalculator.Domain.ResourceCalculator.Models;

namespace TauResourceCalculator.Infrastructure.Data.Configurations;

internal sealed class SprintResourceModifierEntityConfiguration : IEntityTypeConfiguration<SprintResourceModifier>
{
  public void Configure(EntityTypeBuilder<SprintResourceModifier> builder)
  {
    builder
      .HasOne(e => e.Participant);
  }
}
