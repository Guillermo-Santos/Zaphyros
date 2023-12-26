namespace Zaphyros.Logging
{
    public interface IScopable
    {
        public void RemoveScope(IDisposable scope);
    }
}
