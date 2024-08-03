namespace TauResourceCalculator.BlazorServer.Settings;

public enum DatabaseType
{
  SQLite
}

public sealed class ApplicationSettings
{
  public DatabaseType DatabaseType { get; set; }
}
