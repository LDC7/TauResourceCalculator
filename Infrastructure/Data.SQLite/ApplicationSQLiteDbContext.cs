using Microsoft.EntityFrameworkCore;

namespace TauResourceCalculator.Infrastructure.Data.SQLite;

public sealed class ApplicationSQLiteDbContext : ApplicationDbContext
{
  public ApplicationSQLiteDbContext(DbContextOptions<ApplicationSQLiteDbContext> options)
    : base(options)
  {
  }
}
