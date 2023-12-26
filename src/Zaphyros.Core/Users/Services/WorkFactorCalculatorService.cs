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
            workFactor = 10;
            maxTime = 500;
        }
        public override void Update()
        {
            Console.WriteLine("Work Factor: " + workFactor);
            var start = DateTime.UtcNow;
            _ = BCrypt.Net.BCrypt.EnhancedHashPassword(PASSWORD, workFactor, HashType.SHA256);
            var end = DateTime.UtcNow;

            var timeLapse = (end - start).TotalMilliseconds;
            Console.WriteLine("Miliseconds: " + timeLapse);

            if (timeLapse > maxTime || workFactor == 31)
            {
                Console.WriteLine("Getting Normal");
                PasswordConstants.NormalUser.WorkFactor = workFactor;
                Console.WriteLine("Getting Admin");
                PasswordConstants.AdminUser.WorkFactor = workFactor + ADMIN_WORK_FACTOR_MODIFIER;

                Console.WriteLine("Stopping");
                Stop();
            }

            workFactor++;
        }
        public override void AfterStart()
        {
            Console.WriteLine($"Registering Service {nameof(PasswordCheckerService)}");
            Kernel.TaskManager.RegisterService(new PasswordCheckerService());
        }
    }
}
