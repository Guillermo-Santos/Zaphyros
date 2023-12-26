namespace Zaphyros.Logging
{
    public sealed class MultiTargetLogger : IScopable, ILogger
    {
        private readonly IEnumerable<ILogger> loggers;
        private readonly List<IDisposable> _scopes = new();

        public MultiTargetLogger(IEnumerable<ILogger> loggers)
        {
            this.loggers = loggers;
            this.Log(LogLevel.Debug, 0, "Creating new MultiTargetLogger", null, (string state, Exception? error) => state.ToString());
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            foreach (var logger in loggers)
            {
                switch (logger)
                {
                    case Logger log:
                        if (log.IsEnabled(logLevel))
                        {
                            log.Log(logLevel, eventId, state, exception, formatter);
                        }
                        break;
                    default:
                        if (logger.IsEnabled(logLevel))
                        {
                            logger.Log(logLevel, eventId, state, exception, formatter);
                        }
                        break;
                }
            }

            // Remove disposed scopes
            _scopes.RemoveAll(scope => (scope as LoggerScope<TState>)?.Disposed ?? false);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            var scopes = new List<IDisposable?>();
            foreach (var logger in loggers)
            {
                scopes.Add(logger.BeginScope(state));
            }

            var scope = new MultiTargetLoggerScope<TState>(state, this, scopes);
            _scopes.Add(scope);
            return scope;
        }

        public void RemoveScope(IDisposable scope)
        {
            _scopes.Remove(scope);
        }
    }
}
