namespace Zaphyros.Logging
{
    internal sealed class DebuggerLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new Logger(categoryName, LoggerExtension.LogDebugger);
        }

        public void Dispose()
        {
        }
    }
}
