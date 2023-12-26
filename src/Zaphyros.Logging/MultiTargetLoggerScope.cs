namespace Zaphyros.Logging
{
    internal sealed class MultiTargetLoggerScope<TState> : LoggerScope<TState>
    {
        private readonly IEnumerable<IDisposable?> loggerScopes;

        public MultiTargetLoggerScope(TState state, IScopable logger, IEnumerable<IDisposable?> loggerScopes) : base(state, logger)
        {
            this.loggerScopes = loggerScopes;
        }

        public override string ToString()
        {
            string scopeString = string.Empty;
            foreach (var scope in loggerScopes)
            {
                scopeString = $"{scopeString}.{scope?.ToString()}";
            }
            return scopeString;
        }

        public new void Dispose()
        {
            foreach (var scope in loggerScopes)
            {
                scope?.Dispose();
            }
            base.Dispose();
        }
    }
}
