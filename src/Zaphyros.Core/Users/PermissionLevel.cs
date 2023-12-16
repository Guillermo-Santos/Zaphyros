namespace Zaphyros.Core.Users
{
    [Flags]
    public enum PermissionLevel
    {
        None = 0,
        Write = 1,
        Read = 2,
        Execute = 4,
        Recursive = 8,
    }
}
