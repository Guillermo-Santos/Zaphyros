using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos.System.Coroutines;
using Zaphyros.Core.Apps;
using BCrypt.Net;

namespace Zaphyros.Core.Users.Services
{
    internal sealed class WorkFactorCalculatorService : Service
    {
        private const string PASSWORD = "Zaphyros";
        private const int ADMIN_WORK_FACTOR_MODIFIER = 2;
        private int workFactor;
        private double maxTime;

        public WorkFactorCalculatorService()
        {
        }

        public override void BeforeStart()
        {
            workFactor = 12;
            maxTime = 2500;
        }
        public override void Update()
        {
            Sys.Global.Debugger.Send("Work Factor: " + workFactor);
            var start = DateTime.UtcNow;
            _ = BCrypt.Net.BCrypt.HashPassword(PASSWORD, workFactor);
            var end = DateTime.UtcNow;

            var timeLapse = (end - start).TotalMilliseconds;
            Sys.Global.Debugger.Send("Miliseconds: " + timeLapse);

            if (timeLapse >= maxTime || workFactor == 31)
            {
                Sys.Global.Debugger.Send("Getting Normal");
                PasswordConstants.NormalUser.WorkFactor = workFactor;
                Sys.Global.Debugger.Send("Getting Admin");
                PasswordConstants.AdminUser.WorkFactor = workFactor + ADMIN_WORK_FACTOR_MODIFIER;

                Sys.Global.Debugger.Send("Stopping");
                Stop();
            }

            workFactor++;
        }
        public override void AfterStart()
        {
            Sys.Global.Debugger.Send($"Registering Service {nameof(PasswordCheckerService)}");
            Kernel.TaskManager.RegisterService(new PasswordCheckerService());
        }
    }
}
