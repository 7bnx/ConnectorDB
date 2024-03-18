using Avanpost.Interviews.Task.Integration.Data.DbCommon;

internal sealed class ConnectorContextFactory
{
  private readonly DbContextFactory _dbContextFactory;
  private readonly string _provider;
  private readonly string _connectionString;
  public ConnectorContextFactory(string connectionString)
  {
    RawConnectionStringParser parser = new(connectionString);
    _provider = parser.Provider;
    _connectionString = parser.ConnectionString;
    _dbContextFactory = new(_connectionString);
  }
  public DataContext GetContext()
  {
    return _dbContextFactory.GetContext(_provider);
  }
}