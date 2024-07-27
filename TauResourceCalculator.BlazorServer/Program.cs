using System.IO;
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
using TauResourceCalculator.BlazorServer.Components;
using TauResourceCalculator.BlazorServer.Data;
using TauResourceCalculator.BlazorServer.Extensions;
using TauResourceCalculator.BlazorServer.Settings;

namespace TauResourceCalculator.BlazorServer;

public sealed class Program
{
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
      settingsPath = "appsettings.json";

    var configuration = builder.Configuration
      .SetBasePath(CurrentDirectory)
      .AddJsonFile(settingsPath, optional: true, reloadOnChange: false)
      .AddEnvironmentVariables()
      .AddCommandLine(args)
      .Build();

    var appConfigSection = configuration.GetSection("App");
    builder.Services.Configure<ApplicationSettings>(appConfigSection);

    builder.Services.AddDatabaseDeveloperPageExceptionFilter();
    builder.AddApplicationDbContext();

    var app = builder.Build();

    if (!app.Environment.IsDevelopment())
    {
      app.UseExceptionHandler("/Error");
    }

    app.UseStaticFiles();
    app.UseAntiforgery();

    app
      .MapRazorComponents<App>()
      .AddInteractiveServerRenderMode();

    await using (var scope = app.Services.CreateAsyncScope())
    {
      var scopeServiceProvider = scope.ServiceProvider;
      await using (var context = scopeServiceProvider.GetRequiredService<ApplicationDbContext>())
      {
        await context.Database.MigrateAsync();
      }
    }

    await app.RunAsync();
  }
}
