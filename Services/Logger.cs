

using Microsoft.Extensions.Logging;

public class Logger : ILogger
{
  private string filePath { get; set; } = "logs.txt";
  public IDisposable? BeginScope<TState>(TState state) where TState : notnull
  {
    return null;
  }

  public bool IsEnabled(LogLevel logLevel)
  {
    return true;
  }

  public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
  {

    var logMessage = formatter(state, exception);
    switch (logLevel)
    {
      case LogLevel.Critical:
      case LogLevel.Error:
      case LogLevel.Warning:
        File.AppendAllText("./errors" + filePath, logMessage + Environment.NewLine);
        Console.WriteLine("🔥" + logMessage);
        break;
      default:
        Console.WriteLine(logMessage);
        break;
    }
  }
}