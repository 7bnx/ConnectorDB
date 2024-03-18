namespace Avanpost.Interviews.Task.Integration.SandBox.Connector;
internal sealed class PermissionsExtractor
{
  private readonly string _delimiter;
  private readonly string _roleRight;
  private readonly string _requestRight;

  public PermissionsExtractor(string delimiter, string roleRight, string requestRight)
  {
    _delimiter = delimiter;
    _roleRight = roleRight;
    _requestRight = requestRight;
  }
  public Result<bool> ParseIds(IEnumerable<string> permissions, out List<int> rolesIds, out List<int> requestsIds)
  {
    var result = new Result<bool>(true);
    (rolesIds, requestsIds) = (new(), new());
    foreach (var right in permissions)
    {
      var permission = right.Split(_delimiter);
      if (permission is null || permission.Length != 2)
      {
        result.AddMessage($"Неправильный формат прав пользователя {permission}. Пример 'groupName:id'.");
        continue;
      }
      var group = permission[0].Trim();
      if (!int.TryParse(permission[1], out var id))
      {
        result.AddMessage($"Параметр 'id' должен быть числом {right}.");
        continue;
      }
      if (group.Equals(_roleRight, StringComparison.OrdinalIgnoreCase))
        rolesIds.Add(id);
      else if (group.Equals(_requestRight, StringComparison.OrdinalIgnoreCase))
        requestsIds.Add(id);
      else
        result.AddMessage($"Неизвестная группа прав пользователя для добавления: {group}.");
    }
    return result;
  }
}