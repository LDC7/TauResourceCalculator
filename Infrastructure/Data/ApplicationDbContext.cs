using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TauResourceCalculator.Common.Abstractions;
using TauResourceCalculator.Domain.ResourceCalculator.Models;
using TauResourceCalculator.Infrastructure.Data.Configurations;
using TauResourceCalculator.Infrastructure.Data.ValueConversion;
using TauResourceCalculator.Infrastructure.Data.ValueGeneration;

namespace TauResourceCalculator.Infrastructure.Data;

public abstract class ApplicationDbContext : DbContext
{
  public DbSet<Team> Teams { get; set; }

  public DbSet<Project> Projects { get; set; }

  public DbSet<Sprint> Sprints { get; set; }

  public ApplicationDbContext(DbContextOptions options)
    : base(options)
  {
  }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    builder
      .ApplyConfiguration(new TeamEntityConfiguration())
      .ApplyConfiguration(new TeamResourceModifierEntityConfiguration())
      .ApplyConfiguration(new ProjectEntityConfiguration())
      .ApplyConfiguration(new ParticipantEntityConfiguration())
      .ApplyConfiguration(new SprintEntityConfiguration())
      .ApplyConfiguration(new SprintResourceModifierEntityConfiguration());

    SetupIdentifiableKey(builder);
    ApplyTPCStrategyToIdentifiableEntities(builder);
    ApplyEnumToStringConverter(builder);

    base.OnModelCreating(builder);
  }

  private static void SetupIdentifiableKey(ModelBuilder modelBuilder)
  {
    var identifiableEntities = modelBuilder.Model
      .GetEntityTypes()
      .Where(e => e.ClrType.IsAssignableTo(typeof(IIdentifiable)))
      .ToImmutableArray();
    foreach (var entityType in identifiableEntities)
    {
      var entity = modelBuilder.Entity(entityType.ClrType);
      entity.HasKey(nameof(IIdentifiable.Id));
      entity
        .Property(nameof(IIdentifiable.Id))
          .HasValueGenerator<UlidGuidValueGenerator>()
          .ValueGeneratedNever()
          .HasConversion<UlidGuidToBytesConverter>()
          .HasConversion<UlidGuidToStringConverter>();
    }
  }

  private static void ApplyTPCStrategyToIdentifiableEntities(ModelBuilder modelBuilder)
  {
    var identifiableEntities = modelBuilder.Model
      .GetEntityTypes()
      .Where(e => e.ClrType.IsAssignableTo(typeof(IIdentifiable)))
      .ToImmutableArray();
    foreach (var entityType in identifiableEntities)
    {
      modelBuilder
        .Entity(entityType.ClrType)
        .UseTpcMappingStrategy();
    }
  }

  private static void ApplyEnumToStringConverter(ModelBuilder modelBuilder)
  {
    var enumProperties = modelBuilder.Model
      .GetEntityTypes()
      .SelectMany(e => e.GetProperties())
      .Where(p => p.ClrType.IsEnum)
      .ToImmutableArray();
    foreach (var property in enumProperties)
    {
      var converterType = typeof(EnumToStringConverter<>).MakeGenericType(property.ClrType);
      var converter = (ValueConverter?)Activator.CreateInstance(converterType);
      property.SetValueConverter(converter);
    }
  }
}
