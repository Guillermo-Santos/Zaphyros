using Microsoft.Extensions.Logging;

namespace Zaphyros.Core.Logging
{
    internal sealed class MultiTargetLogger : ILogger, IScopable
    {
        internal readonly IEnumerable<ILogger> loggers;
        private readonly List<IDisposable> _scopes = new List<IDisposable>();

        public MultiTargetLogger(IEnumerable<ILogger> loggers)
        {
            this.loggers = loggers;
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

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            foreach (var logger in loggers)
            {
                if (logger.IsEnabled(logLevel))
                {
                    logger.Log(logLevel, eventId, state, exception, formatter);
                }
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
