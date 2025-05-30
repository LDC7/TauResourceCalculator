﻿using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using OfficeOpenXml;
using TauResourceCalculator.Application.BlazorServer.Components;
using TauResourceCalculator.Application.BlazorServer.Context;
using TauResourceCalculator.Application.BlazorServer.Extensions;
using TauResourceCalculator.Application.BlazorServer.Services;
using TauResourceCalculator.Application.BlazorServer.Settings;
using TauResourceCalculator.Common.Abstractions;
using TauResourceCalculator.Infrastructure.Data;

namespace TauResourceCalculator;

public sealed class Program
{
  private const string DefaultSettingsFileName = "appsettings.json";
  private const string ExampleSettingsFileName = "appsettings.example.json";

  internal static string CurrentDirectory { get; } = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;

  public static async Task Main(string[] args)
  {
    Directory.SetCurrentDirectory(CurrentDirectory);
    var webApplicationOptions = new WebApplicationOptions()
    {
      Args = args,
      ContentRootPath = CurrentDirectory,
      WebRootPath = $"{CurrentDirectory}/wwwroot"
    };
    var builder = WebApplication.CreateBuilder(webApplicationOptions);

    builder.Services.AddLogging(config => config.AddConsole());

    builder.Services
      .AddRazorComponents()
      .AddInteractiveServerComponents();

    builder.Services.AddMudServices();

    var settingsPath = args.FirstOrDefault();
    if (settingsPath == null || !File.Exists(settingsPath))
      settingsPath = DefaultSettingsFileName;

    if (settingsPath == DefaultSettingsFileName && !File.Exists(settingsPath) && File.Exists(ExampleSettingsFileName))
    {
      File.Copy(ExampleSettingsFileName, settingsPath, false);
    }

    var configuration = builder.Configuration
      .SetBasePath(CurrentDirectory)
      .AddJsonFile(settingsPath, optional: true, reloadOnChange: false)
      .AddEnvironmentVariables()
      .AddCommandLine(args)
      .Build();

    var appConfigSection = configuration.GetSection("App");
    builder.Services.Configure<ApplicationSettings>(appConfigSection);

    builder.AddApplicationDbContext();

    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

    builder.Services
      .AddSingleton<DownloadFileContext>()
      .AddTransient(typeof(IRepository<>), typeof(EntityFrameworkRepository<>))
      .AddTransient<XlsxReportService>();

    builder.Services.AddControllers();

    var app = builder.Build();

    if (!app.Environment.IsDevelopment())
    {
      app.UseExceptionHandler("/Error", createScopeForErrors: true);
    }

    app.UseStaticFiles();
    app.UseAntiforgery();

    app.MapControllerRoute(name: "default", "{controller}/{action}");
    app.MapControllers();

    app
      .MapRazorComponents<App>()
      .AddInteractiveServerRenderMode();

    await using (var scope = app.Services.CreateAsyncScope())
    {
      await using (var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
      {
        await context.Database.MigrateAsync();
      }
    }

    await app.RunAsync();
  }
}
