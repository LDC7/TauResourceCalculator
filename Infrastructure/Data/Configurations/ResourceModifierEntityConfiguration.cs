using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TauResourceCalculator.Domain.ResourceCalculator.Models;

namespace TauResourceCalculator.Infrastructure.Data.Configurations;

internal sealed class ResourceModifierEntityConfiguration : IEntityTypeConfiguration<ResourceModifier>
{
  public void Configure(EntityTypeBuilder<ResourceModifier> builder)
  {
    builder
      .HasOne(e => e.Member);
  }
}
