using System.Collections.ObjectModel;
using Avanpost.Interviews.Task.Integration.Data.Models.Models;

namespace Avanpost.Interviews.Task.Integration.SandBox.Connector;
internal sealed class UserEntity
{
  public string Login { get; }
  public ReadOnlyDictionary<string, string> Properties => _properties.AsReadOnly();
  public Password Password { get; private set; }
  private readonly Dictionary<string, string> _properties;
  private UserEntity(string login, Dictionary<string, string> properties, Password password)
  {
    Login = login;
    _properties = properties;
    Password = password;
  }

  public Result<bool> UpdateProperties(IEnumerable<UserProperty> properties)
  {
    var messages = new List<string>();
    foreach (var property in properties)
    {
      if (property.Name.Equals("Login", StringComparison.OrdinalIgnoreCase))
      {
        messages.Add($"Нельзя изменить логин {Login} пользователя на {property.Value}");
        continue;
      }
      if (property.Name.Equals("Password", StringComparison.OrdinalIgnoreCase))
      {

        var newPassword = Password.Create(property.Value);
        if (newPassword.IsOk && newPassword.Value is not null)
          Password = newPassword.Value;
        else if (newPassword?.Exception?.Message is not null)
          messages.Add(newPassword.Exception.Message);
        continue;
      }

      if (_properties.ContainsKey(property.Name))
      {
        if (string.IsNullOrEmpty(property.Value))
          messages.Add($"Свойство {property.Name} не может быть нулевым или пустым");
        else
          _properties[property.Name] = property.Value;
      }
      else
        messages.Add($"У пользователя {Login} нет свойства {property.Name}");
    }
    return new(true, messages);
  }

  public static Result<UserEntity> Create(string userLogin, IEnumerable<UserProperty> properties, Password password)
  {
    if (string.IsNullOrEmpty(userLogin))
    {
      return new(new ArgumentException("Логин не может быть нулевым или пустым", $"{nameof(userLogin)}"));
    }

    var propertiesToAdd = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    var messages = new List<string>();
    var isLeadAdded = false;
    foreach (var p in properties)
    {
      if (string.IsNullOrEmpty(p.Value))
      {
        messages.Add($"Свойство {p.Name} не может быть нулевым или пустым");
        continue;
      }
      var propertyName = p.Name.ToLower();
      if (!propertiesToAdd.TryAdd(propertyName, p.Value))
        messages.Add($"Свойство {p.Name} уже добавлено со значением {propertiesToAdd[propertyName]}");

      if (!isLeadAdded && propertyName.Equals("isLead", StringComparison.OrdinalIgnoreCase))
        isLeadAdded = true;

    }
    if (!isLeadAdded)
      return new(null, messages, new ArgumentException("Свойства пользователя не содержат обязательного параметра 'isLead'"));

    return new(new UserEntity(userLogin, propertiesToAdd, password), messages);
  }
}