using System.Reflection;
using Avanpost.Interviews.Task.Integration.Data.DbCommon.DbModels;

namespace Avanpost.Interviews.Task.Integration.SandBox.Connector;
internal static class Extension
{
  public static void FillEmptyStringProperties<T>(this T obj, string fill = "") where T : class
  {
    PropertyInfo[] properties = typeof(User).GetProperties();
    foreach (var property in properties)
    {
      if (property.GetValue(obj) is null && property.PropertyType.Name == "String")
      {
        property.SetValue(obj, fill);
      }
    }
  }

  public static User MapToUser(this UserEntity userSrc)
  {
    var user = new User() { Login = userSrc.Login };
    user.ApplyPropertiesToObject(userSrc.Properties);
    user.FillEmptyStringProperties();
    return user;
  }

  public static Sequrity MapToSequrity(this UserEntity userSrc)
  {
    var sequrity = new Sequrity() { UserId = userSrc.Login, Password = userSrc.Password.Value };
    return sequrity;
  }

  public static Password? MapToPassword(this Sequrity sequrity)
  {
    var password = Password.Create(sequrity.Password);
    return password.Value;
  }
  public static void ApplyPropertiesToObject<T>(this T obj, IDictionary<string, string> properties)
  {
    var bindingFlags = BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance;
    foreach (var property in properties)
    {
      PropertyInfo prop = typeof(User).GetProperty(property.Key, bindingFlags)!;
      if (prop != null)
      {
        object value = Convert.ChangeType(property.Value, prop.PropertyType);
        prop.SetValue(obj, value);
      }
    }
  }
}