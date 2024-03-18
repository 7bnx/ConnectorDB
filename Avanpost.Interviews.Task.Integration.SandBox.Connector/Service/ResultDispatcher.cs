using Avanpost.Interviews.Task.Integration.Data.Models;

namespace Avanpost.Interviews.Task.Integration.SandBox.Connector;

internal sealed class ResultDispatcher
{
  private readonly IDispatcher _dispatcher;

  public ResultDispatcher(IDispatcher dispatcher)
  {
    _dispatcher = dispatcher;
  }

  public void Dispatch(IResult? result, string failedMsg)
    => Dispatch(result, string.Empty, failedMsg);

  public void Dispatch(IResult? result, string successMsg, string failedMsg)
  {
    var message = result?.Message;
    if (!result?.IsOk ?? false)
      _dispatcher.Error($"{failedMsg}. {result?.Exception?.Message} | {message}");
    else if (!string.IsNullOrEmpty(message))
      _dispatcher.Warn($"{successMsg}. {message}");
    else
      _dispatcher.Debug($"{successMsg}");
  }
}

internal interface IDispatcher
{
  void Debug(string message);
  void Warn(string message);
  void Error(string message);
}

internal sealed class LoggerDispatcher : IDispatcher
{
  private readonly ILogger _logger;

  public LoggerDispatcher(ILogger logger)
  {
    _logger = logger;
  }
  public void Debug(string message)
  {
    _logger.Debug(message);
  }

  public void Error(string message)
  {
    _logger.Error(message);
  }

  public void Warn(string message)
  {
    _logger.Warn(message);
  }
}