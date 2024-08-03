using Microsoft.EntityFrameworkCore;

namespace TauResourceCalculator.BlazorServer.Data.SQLite;

internal sealed class ApplicationSQLiteDbContext : ApplicationDbContext
{
  public ApplicationSQLiteDbContext(DbContextOptions<ApplicationSQLiteDbContext> options)
    : base(options)
  {
  }
}
