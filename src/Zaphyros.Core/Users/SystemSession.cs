using Zaphyros.Core.Apps;
using Zaphyros.Core.Users.Services;

namespace Zaphyros.Core.Users
{
    internal class SystemSession : Session
    {
        public SystemSession() : base(new("SYSTEM", UserType.Admin))
        {
            var home = EnvironmentVariables["HOME"];
            home.Value = "0:\\System";
        }

        public override void Start()
        {
            Kernel.TaskManager.RegisterService(new WorkFactorCalculatorService());
        }

        public override void Update()
        {
            // TODO: SYSTEM User will do some miscellaneous tasks, but for now he would just do nothing...
        }
    }
}
