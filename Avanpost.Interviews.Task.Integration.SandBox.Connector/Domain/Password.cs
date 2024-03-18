namespace Avanpost.Interviews.Task.Integration.SandBox.Connector;
internal sealed class Password
{
  public string Value { get; }
  private Password(string value)
  {
    Value = value;
  }

  public static Result<Password> Create(string value)
  {
    if (string.IsNullOrEmpty(value))
      return new(new ArgumentException("Пароль не может быть нулевым или пустым ", $"{nameof(value)}"));
    return new(new Password(value));
  }
}