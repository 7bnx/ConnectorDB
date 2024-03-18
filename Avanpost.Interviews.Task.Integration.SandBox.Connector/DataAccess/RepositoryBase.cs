using Avanpost.Interviews.Task.Integration.Data.DbCommon;
using Microsoft.EntityFrameworkCore;

namespace Avanpost.Interviews.Task.Integration.SandBox.Connector;
internal abstract class RepositoryBase
{
  protected readonly ConnectorContextFactory _dbContextFactory;
  protected RepositoryBase(ConnectorContextFactory dbContextFactory)
  {
    _dbContextFactory = dbContextFactory;
  }

  public bool IsUserExists(string userLogin)
  {
    using var context = _dbContextFactory.GetContext();
    return context.Users.Any(u => u.Login == userLogin);
  }

  protected static Result<bool> Save(DataContext context)
  {
    try
    {
      context.SaveChanges();
      return new(true);
    }
    catch (DbUpdateException ex)
    {
      return new(false, "Не удалось сохранить в базу данных", ex.InnerException ?? ex);
    }
  }
}