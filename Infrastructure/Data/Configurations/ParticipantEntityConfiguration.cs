using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TauResourceCalculator.Domain.ResourceCalculator.Models;

namespace TauResourceCalculator.Infrastructure.Data.Configurations;

internal sealed class ParticipantEntityConfiguration : IEntityTypeConfiguration<Participant>
{
  public void Configure(EntityTypeBuilder<Participant> builder)
  {
    builder
      .HasOne<Member>()
      .WithMany()
      .HasForeignKey(e => e.MemberId)
      .OnDelete(DeleteBehavior.SetNull);
  }
}
