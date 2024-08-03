using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using TauResourceCalculator.BlazorServer.Data.Configurations;
using TauResourceCalculator.BlazorServer.Interfaces;
using TauResourceCalculator.BlazorServer.Models;

namespace TauResourceCalculator.BlazorServer.Data;

internal abstract class ApplicationDbContext : DbContext
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
      .ApplyConfiguration(new ProjectEntityConfiguration())
      .ApplyConfiguration(new SprintEntityConfiguration());

    ApplyTPCStrategy(builder);
    ApplyEnumToStringConverter(builder);

    base.OnModelCreating(builder);
  }

  private static void ApplyTPCStrategy(ModelBuilder modelBuilder)
  {
    var identifiableEntities = modelBuilder.Model
      .GetEntityTypes()
      .Where(e => e.ClrType.IsAssignableTo(typeof(IIdentifiable)))
      .ToImmutableArray();
    foreach (var entityType in identifiableEntities)
    {
      modelBuilder
        .Entity(entityType.ClrType)
          .UseTpcMappingStrategy()
          .Property(nameof(IIdentifiable.Id))
            .HasValueGenerator<GuidValueGenerator>()
            .ValueGeneratedNever();
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
