using Avanpost.Interviews.Task.Integration.Data.DbCommon.DbModels;
using Microsoft.EntityFrameworkCore;

namespace Avanpost.Interviews.Task.Integration.SandBox.Connector;
internal sealed class PermissionsRepository : RepositoryBase
{
  public PermissionsRepository(ConnectorContextFactory connectorContextFactory)
    : base(connectorContextFactory) { }

  public List<ITRole> GetRolesPermissions()
  {
    using var context = _dbContextFactory.GetContext();
    return context.ITRoles.AsNoTracking().ToList();
  }

  public List<RequestRight> GetRequestsPermissions()
  {
    using var context = _dbContextFactory.GetContext();
    return context.RequestRights.AsNoTracking().ToList();
  }

  public List<string> GetRequestsPermission(string userLogin)
  {
    using var context = _dbContextFactory.GetContext();
    return
      context.UserRequestRights.AsNoTracking()
      .Where(urr => urr.UserId == userLogin)
      .Select(urr => $"{urr.RightId}").ToList();
  }

  public List<string> GetRolesPermission(string userLogin)
  {
    using var context = _dbContextFactory.GetContext();
    return
      context.UserITRoles.AsNoTracking()
      .Where(role => role.UserId == userLogin)
      .Select(role => $"{role.RoleId}")
      .ToList();
  }

  public Result<bool> RemovePermissions
  (
    string userLogin,
    IEnumerable<int> rolesIds,
    IEnumerable<int> requestsIds
  )
  {
    using var context = _dbContextFactory.GetContext();
    var rolesToRemove = context.UserITRoles
                        .Where(role => role.UserId == userLogin && rolesIds.Contains(role.RoleId));
    var rightsToRemove = context.UserRequestRights
                        .Where(right => right.UserId == userLogin && requestsIds.Contains(right.RightId));
    context.ChangeTracker.AutoDetectChangesEnabled = false;

    context.UserITRoles.RemoveRange(rolesToRemove);
    context.UserRequestRights.RemoveRange(rightsToRemove);

    context.ChangeTracker.DetectChanges();
    return Save(context);
  }

  public Result<bool> AddUsersPermissions
  (
    IEnumerable<UserITRole> rolesPermissions,
    IEnumerable<UserRequestRight> requestsPermissions
  )
  {
    using var context = _dbContextFactory.GetContext();
    context.ChangeTracker.AutoDetectChangesEnabled = false;

    context.UserITRoles.AddRange(rolesPermissions);
    context.UserRequestRights.AddRange(requestsPermissions);

    context.ChangeTracker.DetectChanges();
    return Save(context);
  }

  public List<int> GetNewRolesPermissionsIdsForUser(string userLogin, IEnumerable<int> newRolesIds)
  {
    using var context = _dbContextFactory.GetContext();
    return (
      from x in context.ITRoles.AsNoTracking()
      where x.Id.HasValue && newRolesIds.Contains(x.Id.Value)
      select x.Id.Value)
      .Except(
      from x in context.UserITRoles.AsNoTracking()
      where x.UserId == userLogin
      select x.RoleId)
      .ToList();
  }

  public List<int> GetNewRequestsPermissionsIdsForUser(string userLogin, IEnumerable<int> newRequestsIds)
  {
    using var context = _dbContextFactory.GetContext();
    return (
      from x in context.RequestRights.AsNoTracking()
      where x.Id.HasValue && newRequestsIds.Contains(x.Id.Value)
      select x.Id.Value)
      .Except(
      from x in context.UserRequestRights.AsNoTracking()
      where x.UserId == userLogin
      select x.RightId)
      .ToList();
  }
}