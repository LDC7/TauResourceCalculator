using Microsoft.EntityFrameworkCore;
using TauResourceCalculator.BlazorServer.Models;

namespace TauResourceCalculator.BlazorServer.Data.SQLite;

internal sealed class ApplicationSQLiteDbContext : ApplicationDbContext
{
  public DbSet<Team> Teams { get; set; }

  public ApplicationSQLiteDbContext(DbContextOptions<ApplicationSQLiteDbContext> options)
    : base(options)
  {
  }
}
