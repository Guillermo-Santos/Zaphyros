using Microsoft.Extensions.Logging;

namespace Zaphyros.Core.Logging
{
    internal sealed class DebuggerLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new Logger(categoryName, LoggingExtension.LogDebugger);
        }

        public void Dispose()
        {
        }
    }
}
