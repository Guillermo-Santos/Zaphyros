namespace Zaphyros.Logging
{
    internal sealed class ConsoleLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new Logger(categoryName, LoggerExtension.LogConsole);
        }

        public void Dispose()
        {
        }
    }
}
