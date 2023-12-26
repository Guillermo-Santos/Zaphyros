using Zaphyros.Logging.Providers;

namespace Zaphyros.Logging
{
    public static class LoggerFactoryExtensions
    {
        public static ILoggerFactory UseConsole(this ILoggerFactory loggerFactory)
        {
            loggerFactory.AddProvider(new ConsoleLoggerProvider());
            return loggerFactory;
        }

        public static ILoggerFactory UseDebugger(this ILoggerFactory loggerFactory)
        {
            loggerFactory.AddProvider(new DebuggerLoggerProvider());
            return loggerFactory;
        }
    }
}
