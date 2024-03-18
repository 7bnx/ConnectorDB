using System.Data.Common;

internal sealed class RawConnectionStringParser
{
  public string Provider { get; init; }
  public string ConnectionString { get; }
  private const string PRIVIDER_POSTGRE_TRIGGER = "PostgreSQL";
  private const string PRIVIDER_MSSQL_TRIGGER = "SqlServer";
  private const string PROVIDER_POSTGRE = "POSTGRE";
  private const string PROVIDER_MSSQL = "MSSQL";
  public RawConnectionStringParser(string connectionString)
  {
    var connectionStringBuilder = new DbConnectionStringBuilder { ConnectionString = connectionString };
    ConnectionString = connectionStringBuilder["ConnectionString"].ToString()!;
    var provider = connectionStringBuilder["Provider"].ToString();
    if (provider?.StartsWith(PRIVIDER_POSTGRE_TRIGGER, StringComparison.OrdinalIgnoreCase) ?? false)
      Provider = PROVIDER_POSTGRE;
    else if (provider?.StartsWith(PRIVIDER_MSSQL_TRIGGER, StringComparison.OrdinalIgnoreCase) ?? false)
      Provider = PROVIDER_MSSQL;
    else
      throw new Exception("Неопределенный провайдер - " + provider);
  }
}