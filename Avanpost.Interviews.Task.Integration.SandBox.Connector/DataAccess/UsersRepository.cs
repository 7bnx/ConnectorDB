using System.Data;
using Avanpost.Interviews.Task.Integration.Data.DbCommon.DbModels;
using Avanpost.Interviews.Task.Integration.Data.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace Avanpost.Interviews.Task.Integration.SandBox.Connector;
internal sealed class UsersRepository : RepositoryBase
{
  public UsersRepository(ConnectorContextFactory dbContextFactory)
    : base(dbContextFactory) { }

  public List<Property>? GetAllProperties()
  {
    using var context = _dbContextFactory.GetContext();

    return context
      .Model
      .FindEntityType(typeof(User))?
      .GetProperties()
      .Where(x => !x.IsKey())
      .Select(x => new Property(x.Name, $"Тип: {x.PropertyInfo!.PropertyType.Name}"))
      .ToList();
  }

  public IEnumerable<UserProperty> GetUserProperties(string userLogin)
  {
    using var context = _dbContextFactory.GetContext();
    var user = context.Users.First(u => u.Login == userLogin);

    return
      context.Entry(user)
      .Properties.Where(p => !p.Metadata.IsKey())
      .Select(p => new UserProperty(p.Metadata.Name, $"{p.CurrentValue}"))
      .ToList();
  }

  public Result<bool> CreateUser(UserEntity userEntity)
  {
    var user = userEntity.MapToUser();
    var password = userEntity.MapToSequrity();
    using var context = _dbContextFactory.GetContext();
    context.ChangeTracker.AutoDetectChangesEnabled = false;

    context.Users.Add(user);
    context.Passwords.Add(password);

    context.ChangeTracker.DetectChanges();
    return Save(context);
  }

  public UserEntity? GetUserEntity(string userLogin)
  {
    using var context = _dbContextFactory.GetContext();
    var password = context.Passwords.AsNoTracking().FirstOrDefault(p => p.UserId == userLogin);
    var properties = GetUserProperties(userLogin);
    var user = UserEntity.Create(userLogin, properties, password!.MapToPassword()!);
    return user.Value;
  }

  public Result<bool> UpdateUserEntity(UserEntity userEntity)
  {
    var userUpdated = userEntity.MapToUser();
    var passwordUpdated = userEntity.MapToSequrity();
    using var context = _dbContextFactory.GetContext();
    context.ChangeTracker.AutoDetectChangesEnabled = false;

    var user = context.Users.FirstOrDefault(u => u.Login == userUpdated.Login);
    if (user is null)
      return new(false);
    context.Entry(user).CurrentValues.SetValues(userUpdated);

    var password = context.Passwords.FirstOrDefault(p => p.UserId == userUpdated.Login);
    if (password is not null)
    {
      passwordUpdated.Id = password.Id;
      context.Entry(password).CurrentValues.SetValues(passwordUpdated);
    }
    else
      context.Passwords.Add(passwordUpdated);

    context.ChangeTracker.DetectChanges();
    return Save(context);
  }
}