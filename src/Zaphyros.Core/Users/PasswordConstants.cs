using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using Zaphyros.Core.Users;

namespace Zaphyros.Core.Users
{
    public sealed class PasswordConfiguration
    {
        public UserType UserType { get; init; }
        public int WorkFactor { get; set; }
        public HashType HashType { get; init; }
    }

    internal static class PasswordConstants
    {
        private static readonly PasswordConfiguration adminUser = new()
        {
            UserType = UserType.Admin,
            HashType = HashType.SHA384
        };

        public static readonly PasswordConfiguration NormalUser = new()
        {
            UserType = UserType.Normal,
            HashType = HashType.SHA256
        };

        public static PasswordConfiguration AdminUser => adminUser;
    }
}
