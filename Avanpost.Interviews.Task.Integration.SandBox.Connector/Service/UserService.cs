using Avanpost.Interviews.Task.Integration.Data.DbCommon.DbModels;
using Avanpost.Interviews.Task.Integration.Data.Models.Models;

namespace Avanpost.Interviews.Task.Integration.SandBox.Connector;
internal sealed class UserService
{
  private readonly UsersRepository _usersRepository;

  public UserService(UsersRepository usersRepository)
  {
    _usersRepository = usersRepository;
  }

  public Result<bool> CreateUser(UserToCreate user)
  {
    if (IsUserExists(user.Login))
      return new
      (
        false,
        exception: new ArgumentException($"Пользователь {user.Login} уже существует")
      );

    var passwordResult = Password.Create(user.Login);
    if (!passwordResult.IsOk)
      return new(passwordResult, false);

    var userEntityResult = UserEntity.Create(user.Login, user.Properties, passwordResult.Value!);
    if (!userEntityResult.IsOk)
      return new(userEntityResult, false);

    return
      _usersRepository.CreateUser(userEntityResult.Value!)
      .AddMessages(passwordResult.Messages)
      .AddMessages(userEntityResult.Messages);
  }

  public Result<IEnumerable<Property>> GetAllProperties()
  {
    var properties = _usersRepository.GetAllProperties();
    if (properties is null || properties.Count == 0)
      return new
      (
        Array.Empty<Property>(),
        exception: new ArgumentException($"Таблицы {nameof(User)} не существует или она не содержит полей")
      );

    return new(properties);
  }

  public Result<IEnumerable<UserProperty>> GetUserProperties(string userLogin)
  {
    if (!_usersRepository.IsUserExists(userLogin))
      return new
      (
        Array.Empty<UserProperty>(),
        exception: new ArgumentException($"Пользователя {userLogin} не существует")
      );

    return new(_usersRepository.GetUserProperties(userLogin));
  }

  public bool IsUserExists(string userLogin)
    => _usersRepository.IsUserExists(userLogin);

  public Result<bool> UpdateUserProperties(IEnumerable<UserProperty> properties, string userLogin)
  {
    var existedUser = _usersRepository.GetUserEntity(userLogin);
    if (existedUser is null)
      return new
      (
        false,
        exception: new ArgumentException($"Пользователя {userLogin} не существует")
      );
    ;

    existedUser.UpdateProperties(properties);
    var result = _usersRepository.UpdateUserEntity(existedUser);
    return result;
  }
}