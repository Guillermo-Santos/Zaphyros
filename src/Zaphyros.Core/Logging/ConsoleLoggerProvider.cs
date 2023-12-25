using Microsoft.Extensions.Logging;

namespace Zaphyros.Core.Logging
{
    internal sealed class ConsoleLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new Logger(categoryName, LoggingExtension.LogConsole);
        }

        public void Dispose()
        {
        }
    }
}
