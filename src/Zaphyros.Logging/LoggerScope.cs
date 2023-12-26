namespace Zaphyros.Logging
{
    internal class LoggerScope<TState> : IDisposable
    {
        private readonly TState _state;
        private readonly IScopable _logger;
        private bool _disposed;

        public LoggerScope(TState state, IScopable logger)
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
