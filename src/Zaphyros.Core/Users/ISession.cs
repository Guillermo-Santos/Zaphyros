using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Zaphyros.Core.Apps;
using Zaphyros.Core.Configuration;

namespace Zaphyros.Core.Users
{

    internal abstract class Session
    {
        public string Name => User.Name;
        public User User { get; init; }
        public bool IsActive { get; set; }


        public Session(User user)
        {
            User = user;
        }

        public virtual void Start() { }
        public abstract void Update();
        public virtual void Stop() { }

    }

    internal class TerminalSession : Session
    {
        public static string Key => "899F7432-3F58-4994-A8D0-F312DAD5319B";
        public string CurrentWorkingDirectory { get; set; } = Environment.CurrentDirectory;

        public TerminalSession(User user) : base(user)
        {

        }

        public override void Update()
        {
            throw new NotImplementedException();
        }
    }

    internal class SessionManagerService : Service
    {
        public SessionManagerService() : base()
        {
        }

        public override void Update()
        {
            throw new NotImplementedException();
        }
    }
}
