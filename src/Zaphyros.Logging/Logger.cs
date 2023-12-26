namespace Zaphyros.Logging
{
    public sealed class Logger : IScopable, ILogger
    {
        private readonly string _categoryName;
        private readonly Action<LogLevel, string, string> logWriter;
        private readonly List<IDisposable> _scopes = new();

        public Logger(string categoryName, Action<LogLevel, string, string> logWriter)
        {
            _categoryName = categoryName;
            this.logWriter = logWriter;
            Log(LogLevel.Debug, 0, "Creating new Logger", null, (string state, Exception? error) => state.ToString());
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!this.IsEnabled(logLevel))
            {
                return;
            }

            string scopeMessage = _categoryName;
            // Log information about the active scopes
            foreach (var scope in _scopes)
            {
                scopeMessage = $"{scopeMessage}.{scope}";
            }

            logWriter(logLevel, scopeMessage, $"{_categoryName}[{eventId.Id}] - {formatter(state, exception)}");

            // Remove disposed scopes
            _scopes.RemoveAll(scope => (scope as LoggerScope<TState>)?.Disposed ?? false);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            // In a real logger, you might implement more sophisticated logic based on log level.
            return true;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            Log(LogLevel.Debug, 0, "Registering Scope", null, (string state, Exception? error) => state.ToString());
            var scope = new LoggerScope<TState>(state, this);
            _scopes.Add(scope);
            Log(LogLevel.Debug, 0, "Scope Registered", null, (string state, Exception? error) => state.ToString());
            return scope;
        }

        public void RemoveScope(IDisposable scope)
        {
            _scopes.Remove(scope);
        }
    }
}
