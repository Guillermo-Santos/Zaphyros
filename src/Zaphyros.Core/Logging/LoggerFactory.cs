using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Zaphyros.Core.Logging
{
    public class LoggerFactory : ILoggerFactory
    {
        private readonly List<ILoggerProvider> loggerProviders = new();

        public void AddProvider(ILoggerProvider provider)
        {
            loggerProviders.Add(provider);
        }

        public ILogger CreateLogger(string categoryName)
        {
            switch (loggerProviders.Count)
            {
                case 0:// Default Logger
                    return NullLoggerProvider.Instance.CreateLogger(categoryName);
                case 1:// Ungrapped Logger
                    return loggerProviders[0].CreateLogger(categoryName);
                default:// Multi Logger
                {
                    var loggers = new List<ILogger>();
                    foreach (var loggerProvider in loggerProviders)
                    {
                        loggers.Add(loggerProvider.CreateLogger(categoryName));
                    }

                    return new MultiTargetLogger(loggers);
                }
            }
        }

        public void Dispose()
        {
            foreach (var provider in loggerProviders)
            {
                provider.Dispose();
            }
            loggerProviders.Clear();
        }
    }
}
