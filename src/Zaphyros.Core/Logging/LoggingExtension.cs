using Microsoft.Extensions.Logging;

namespace Zaphyros.Core.Logging
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
            loggerFactory.AddProvider(new  DebuggerLoggerProvider());
            return loggerFactory;
        }
    }
    internal static class LoggingExtension
    {
        public static void LogConsole(LogLevel logLevel, string scope, string message)
        {
            var foregroundColor = Console.ForegroundColor;
            Console.Write("[");
            Console.ForegroundColor = logLevel switch
            {
                LogLevel.Trace => ConsoleColor.DarkCyan,
                LogLevel.Debug => ConsoleColor.Cyan,
                LogLevel.Information => ConsoleColor.Green,
                LogLevel.Warning => ConsoleColor.Yellow,
                LogLevel.Error or LogLevel.Critical => ConsoleColor.Red,
                _ => foregroundColor,
            };
            Console.Write(GetLogLevelString(logLevel));
            Console.ForegroundColor = foregroundColor;
            Console.WriteLine($"]{scope}:{message}");
        }

        public static void LogDebugger(LogLevel logLevel, string scope, string message)
        {
            Sys.Global.Debugger.Send($"[{GetLogLevelString(logLevel)}]{scope}:{message}");
        }

        public static string GetLogLevelString(LogLevel logLevel)
        {
            return logLevel switch
            {
                LogLevel.Debug => "DEBUG",
                LogLevel.Trace => "TRACE",
                LogLevel.Information => "INFO",
                LogLevel.Warning => "WARN",
                LogLevel.Error => "ERROR",
                LogLevel.Critical => "CRITICAL",
                LogLevel.None => string.Empty,
                _ => throw new ArgumentOutOfRangeException(nameof(logLevel)),
            };
        }
    }
}
