namespace Zaphyros.Core.Logging
{
    public interface IScopable
    {
        public void RemoveScope(IDisposable scope);
    }
}
