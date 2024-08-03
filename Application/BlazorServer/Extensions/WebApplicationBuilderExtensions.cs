using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TauResourceCalculator.Application.BlazorServer.Settings;
using TauResourceCalculator.Infrastructure.Data;
using TauResourceCalculator.Infrastructure.Data.SQLite;

namespace TauResourceCalculator.Application.BlazorServer.Extensions;

internal static class WebApplicationBuilderExtensions
{
  public static WebApplicationBuilder AddApplicationDbContext(this WebApplicationBuilder builder)
  {
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
      ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    var applicationSettings = builder.Configuration.GetSection("App").Get<ApplicationSettings>()
      ?? throw new InvalidOperationException("'App' settings section not found.");

    switch (applicationSettings.DatabaseType)
    {
      case DatabaseType.SQLite:
        {
          AddApplicationSQLiteDbContext(builder.Services, connectionString);
          break;
        }
      default:
        throw new InvalidOperationException("ApplicationSettings 'DatabaseType' is not correct.");
    }

    return builder;
  }

  private static void AddApplicationSQLiteDbContext(IServiceCollection services, string connectionString)
  {
    services
      .AddDbContext<ApplicationSQLiteDbContext>((provider, options) =>
      {
        // Это должно быть в коробке! Но почему то этого нет, хотя раньше вроде было.
        var configurators = provider.GetServices<IConfigureOptions<DbContextOptionsBuilder>>();
        foreach (var configurator in configurators)
          configurator.Configure(options);
      })
      .AddScoped<ApplicationDbContext>(p => p.GetRequiredService<ApplicationSQLiteDbContext>())
      .Configure<DbContextOptionsBuilder>(options =>
      {
        options
          .EnableSensitiveDataLogging()
          .UseLazyLoadingProxies()
          .UseSqlite(connectionString);
      });
  }
}
