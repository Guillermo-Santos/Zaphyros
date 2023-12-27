using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Cosmos.Core.Memory;
using Zaphyros.Core.Configuration;

namespace Zaphyros.Core.Users
{
    public struct EnvironmentVariable
    {
        public string Name;
        public string Value;
        public bool SystemManaged;
    }

    public abstract class Session
    {
        public string Name => User.Name;
        public User User { get; private set; }
        public SessionState State { get; protected set; }

        public Dictionary<string, EnvironmentVariable> EnvironmentVariables { get; private set; }

        public Session(User user)
        {
            User = user;
            var home = new EnvironmentVariable()
            {
                Name = "HOME",
                Value = $"0:\\Users\\{User.Name}",
                SystemManaged = true
            };
            EnvironmentVariables = new()
            {
                { home.Name, home }
            };

        }

        public virtual void Start() { }
        public abstract void Update();
        public virtual void Stop() { }

    }
}
