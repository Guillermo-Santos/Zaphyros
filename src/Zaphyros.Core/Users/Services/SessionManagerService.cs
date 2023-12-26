using Zaphyros.Core.Apps;

namespace Zaphyros.Core.Users.Services
{
    internal class SessionManagerService : Service
    {
        private List<Session> _sessions;
        private static SessionManagerService _instance;

        public static SessionManagerService Instance => _instance ??= new();
        public SessionManagerService() : base()
        {
            _sessions = new();
        }

        public override void BeforeStart()
        {
            foreach (var session in _sessions)
            {
                session.Start();
            }
        }

        public override void Update()
        {
            foreach (var session in _sessions)
            {
                session.Update();
            }
        }

        public override void AfterStart()
        {
            foreach (var session in _sessions)
            {
                session.Stop();
            }
        }

        internal void RegisterSession(Session session)
        {
            Sys.Global.Debugger.Send($"Registering {session.GetType().Name}-{session.Name}");
            _sessions.Add(session);
        }
    }
}
