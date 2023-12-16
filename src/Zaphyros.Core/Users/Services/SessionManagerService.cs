using Zaphyros.Core.Apps;

namespace Zaphyros.Core.Users.Services
{
    internal class SessionManagerService : Service
    {
        private List<Session> _sessions;
        public SessionManagerService() : base()
        {
            _sessions = new();
        }

        public override void Update()
        {
            foreach (var session in _sessions)
            {
                session.Update();
            }
        }

        internal void RegisterSession(Session session)
        {
            Sys.Global.Debugger.Send($"Registering Session {session.GetType().Name}");
            _sessions.Add(session);
        }
    }
}
