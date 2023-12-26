namespace Zaphyros.Logging.Scopes
{
    internal class LoggerScope<TState> : IDisposable
    {
        private readonly TState _state;
        private readonly IScopableLogger _logger;
        private bool _disposed;

        public LoggerScope(TState state, IScopableLogger logger)
        {
            _state = state;
            _logger = logger;
        }

        public override string ToString()
        {
            return _disposed ? string.Empty : _state?.ToString() ?? string.Empty;
        }

        public void Dispose()
        {
            _disposed = true;

            _logger.RemoveScope(this);
        }

        public bool Disposed => _disposed;
    }
}
