using Avanpost.Interviews.Task.Integration.Data.Models;
using Avanpost.Interviews.Task.Integration.Data.Models.Models;

namespace Avanpost.Interviews.Task.Integration.SandBox.Connector;

public class ConnectorDb : IConnector
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor
  private ILogger _logger;
#pragma warning restore CS8618
  private UserService? _userService;
  private PermissionService? _permissionService;
  private ResultDispatcher? _resultDispatcher;
  public void StartUp(string connectionString)
  {
    ConnectorContextFactory factory = new(connectionString);
    _userService = new(new(factory));
    _permissionService = new(new(factory));
  }

  public void CreateUser(UserToCreate user)
  {
    var result = _userService?.CreateUser(user);
    _resultDispatcher?.Dispatch
    (
      result,
      $"Пользователь {user.Login} добавлен",
      $"Пользователь {user.Login} не добавлен"
    );
  }

  public IEnumerable<Property> GetAllProperties()
  {
    var properties = _userService?.GetAllProperties();
    _resultDispatcher?.Dispatch
    (
      properties,
      "Получены свойства сущности",
      "Ошибка при получении всех свойств сущности"
    );

    return properties?.Value ?? Array.Empty<Property>();
  }

  public IEnumerable<UserProperty> GetUserProperties(string userLogin)
  {
    var properties = _userService?.GetUserProperties(userLogin);
    _resultDispatcher?.Dispatch
    (
      properties,
      $"Получены свойства пользователя {userLogin}",
      $"Ошибка при получении всех свойств пользователя {userLogin}"
    );

    return properties?.Value ?? Array.Empty<UserProperty>();
  }

  public bool IsUserExists(string userLogin)
    => _userService?.IsUserExists(userLogin) ?? false;

  public void UpdateUserProperties(IEnumerable<UserProperty> properties, string userLogin)
  {
    var result = _userService?.UpdateUserProperties(properties, userLogin);
    _resultDispatcher?.Dispatch
    (
      result,
      $"Свойства пользователя {userLogin} обновлены",
      $"Свойства пользователя {userLogin} не обновлены"
    );
  }

  public IEnumerable<Permission> GetAllPermissions()
    => _permissionService?.GetAllPermissions() ?? Array.Empty<Permission>();

  public void AddUserPermissions(string userLogin, IEnumerable<string> rightIds)
  {
    var result = _permissionService?.AddUserPermissions(userLogin, rightIds);
    _resultDispatcher?.Dispatch
    (
      result,
      $"Права пользователя {userLogin} добавлены",
      $"Не удалось добавить права пользователя {userLogin}"
    );
  }

  public void RemoveUserPermissions(string userLogin, IEnumerable<string> rightIds)
  {
    var result = _permissionService?.RemoveUserPermissions(userLogin, rightIds);
    _resultDispatcher?.Dispatch
    (
      result,
      $"Права пользователя {userLogin} удалены",
      $"Не удалось удалить права пользователя {userLogin}"
    );
  }

  public IEnumerable<string> GetUserPermissions(string userLogin)
  {
    var permissionsResult = _permissionService?.GetUserPermissions(userLogin);
    _resultDispatcher?.Dispatch
    (
      permissionsResult,
      $"Права пользователя {userLogin} получены",
      $"Не удалось получить права пользователя {userLogin}"
    );

    return permissionsResult?.Value ?? Array.Empty<string>();
  }

  public ILogger Logger
  {
    get => _logger;
    set
    {
      _logger = value;
      _resultDispatcher = new(new LoggerDispatcher(_logger));
    }
  }
}