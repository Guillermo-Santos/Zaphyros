namespace Zaphyros.Logging
{
    public interface IScopableLogger : ILogger
    {
        public void RemoveScope(IDisposable scope);
    }
}
