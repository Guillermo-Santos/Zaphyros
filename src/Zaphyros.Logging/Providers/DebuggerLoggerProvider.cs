namespace Zaphyros.Logging.Providers
{
    public sealed class DebuggerLoggerProvider : ILoggerProvider
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
