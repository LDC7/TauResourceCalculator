using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TauResourceCalculator.Infrastructure.Data.SQLite;

internal sealed class ApplicationSQLiteDbContextDesignFactory : IDesignTimeDbContextFactory<ApplicationSQLiteDbContext>
{
  public ApplicationSQLiteDbContext CreateDbContext(string[] args)
  {
    var optionsBuilder = new DbContextOptionsBuilder<ApplicationSQLiteDbContext>();
    optionsBuilder.UseSqlite("DataSource=./test.db;Cache=Shared");

    return new ApplicationSQLiteDbContext(optionsBuilder.Options);
  }
}
