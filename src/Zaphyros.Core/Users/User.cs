namespace Zaphyros.Core.Users
{
    internal class User
    {
        private readonly List<PermissionEntry> _permisions;

        public string Name { get; private set; }
        public UserType UserType { get; private set; }
        public bool IsAdmin => UserType is UserType.Admin;
        public bool IsNormal => UserType is UserType.Normal;
        public IReadOnlyCollection<PermissionEntry> Permisions => _permisions;

        public UserEntry? UserEntry => UserEntry.GetUserEntries().FirstOrDefault(e => e.Name == this.Name);

        public User(string name, UserType userType)
        {
            Name = name;
            UserType = userType;

            Init();
        }

        private void Init()
        {
        }


        public bool CanRead(string path) => HavePermision(PermissionLevel.Read, path);
        public bool CanWrite(string path) => HavePermision(PermissionLevel.Write, path);
        public bool CanExecute(string path) => HavePermision(PermissionLevel.Execute, path);

        public bool HavePermision(PermissionLevel permission, string path)
        {
            return _permisions.Where(p => ((p.Level & permission) > 0) && (p.Path.Length <= path.Length))
                .OrderByDescending(p => p.Path.Length).Any(p =>
                path == p.Path
                || (path.StartsWith(p.Path) && (p.Level & PermissionLevel.Recursive) > 0)
                );
        }
    }
}
