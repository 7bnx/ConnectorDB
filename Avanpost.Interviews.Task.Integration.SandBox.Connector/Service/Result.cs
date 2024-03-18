namespace Avanpost.Interviews.Task.Integration.SandBox.Connector;

internal interface IResult
{
  string? Message { get; }
  IReadOnlyList<string>? Messages { get; }
  Exception? Exception { get; init; }
  bool IsOk { get; }
}

internal sealed class Result<TResult> : IResult
{
  public TResult? Value { get; init; }
  private readonly List<string> _messages = new();
  public IReadOnlyList<string> Messages => _messages.AsReadOnly();
  public string? Message => _messages is not null && _messages.Count > 0 ? string.Join(". ", _messages) : null;
  public Exception? Exception { get; init; }
  public Result(IResult src, TResult result = default!) : this(result, src.Message, src.Exception) { }
  public Result(IResult src, TResult result, string? message = null)
  {
    Value = result;
    Exception = src.Exception;
    if (message != default)
      _messages.Add(message);
    _messages.AddRange(src.Messages!);
  }
  public Result(TResult? result, string? message = null, Exception? exception = null)
  {
    Value = result;
    Exception = exception;
    AddMessage(message);
  }
  public bool IsOk
    => Exception is null;
  public Result(string? message) : this(default, message, null) { }
  public Result(Exception? exception)
  {
    Exception = exception;
  }
  public Result(string? message, Exception? exception) : this(default, message, exception) { }
  public Result(TResult? result, IEnumerable<string> messages) : this(result, messages, null) { }

  public Result(TResult? result, IEnumerable<string> messages, Exception? exception)
  {
    Value = result;
    _messages.AddRange(messages);
    Exception = exception;
  }


  public Result<TResult> AddMessage(string? message)
  {
    if (!string.IsNullOrEmpty(message))
      _messages.Add(message);
    return this;
  }
  public Result<TResult> AddMessages(IEnumerable<string> messages)
  {
    if (messages is not null)
      _messages.AddRange(messages);
    return this;
  }
}