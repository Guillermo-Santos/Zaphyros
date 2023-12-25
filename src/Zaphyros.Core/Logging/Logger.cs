using Microsoft.Extensions.Logging;

namespace Zaphyros.Core.Logging
{
    internal sealed class Logger : ILogger, IScopable
    {
        private readonly string _categoryName;
        private readonly Action<LogLevel, string, string>? logWriter;
        private readonly List<IDisposable> _scopes = new List<IDisposable>();

        public Logger(string categoryName, Action<LogLevel, string, string>? logWriter)
        {
            _categoryName = categoryName;
            this.logWriter = logWriter;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            var scope = new LoggerScope<TState>(state, this);
            _scopes.Add(scope);
            return scope;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            // In a real logger, you might implement more sophisticated logic based on log level.
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            string scopeMessage = _categoryName;
            // Log information about the active scopes
            foreach (var scope in _scopes)
            {
                scopeMessage = $"{scopeMessage}.{scope}";
            }

            if(logWriter is not null)
            {
                logWriter(logLevel, scopeMessage, $"{_categoryName}[{eventId.Id}] - {formatter(state, exception)}");
            }

            // Remove disposed scopes
            _scopes.RemoveAll(scope => (scope as LoggerScope<TState>)?.Disposed ?? false);
        }

        public void RemoveScope(IDisposable scope)
        {
            _scopes.Remove(scope);
        }
    }
}
