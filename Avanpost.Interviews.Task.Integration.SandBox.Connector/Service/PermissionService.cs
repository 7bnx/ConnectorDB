using Avanpost.Interviews.Task.Integration.Data.DbCommon.DbModels;
using Avanpost.Interviews.Task.Integration.Data.Models.Models;

namespace Avanpost.Interviews.Task.Integration.SandBox.Connector;
internal sealed class PermissionService
{
  private const string DELIMITER = ":";
  private const string ROLE_RIGHT = "Role";
  private const string REQUEST_RIGHT = "Request";

  private readonly PermissionsRepository _permissionsRepository;
  private readonly PermissionsExtractor _permissionsExtractor;

  public PermissionService(PermissionsRepository permissionsRepository)
  {
    _permissionsRepository = permissionsRepository;
    _permissionsExtractor = new(DELIMITER, ROLE_RIGHT, REQUEST_RIGHT);
  }

  public IEnumerable<Permission> GetAllPermissions()
  {
    return _permissionsRepository.GetRolesPermissions()
      .Select(role => new Permission($"{role.Id}", role.Name, ROLE_RIGHT))
      .Concat(
      _permissionsRepository.GetRequestsPermissions()
      .Select(right => new Permission($"{right.Id}", right.Name, REQUEST_RIGHT)))
      .ToArray();
  }

  public Result<bool> AddUserPermissions(string userLogin, IEnumerable<string> rightIds)
  {
    if (!_permissionsRepository.IsUserExists(userLogin))
      return new
      (
        false,
        exception: new ArgumentException($"Пользователя {userLogin} не существует")
      );

    var parseRes = _permissionsExtractor.ParseIds(rightIds, out var idsRoles, out var idsRequests);

    var rolesIdsToAdd = _permissionsRepository.GetNewRolesPermissionsIdsForUser(userLogin, idsRoles);
    var rightsIdsToAdd = _permissionsRepository.GetNewRequestsPermissionsIdsForUser(userLogin, idsRequests);

    var newUserRoles = rolesIdsToAdd.Select(id => new UserITRole { RoleId = id, UserId = userLogin });
    var newUserRights = rightsIdsToAdd.Select(id => new UserRequestRight { RightId = id, UserId = userLogin });

    var result = _permissionsRepository.AddUsersPermissions(newUserRoles, newUserRights);

    return result.AddMessages(parseRes.Messages);
  }

  public Result<bool> RemoveUserPermissions(string userLogin, IEnumerable<string> rightIds)
  {
    if (!_permissionsRepository.IsUserExists(userLogin))
      return new
      (
        false,
        exception: new ArgumentException($"Пользователя {userLogin} не существует")
      );

    var parseRes = _permissionsExtractor.ParseIds(rightIds, out var idsRoles, out var idsRequests);

    var result = _permissionsRepository.RemovePermissions(userLogin, idsRoles, idsRequests);

    return result.AddMessages(parseRes.Messages);
  }

  public Result<IEnumerable<string>> GetUserPermissions(string userLogin)
  {
    if (!_permissionsRepository.IsUserExists(userLogin))
      return new
      (
        Array.Empty<string>(),
        exception: new ArgumentException($"Пользователя {userLogin} не существует")
      );

    return new(
      _permissionsRepository.GetRequestsPermission(userLogin).Select(r => $"{REQUEST_RIGHT}{DELIMITER}{r}")
      .Concat(
      _permissionsRepository.GetRolesPermission(userLogin).Select(r => $"{ROLE_RIGHT}{DELIMITER}{r}")));
  }

}