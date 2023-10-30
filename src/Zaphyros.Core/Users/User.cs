namespace Zaphyros.Core.Users
{
    [Serializable]
    internal struct User
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public UserType UserType { get; set; }
        public readonly bool IsAdmin => UserType is UserType.Admin;
        public readonly bool IsNormal => UserType is UserType.Normal;
    }
}
