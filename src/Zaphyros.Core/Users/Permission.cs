using System.Linq;

namespace Zaphyros.Core.Users
{
    public struct PermissionEntry
    {
        public string Path;
        public PermissionLevel Level;

        public PermissionEntry(string path, int level) : this()
        {
            Path = path;
            Level = (PermissionLevel)level;
        }
    }
}
